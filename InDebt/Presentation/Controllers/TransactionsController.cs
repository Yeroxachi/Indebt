using Application.DTOs;
using Application.Responses;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[Authorize]
public class TransactionsController : BaseController
{
    private readonly ITransactionService _management;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService management, ILogger<TransactionsController> logger)
    {
        _management = management;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginationResponse<TransactionResponse>>>> GetAllUserTransactions([FromQuery]Guid? debtId, [FromQuery]TransactionType? transactionType, [FromQuery] PaginationDto dto)
    {
        var response = await _management.GetAll(debtId, transactionType, dto);
        return HandleRequest(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<TransactionResponse>>> GetById([FromRoute]Guid id)
    {
        var response = await _management.GetById(id);
        return HandleRequest(response);
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TransactionResponse>>> CreateTransaction([FromBody]TransactionDto dto)
    {
        var response = await _management.Create(dto);
        return HandleRequest(response);
    }

    [HttpPut("{id}/accept")]
    public async Task<ActionResult<BaseResponse<TransactionResponse>>> AcceptTransaction([FromRoute]Guid id)
    {
        var response = await _management.Accept(id);
        return HandleRequest(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BaseResponse<TransactionResponse>>> DeleteTransaction([FromRoute]Guid id)
    {
        var response = await _management.Delete(id);
        return HandleRequest(response);
    }
}