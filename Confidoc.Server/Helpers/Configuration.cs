using Serilog;

namespace Confidoc.Server.Helpers
{
    public static class Configuration
    {
        private static readonly Dictionary<string, string> defaultValues = new()
        {
            {"CONFIDOC_DATABASE", "sqlite"},
            {"CONFIDOC_CONNECTION", "Data Source=confidoc.db"},
            {"CONFIDOC_JWT_SECRET", "testingtestingTesting1234!Teeeestinng!"},
            {"CONFIDOC_JWT_ISSUER", "https://localhost:5173"},
            {"CONFIDOC_JWT_AUDIENCE", "https://localhost:5173"},
            {"CONFIDOC_JWT_EXPIRES", "60"}, // 1h
            {"PASSWORD_REQUIRE_DIGITS", "true"},
            {"PASSWORD_REQUIRE_NONALPHA", "true"},
            {"PASSWORD_REQUIRE_UPPER", "true"},
            {"PASSWORD_REQUIRE_LOWER", "true"},
            {"LOG_TYPE", "console"},
            {"LOG_OUT", "null"},
            {"LOG_LEVEL", "debug"}
        };


        /// <summary>
        /// Reads .env file and makes them readable by <see cref="GetEnvVariable(string)"/>
        /// </summary>
        public static void InitConfigs()
        {
            if (!File.Exists(".env")) return;

            foreach (var line in File.ReadAllLines(".env"))
            {
                if (!line.Contains("=")) continue;
                var sections = line.Split('=');
                var key = sections[0].Trim();
                var value = line.TrimStart($"{key}=").Trim().ToString();
                Environment.SetEnvironmentVariable(key, value);
                Log.Debug($".env parsed \"{key}={value}\"");
            }
        }

        /// <summary>
        /// Attempts to retrieve environment variable and returns
        /// the default value if no such variable exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEnvVariable(string name)
            => Environment.GetEnvironmentVariable(name) ?? defaultValues[name];
    }
}
