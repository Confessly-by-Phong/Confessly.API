using Microsoft.Extensions.Configuration;

namespace Confessly.Configuration
{
    public static class ConfesslyConfiguration
    {
        #region Private Members to get configuration
        private static IConfiguration? configuration;

        private static IConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    throw new Exception("Configuration has not been initialized yet!");
                }
                return configuration;
            }
        }
        #endregion

        #region Initialize the configuration
        public static void Initialize(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region Public Configuration Fields
        public static string ConnectionString(string connectionStringKey)
            => Configuration.GetConnectionString(connectionStringKey) ?? "";

        public static string JWTAudience
            => Configuration.GetSection("JWT")["Audience"] ?? "";

        public static string JWTIssuer
            => Configuration.GetSection("JWT")["Issuer"] ?? "";

        public static string JWTSecret
            => Configuration.GetSection("JWT")["Secret"] ?? "";

        public static int JWTExpirationMinutes
            => int.Parse(Configuration.GetSection("JWT")["ExpirationMinutes"] ?? "15");
        #endregion
    }
}
