using Swashbuckle.AspNetCore.Annotations;

public class PublishRequest
{
    [SwaggerSchema("主題")]
    public string Topic { get; set; }

    [SwaggerSchema("訊息")]
    public string Message { get; set; }
}