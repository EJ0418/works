using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using Swashbuckle.AspNetCore.Annotations;

namespace YourApp.Controllers;

[ApiController]
[Route("api/mqtt")]
public class MqttController : ControllerBase
{
    private readonly MqttService _mqtt;

    public MqttController(MqttService mqtt)
    {
        _mqtt = mqtt;
    }

    [HttpPost("subscribe")]
    [SwaggerOperation(Summary = "訂閱主題", Description = "訂閱MQTT主題")]
    public async Task<IActionResult> Subscribe([SwaggerParameter("主題內容")] string topic)
    {
        await _mqtt.SubscribeTopicAsync(topic);
        return Ok($"訂閱主題：{topic}");
    }

    [HttpPost("unsubscribe")]
    [SwaggerOperation(Summary = "取消訂閱主題", Description = "取消訂閱MQTT主題")]
    public async Task<IActionResult> UnSubscribe([SwaggerParameter("主題內容")] string topic)
    {
        await _mqtt.UnSubscribeTopicAsync(topic);
        return Ok($"取消訂閱主題：{topic}");
    }

    [HttpPost("publish")]
    [SwaggerOperation(Summary = "發布", Description = "發布MQTT主題及訊息內容")]
    public async Task<IActionResult> Publish([FromBody, SwaggerParameter("主題及訊息內容")] PublishRequest dto)
    {
        await _mqtt.PublishAsync(dto.Topic, dto.Message);
        return Ok("訊息已發送");
    }

    [HttpGet("messages")]
    [SwaggerOperation(Summary = "取得訊息", Description = "取得訂閱的訊息")]
    public IActionResult GetReceivedMessages()
    {
        // 檢查訊息內容是否為空
        if (_mqtt.ReceivedMessages == null || _mqtt.ReceivedMessages.Count == 0)
        {
            return Ok(new { message = "目前沒有收到任何訊息", data = new List<object>() });
        }

        // 將訊息轉為可序列化的物件
        var serializableMessages = _mqtt.ReceivedMessages.Select(msg => new
        {
            msg.Topic,
            msg.Payload,
        });

        return Ok(serializableMessages);
    }

}
