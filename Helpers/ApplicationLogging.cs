using Microsoft.Extensions.Logging;

namespace SeedApi.Helpers {

    public class ApplicationLogging {
        private static ILoggerFactory _factory = null;

        public static void ConfigureLogger(ILoggerFactory factory) {
            _factory = factory;
        }

        public static ILogger CreateLogger(string name) => _factory.CreateLogger(name);
    }
}