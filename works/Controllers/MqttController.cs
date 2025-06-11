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
    [SwaggerOperation(Summary ="訂閱主題", Description ="訂閱MQTT主題")]
    public async Task<IActionResult> Subscribe([SwaggerParameter("主題內容")] string topic)
    {
        await _mqtt.SubscribeTopicAsync(topic);
        return Ok($"訂閱主題：{topic}");
    }

    [HttpPost("publish")]
    [SwaggerOperation(Summary ="發布", Description ="發布MQTT主題及訊息內容")]
    public async Task<IActionResult> Publish([FromBody, SwaggerParameter("主題及訊息內容")] PublishRequest dto)
    {
        await _mqtt.PublishAsync(dto.Topic, dto.Message);
        return Ok("訊息已發送");
    }
}
