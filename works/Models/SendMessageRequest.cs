using Swashbuckle.AspNetCore.Annotations;

public class SendMessageRequest
{
    [SwaggerSchema("使用者名稱")]
    public string User { get; set; } = string.Empty;

    [SwaggerSchema("聊天訊息")]
    public string Message { get; set; } = string.Empty;
}