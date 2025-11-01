using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;

public class RabbitMQReceiver : IRabbitMQReceiver
{
    public async Task<string> ReceivePayload()
    {
        var factory = new ConnectionFactory() 
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOSTNAME") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ__PORT"), out var port) ? port : 5672,
            UserName = Environment.GetEnvironmentVariable("RABBITMQ__USERNAME") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ__PASSWORD") ?? "guest"
        };
         
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();


        const string queueName = "server_statistics_queue";

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        var tcs = new TaskCompletionSource<string>();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());;
            tcs.TrySetResult(message);
            await Task.CompletedTask;
           await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };
        await channel.BasicConsumeAsync(queue: queueName,
            autoAck: false,
            consumer: consumer);

        var result = await tcs.Task;

       
        await channel.CloseAsync();
        await connection.CloseAsync();
        return result;
    }
 
}