using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using System.Text;

public class MqttService
{
    private IMqttClient _client;
    public MqttService()
    {
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

        _client.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            return Task.CompletedTask;
        };

        _client.ConnectAsync(options).Wait();
    }
    public async Task SubscribeTopicAsync(string topic)
    {
        await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
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
