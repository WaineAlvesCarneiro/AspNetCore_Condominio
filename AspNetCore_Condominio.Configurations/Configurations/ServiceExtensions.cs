using AspNetCore_Condominio.Application.Behaviors;
using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Features.Auth.Commands.Create;
using AspNetCore_Condominio.Configurations.Converters;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Constants;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
using AspNetCore_Condominio.Infrastructure.Messaging;
using AspNetCore_Condominio.Infrastructure.Repositories;
using AspNetCore_Condominio.Infrastructure.Repositories.Auth;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Configurations.Configurations;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddJsonConfigs()
            .AddMediatRAndValidators()
            .AddRepositories()
            .AddInfrastructureServices()
            .AddSecurity(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext")
                ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));
        return services;
    }

    private static IServiceCollection AddJsonConfigs(this IServiceCollection services)
    {
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonDateOnlyConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        return services;
    }

    private static IServiceCollection AddMediatRAndValidators(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthLoginQuery).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateCommandValidatorAuthUser).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IImovelRepository, ImovelRepository>();
        services.AddScoped<IMoradorRepository, MoradorRepository>();
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IMensageriaService, RabbitMQService>();
        services.AddHostedService<EmailConsumerWorker>();
        services.AddSingleton<TokenService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
                policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });
        return services;
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);
        services.AddAppAuthorization();

        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key não está configurada"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    private static void AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            string[] rolesPermitidas = ["Suporte", "Sindico", "Porteiro"];

            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireRole(rolesPermitidas);
                policy.RequireClaim(AuthClaims.PrimeiroAcesso, AuthClaims.FalseValue);
                policy.RequireClaim(AuthClaims.StatusAtivo, AuthClaims.StatusAtivoValue);
                policy.RequireClaim(AuthClaims.EmpresaAtiva, AuthClaims.StatusAtivoValue);
            });

            options.AddPolicy("PermitirTrocaSenha", policy =>
            {
                policy.RequireRole(rolesPermitidas);
                policy.RequireClaim(AuthClaims.PrimeiroAcesso, AuthClaims.TrueValue);
                policy.RequireClaim(AuthClaims.StatusAtivo, AuthClaims.StatusAtivoValue);
                policy.RequireClaim(AuthClaims.EmpresaAtiva, AuthClaims.StatusAtivoValue);
            });
        });
    }

    public static IServiceCollection AddSwaggerAndSecurity(this IServiceCollection services, string title, string description)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = "v1",
                Description = $"{description} Waine Alves Carneiro 🔗 [Acesse meu GitHub](https://github.com/WaineAlvesCarneiro)"
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xmlPath)) opt.IncludeXmlComments(xmlPath);

            ConfigurarSegurancaSwagger(opt);
        });

        return services;
    }

    private static void ConfigurarSegurancaSwagger(SwaggerGenOptions opt)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Insira o token JWT desta forma: Bearer {seu_token}",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };

        opt.AddSecurityDefinition("Bearer", securityScheme);

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        });
    }
}
