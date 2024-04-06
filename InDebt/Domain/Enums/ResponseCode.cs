namespace Domain.Enums;

public enum ResponseCode
{
    Ok = 200,
    Created = 201,
    NoContent = 204,
    BadRequest = 400,
    UnAuthorize = 401,
    Forbidden = 403,
    NotFound = 404,
    InternalServerError = 500
}