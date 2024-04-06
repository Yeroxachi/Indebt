using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InDebt.Controllers;

[Authorize]
public class RatingController : BaseController
{
    private readonly IRatingService _ratingService;
    private readonly ILogger<RatingController> _logger;

    public RatingController(IRatingService ratingService, ILogger<RatingController> logger)
    {
        _ratingService = ratingService;
        _logger = logger;
    }

    [HttpGet("Users/{userId:guid}")]
    public async Task<ActionResult<BaseResponse<RatingResponse>>> GetUserRatingById([FromRoute]Guid userId)
    {
        var response = await _ratingService.GetRatingOfUserByIdAsync(userId);
        return HandleRequest(response);
    }
    
    [HttpGet("Groups/{groupId:guid}")]
    public async Task<ActionResult<BaseResponse<IReadOnlyCollection<RatingResponse>>>> GetRatingOfUsersByGroupId([FromRoute]Guid groupId)
    {
        var response = await _ratingService.GetRatingOfUsersByGroupIdAsync(groupId);
        return HandleRequest(response);
    }
}