using InDebt.Constants;

namespace InDebt.Helpers;

public abstract class StartupHelper
{
    public static void EnvironmentVariableSettings(IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConfigurationConstants.EnvironmentConnectionString);
        var exchangeRateApiKey = Environment.GetEnvironmentVariable(ConfigurationConstants.EnvironmentExchangeRateApiKey);
        var authKey = Environment.GetEnvironmentVariable(ConfigurationConstants.EnvironmentAuthKey);
        var mailServiceHost = Environment.GetEnvironmentVariable(ConfigurationConstants.EnvironmentMailServiceHost);
        var mailServiceLogin = Environment.GetEnvironmentVariable(ConfigurationConstants.EnvironmentMailServiceLogin);
        var mailServicePassword = Environment.GetEnvironmentVariable(ConfigurationConstants.EnvironmentMailServicePassword);

        configuration["Authentication:Key"] = authKey;
        configuration["ConnectionStrings:DefaultConnection"] = connectionString;
        configuration["Serilog:WriteTo:0:Args:connectionString"] = connectionString;
        configuration["EmailServiceConfiguration:Host"] = mailServiceHost;
        configuration["EmailServiceConfiguration:SenderAddress"] = mailServiceLogin;
        configuration["EmailServiceConfiguration:Password"] = mailServicePassword;
        configuration["ExchangeRate:ApiKey"] = exchangeRateApiKey;
    }
}