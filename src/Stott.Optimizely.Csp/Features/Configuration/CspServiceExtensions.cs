﻿using Microsoft.Extensions.DependencyInjection;

using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.Permissions.Save;

namespace Stott.Optimizely.Csp.Features.Configuration
{
    public static class CspServiceExtensions
    {
        public static IServiceCollection AddCspManager(this IServiceCollection services)
        {
            services.AddTransient<ICspPermissionsRepository, CspPermissionsRepository>();
            services.AddTransient<ICspPermissionsViewModelBuilder, CspPermissionsViewModelBuilder>();
            services.AddTransient<ISaveCspPermissionsCommand, SaveCspPermissionsCommand>();

            return services;
        }
    }
}
