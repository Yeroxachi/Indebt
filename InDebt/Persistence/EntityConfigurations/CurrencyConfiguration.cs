using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Persistence.Context;

namespace Persistence.EntityConfigurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasIndex(c => c.CurrencyCode).IsUnique();
    }

    private const string CurrencyResource = "Persistence.SeedData.Currencies.json";

    public static async Task SeedCurrencyData(InDebtContext context)
    {
        await context.AddRangeAsync(GetCurrencies());
        await context.SaveChangesAsync();
    }
        
    private static IEnumerable<Currency> GetCurrencies()
    {
        using var stream = typeof(InDebtContext).Assembly.GetManifestResourceStream(CurrencyResource);
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using StreamReader reader = new(stream);
        var content = reader.ReadToEnd();
        var currencyCodesAndNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        var currencies = currencyCodesAndNames.Select(kvp =>
        {
            var id = Guid.NewGuid();
            return new Currency
            {
                Id = id,
                CurrencyCode = kvp.Key,
                Name = kvp.Value
            };
        });
        return currencies;
    }
}