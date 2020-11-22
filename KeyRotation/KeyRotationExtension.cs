using Microsoft.Extensions.DependencyInjection;

namespace KeyRotationSample.KeyRotation
{
    public static class KeyRotationExtension
    {
        /// <summary>
        /// Adds the KeyRotation to services via DI.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>The IServiceCollection</returns>
        public static IServiceCollection AddKeyRotation(this IServiceCollection services)
        {
            services.AddSingleton<IKeyRotation, KeyRotationHelper>();
            return services;
        }
    }
}
