using System;
using System.Security.Claims;
using System.Text;
using Domain.Enum;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Riders.Config.Autorization;

namespace Riders.Config.Token;

public static class TokenService
{

    public static IServiceCollection AddTokenService(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddSingleton<IAuthorizationHandler, MinimumAccessLevelHandler>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // Se a requisição for para o nosso hub, leia o token da query string
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/locationhub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };

        });

        services.AddAuthorization(options =>
        {
            // Para cada nível de acesso, criamos uma política que usa nosso requisito personalizado.
            // Um usuário Admin (nível 1) passará na política "CommonUser" (nível 6), pois 1 <= 6.
            options.AddPolicy(nameof(AccessLevel.Programmer), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.Programmer)));
            options.AddPolicy(nameof(AccessLevel.Admin), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.Admin)));
            options.AddPolicy(nameof(AccessLevel.Manager), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.Manager)));
            options.AddPolicy(nameof(AccessLevel.Coordinator), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.Coordinator)));
            options.AddPolicy(nameof(AccessLevel.Employee), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.Employee)));
            options.AddPolicy(nameof(AccessLevel.SuperUser), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.SuperUser)));
            options.AddPolicy(nameof(AccessLevel.CommonUser), policy => policy.AddRequirements(new MinimumAccessLevelRequirement(AccessLevel.CommonUser)));
        });

        return services;
    }
}
