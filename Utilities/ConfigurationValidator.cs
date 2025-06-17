using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

public static class ConfigurationValidator
{
    public static void ValidateProductionSecrets(IConfiguration configuration, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            var criticalSettings = new[]
            {
                "DATABASE_CONNECTION_STRING",
                "JWT_SECRET"
            };

            foreach (var setting in criticalSettings)
            {
                var value = Environment.GetEnvironmentVariable(setting);
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException($"Production environment variable '{setting}' is required.");
                }
            }
        }
    }
}
