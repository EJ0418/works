using Swashbuckle.AspNetCore.Annotations;

public class SendToRoomRequest
    {
        [SwaggerSchema("聊天室ID")]
        public string Room { get; set; } = string.Empty;

        [SwaggerSchema("使用者名稱")]
        public string User { get; set; } = string.Empty;

        [SwaggerSchema("聊天訊息")]
        public string Message { get; set; } = string.Empty;
    }