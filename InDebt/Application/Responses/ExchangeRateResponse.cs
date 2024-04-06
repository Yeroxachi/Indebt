using Newtonsoft.Json;

namespace Application.Responses;

public record ExchangeRateResponse
{
    [JsonProperty(PropertyName = "base_code")]
    public required string BaseCode { get; init; }
    [JsonProperty(PropertyName = "target_code")]
    public required string TargetCode { get; init; }
    [JsonProperty(PropertyName = "conversion_rate")]
    public required decimal ConversionRate { get; init; }

    [JsonProperty(PropertyName = "conversion_result")]
    public required decimal ConversionResult { get; init; }
}