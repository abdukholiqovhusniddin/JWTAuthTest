namespace Application.Commons;
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; } = 200;
    public ApiResponse() { }
    public ApiResponse(T data) {     
        Data = data;
    }
    public ApiResponse(string message, int statusCode = 400)
    {
        StatusCode = statusCode;
    }
}