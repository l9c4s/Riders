using System;
using System.Security.Claims;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;

namespace Riders.Config.Autorization;

public class MinimumAccessLevelHandler : AuthorizationHandler<MinimumAccessLevelRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAccessLevelRequirement requirement)
        {
            // Pega a claim 'role' do token do usuário.
            var roleClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                return Task.CompletedTask;
            }

            // Converte a string da claim para o nosso enum AccessLevel.
            if (Enum.TryParse<AccessLevel>(roleClaim.Value, out var userLevel))
            {
            // Lógica da hierarquia: um valor de enum MENOR significa um privilégio MAIOR.
            // Se o nível do usuário for menor ou igual ao nível mínimo exigido, ele tem acesso.
                if (userLevel <= requirement.MinimumLevel)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
}
