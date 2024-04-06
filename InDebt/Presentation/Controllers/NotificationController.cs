using Application.DTOs;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[Authorize]
public class NotificationController : BaseController
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<NotificationResponse>>> GetAllAsync([FromQuery] PaginationDto paginationDto)
    {
        var response = await _notificationService.GetAllUnreadAsync(paginationDto);
        return HandleRequest(response);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<BaseResponse>> MarkAsReadAsync([FromRoute]Guid id)
    {
        var response = await _notificationService.MarkAsReadAsync(id);
        return HandleRequest(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse>> DeleteAsync([FromRoute]Guid id)
    {
        var response = await _notificationService.DeleteAsync(id);
        return HandleRequest(response);
    }
}