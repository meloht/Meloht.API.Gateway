namespace Meloht.API.Gateway.Common.HealthCheck
{
    public static class HealthCheckAPI
    {
        public const string HealthCheckPath = "/health/live";

        public const int HealthCheckIntervalSeconds = 120;
        public const int HealthChecTimeoutSeconds = 5;
    }
}
