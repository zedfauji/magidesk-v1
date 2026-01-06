using System;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Services
{
    /// <summary>
    /// Helper for safe service resolution with automatic error reporting.
    /// </summary>
    public static class ServiceResolutionHelper
    {
        /// <summary>
        /// Safely resolves a required service with automatic error reporting.
        /// </summary>
        public static T? GetServiceSafely<T>(IServiceProvider services, IErrorService errorService) where T : class
        {
            try
            {
                return services.GetRequiredService<T>();
            }
            catch (Exception ex)
            {
                _ = errorService.ShowFatalAsync(
                    "Service Missing", 
                    $"Required service {typeof(T).Name} is not registered.", 
                    ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Safely resolves an optional service with automatic error reporting.
        /// </summary>
        public static T? GetOptionalServiceSafely<T>(IServiceProvider services, IErrorService errorService) where T : class
        {
            try
            {
                return services.GetService<T>();
            }
            catch (Exception ex)
            {
                _ = errorService.ShowWarningAsync(
                    "Service Resolution Failed", 
                    $"Optional service {typeof(T).Name} could not be resolved.", 
                    ex.ToString());
                return null;
            }
        }
    }
}