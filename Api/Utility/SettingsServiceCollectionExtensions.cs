using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Utility {
    public static class SettingsServiceCollectionExtensions {

        // This is to avoid needing to use IOptions<T> everwhere just to support reloading of
        // settings, which I think we're unlikely to use (and if we do the code will need to be
        // designed to support it). See also (similar but not identical suggestions):
        // https://www.strathweb.com/2016/09/strongly-typed-configuration-in-asp-net-core-without-ioptionst/
        // https://stackoverflow.com/questions/41150178/ioptions-injection
        public static void AddSettings<T>(this IServiceCollection services) where T : class, new() {
            services.AddSingleton(s => s.GetService<IConfiguration>().GetSettings<T>());
        }

        public static T AddSettings<T>(this IServiceCollection services, IConfiguration configuration) where T : class, new() {
            T settings = configuration.GetSettings<T>();
            services.AddSingleton(settings);

            return settings;
        }

        private static T GetSettings<T>(this IConfiguration configuration) where T : class, new() {
            string name = typeof(T).Name;
            const string suffix = "Settings";
            if (name.EndsWith(suffix)) {
                name = name.Substring(0, name.Length - suffix.Length);
            }

            T settings = ConfigurationBinder.Get<T>(
                configuration.GetSection(name));
            return settings;
        }
    }
}
