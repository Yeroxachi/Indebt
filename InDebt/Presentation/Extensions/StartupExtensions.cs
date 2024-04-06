using Application.Context;
using Application.Services;
using Domain.Models;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.EntityConfigurations;
using System.Text;

namespace InDebt.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddManagementServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IMergeRequestService, MergeRequestService>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IGroupService, GroupService>();
        serviceCollection.AddScoped<IAuthService, AuthService>();
        serviceCollection.AddScoped<IGroupInviteService, GroupInviteService>();
        serviceCollection.AddScoped<IEmailService, EmailService>();
        serviceCollection.AddScoped<IDebtService, DebtService>();
        serviceCollection.AddScoped<ITransactionService, TransactionService>();
        serviceCollection.AddScoped<IBalanceService, BalanceService>();
        serviceCollection.AddScoped<IDebtOptimizationService, DebtOptimizationService>();
        serviceCollection.AddScoped<ICurrencyService, CurrencyService>();
        serviceCollection.AddScoped<IExchangeRateService, ExchangeRateService>();
        serviceCollection.AddScoped<IRatingService, RatingService>();
        serviceCollection.AddScoped<INotificationService, NotificationService>();
        return serviceCollection;
    }

    public static IServiceCollection AddMapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(expression => expression
            .AddMaps(nameof(Application)));
        return serviceCollection;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<InDebtContext>(options =>
            options.UseSqlServer(connectionString));
        serviceCollection.AddScoped<IInDebtContext, InDebtContext>();
        return serviceCollection;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection,
        AuthOptions options)
    {
        serviceCollection.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var key = Encoding.UTF8.GetBytes(options.Key);
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });
        return serviceCollection;
    }

    public static IServiceCollection AddSwaggerSecurity(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            c.SwaggerDoc(name: "v1", new OpenApiInfo
            {
                Title = "InDebt",
                Version = "v1"
            });
        });
    }

    public static IServiceCollection AddHangfireServices(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
            config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions()
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            });
        });
        serviceCollection.AddHangfireServer();

        return serviceCollection;
    }

    public static async Task InitializeDbContext(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<InDebtContext>();
        await dataContext.Database.MigrateAsync();
        if (!dataContext.Currencies.Any())
        {
            await CurrencyConfiguration.SeedCurrencyData(dataContext);
        }
    }
}