﻿using AspNetCore_Condominio.Application.Behaviors;
using AspNetCore_Condominio.Application.Features.Auth;
using AspNetCore_Condominio.Application.Features.Imoveis.Commands.CreateImovel;
using AspNetCore_Condominio.Application.Features.Moradores.Commands.CreateMorador;
using AspNetCore_Condominio.Application.Features.Moradores.Events;
using AspNetCore_Condominio.Configurations.ServicesJWT;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Data;
using AspNetCore_Condominio.Infrastructure.Repositories;
using AspNetCore_Condominio.Infrastructure.Repositories.Auth;
using AspNetCore_Condominio.Infrastructure.Repositories.Email;
using AspNetCore_Condominio.Infrastructure.Services;
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

namespace AspNetCore_Condominio.Configurations.Configurations;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("ApplicationDbContext")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AuthLoginQuery).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(MoradorCriadoEventHandler).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(CreateImovelCommandValidator).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateMoradorCommandValidator).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Removido, pois será incluído no método de Swagger
        // services.AddEndpointsApiExplorer();

        services.AddScoped<IAuthUserRepository, AuthUserRepository>();

        services.AddScoped<IImovelRepository, ImovelRepository>();
        services.AddScoped<IMoradorRepository, MoradorRepository>();

        services.AddScoped<EmailRepository>();
        services.AddScoped<IEmailService, EmailService>();

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
                policy.RequireRole("Admin"));
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
