using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CurrencyService : BaseService, ICurrencyService
{
    private readonly IInDebtContext _context;
    private readonly IMapper _mapper;

    public CurrencyService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> GetAll(PaginationDto paginationDto)
    {
        var totalCount = await Context.Currencies.CountAsync(); 
        var currencies = await Context.Currencies
            .Skip(paginationDto.SkipCount())
            .Take(paginationDto.PageSize)
            .ProjectTo<CurrencyResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();
        var response = new PaginationResponse<CurrencyResponse>(totalCount, currencies);
        return Ok(response);
    }
}