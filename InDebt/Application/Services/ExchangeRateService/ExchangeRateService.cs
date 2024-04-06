using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Application.Services;

public class ExchangeRateService : BaseService, IExchangeRateService
{
    private readonly IInDebtContext _context;
    private readonly IOptions<ExchangeRateServiceOptions> _options;
    private readonly HttpClient _client;

    public ExchangeRateService(
        IInDebtContext context, 
        IMapper mapper, 
        IHttpContextAccessor accessor,
        IHttpClientFactory clientFactory,
        IOptions<ExchangeRateServiceOptions> options) : base(context, mapper, accessor)
    {
        _context = context;
        _options = options;
        _client = clientFactory.CreateClient();
    }

    public async Task<BaseResponse<ExchangeRateResponse>> CalculateExchangeRateAsync(ExchangeRateDto dto)
    {
        var from = await _context.Currencies.FindAsync(dto.LeftCurrencyId);
        var to = await _context.Currencies.FindAsync(dto.RightCurrencyId);
        if (from is null || to is null)
        {
            return NotFound<ExchangeRateResponse>();
        }

        var pairConversionEndpoint = string.Format(CultureInfo.InvariantCulture, _options.Value.PairConversionEndpoint, from.CurrencyCode, to.CurrencyCode, dto.Amount);
        var pairConversionUri = _options.Value.Host + _options.Value.ApiKey + pairConversionEndpoint;
            
        var response = await _client.GetAsync(pairConversionUri);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var errorType = JObject.Parse(content)["error-type"]?.ToString();
            return BadRequest<ExchangeRateResponse>($"Error occurred while sending request to the server: {errorType}");
        }

        var exchangeRateResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(content);
        return Ok(exchangeRateResponse);
    }
}