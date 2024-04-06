using Domain.Enums;

namespace Application.Responses;

public class BaseResponse
{
    public BaseResponse(ResponseCode code = ResponseCode.Ok, string errorMessage = null)
    {
        Code = code;
        ErrorMessage = errorMessage;
    }

    public ResponseCode Code { get; set; }
    public bool Success => Code is ResponseCode.Ok or ResponseCode.Created;
    public string ErrorMessage { get; set; }
}

public class BaseResponse<T> : BaseResponse where T : class
{
    public BaseResponse(T data = null, ResponseCode code = ResponseCode.Ok, string errorMessage = null) : base(code, errorMessage)
    {
        Data = data;
    }

    public T Data { get; set; }
}