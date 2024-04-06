using Domain.Models;
using Hangfire;
using InDebt.Constants;
using InDebt.Extensions;
using InDebt.Helpers;
using InDebt.Jobs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
StartupHelper.EnvironmentVariableSettings(builder.Configuration);
var authOptionsSection = builder.Configuration.GetSection(ConfigurationConstants.Authentication);
var authOptions = authOptionsSection.Get<AuthOptions>();
var emailServiceOptionsSection = builder.Configuration.GetSection(ConfigurationConstants.EmailServiceConfiguration);
var environmentOptionsSection = builder.Configuration.GetSection(ConfigurationConstants.Environment);
var exchangeRateOptions = builder.Configuration.GetSection(ConfigurationConstants.ExchangeRate);
var connectionString = builder.Configuration.GetConnectionString(ConfigurationConstants.DefaultConnection);
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));
builder.Services.Configure<AuthOptions>(authOptionsSection);
builder.Services.Configure<EmailServiceOptions>(emailServiceOptionsSection);
builder.Services.Configure<EnvironmentOptions>(environmentOptionsSection);
builder.Services.Configure<ExchangeRateServiceOptions>(exchangeRateOptions);
builder.Services.AddControllers();
builder.Services.AddManagementServices();
builder.Services.AddMapper();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext(connectionString);
builder.Services.AddJwtAuthentication(authOptions);
builder.Services.AddSwaggerSecurity();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHangfireServices(connectionString);

var app = builder.Build();
await app.InitializeDbContext();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard();

RecurringJobs.Register();

app.MapControllers();

app.Run();