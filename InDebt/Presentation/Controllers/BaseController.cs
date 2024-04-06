using Application.Responses;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InDebt.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected ActionResult HandleRequest(BaseResponse response)
    {
        try
        {
            ActionResult status = response.Code switch
            {
                ResponseCode.Created => new ObjectResult(response) { StatusCode = (int)HttpStatusCode.Created },
                ResponseCode.NotFound => NotFound(response.ErrorMessage),
                ResponseCode.Forbidden => Forbid(response.ErrorMessage),
                ResponseCode.BadRequest => BadRequest(response.ErrorMessage),
                ResponseCode.UnAuthorize => Unauthorized(),
                ResponseCode.Ok => Ok(response),
                ResponseCode.NoContent => NoContent(),
                _ => throw new ArgumentOutOfRangeException(nameof(response.Code))
            };
            return status;
        }
        catch (Exception e)
        {
            return new ObjectResult(new BaseResponse
            {
                ErrorMessage = e.Message,
                Code = ResponseCode.InternalServerError
            })
            {
                StatusCode = (int)ResponseCode.InternalServerError
            };
        }
    }
}