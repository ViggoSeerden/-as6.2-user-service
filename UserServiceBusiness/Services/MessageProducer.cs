using System.Text;
using RabbitMQ.Client;

namespace UserServiceBusiness.Services;

public class MessageProducer
{
    private readonly IModel _channel;

    public MessageProducer()
    {
        var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("RabbitMQ") ?? "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.QueueDeclare(queue: "send-email",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    public void SendMessage(string content)
    {
        var body = Encoding.UTF8.GetBytes(content);

        _channel.BasicPublish(exchange: string.Empty,
            routingKey: "send-email",
            basicProperties: null,
            body: body);
        Console.WriteLine($" [x] Sent {content}");
    }
}