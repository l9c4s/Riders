using System;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;

namespace Riders.Config.Autorization;

public class MinimumAccessLevelRequirement : IAuthorizationRequirement
    {
        public AccessLevel MinimumLevel { get; }

        public MinimumAccessLevelRequirement(AccessLevel minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }
    }
