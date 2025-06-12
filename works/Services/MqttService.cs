using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using System.Text;

public class MqttService
{
    public List<(string Topic, string Payload)> ReceivedMessages { get; set; } = new();

    private IServiceScopeFactory _scopeFactory;
    private IMqttClient _client;
    private RedisService _redis;
    public MqttService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        var mqttHost = Environment.GetEnvironmentVariable("MQTT_HOST");
        var mqttPort = int.Parse(Environment.GetEnvironmentVariable("MQTT_PORT"));
        var mqttUser = Environment.GetEnvironmentVariable("MQTT_USER");
        var mqttPw = Environment.GetEnvironmentVariable("MQTT_PASSWORD");

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttHost, mqttPort)
            .WithCredentials(mqttUser, mqttPw)
            .Build();

        _client.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var topic = e.ApplicationMessage.Topic ?? "It's Empty";
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment) ?? "It's Empty";

                using var scope = _scopeFactory.CreateScope();
                var redis = scope.ServiceProvider.GetRequiredService<RedisService>();
                await redis.SaveMessageAsync(topic, payload);

                ReceivedMessages.Add((topic, payload));
                Console.WriteLine($"[MQTT] 收到訊息：{topic} - {payload}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] 儲存錯誤：{ex.Message}");
            }
        };

        _client.ConnectAsync(options).Wait();
    }
    public async Task SubscribeTopicAsync(string topic)
    {
        await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(topic)
            .Build());
    }
    public async Task SubscribeShareTopicAsync(string group, string topic)
    {
        var sharedTopic = $"$share/{group}/{topic}";
        await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(sharedTopic)
            .Build());
    }

    public async Task UnSubscribeTopicAsync(string topic)
    {
        await _client.UnsubscribeAsync(new MqttClientUnsubscribeOptionsBuilder()
        .WithTopicFilter(topic)
        .Build());
    }

    public async Task PublishAsync(string topic, string message)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _client.PublishAsync(mqttMessage);
    }
}
