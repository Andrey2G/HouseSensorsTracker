using SensorServer.Services.TgCommands;
using System.Reflection;

namespace SensorServer.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBotCommands(this IServiceCollection services)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var typesToRegister = callingAssembly.GetTypes().Where(x => typeof(ITgBotCommand).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var typeToRegister in typesToRegister)
            {
                services.AddTransient(typeof(ITgBotCommand), typeToRegister);
            }

            return services;
        }
    }
}
