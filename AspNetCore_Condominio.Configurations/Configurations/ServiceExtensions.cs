using AspNetCore_Condominio.Application.Behaviors;
using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Features.Empresas.Commands.Create;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.Create;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.Create;
using AspNetCore_Condominio.Configurations.Converters;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
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
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace AspNetCore_Condominio.Configurations.Configurations;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("ApplicationDbContext")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonDateOnlyConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AuthLoginQuery).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(CreateCommandValidatorEmpresa).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateCommandValidatorImovel).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateCommandValidatorMorador).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IAuthUserRepository, AuthUserRepository>();

        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IImovelRepository, ImovelRepository>();
        services.AddScoped<IMoradorRepository, MoradorRepository>();

        services.AddSingleton<TokenService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", policy =>
                policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

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
                ValidateAudience = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"]
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("Suporte"));
        });

        return services;
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
                Description = description,
                Contact = new OpenApiContact
                {
                    Name = "Waine Alves Carneiro",
                    Email = "carneirowaine@gmail.com",
                    Url = new Uri("https://github.com/WaineAlvesCarneiro?tab=repositories")
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                opt.IncludeXmlComments(xmlPath);
            }

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Insira o token JWT no formato: Bearer {seu token}"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }
}
