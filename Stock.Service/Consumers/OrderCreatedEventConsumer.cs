using MassTransit;
using Shared.Events;
using System.Text.Json;

namespace Stock.Service.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine(JsonSerializer.Serialize(context.Message));
        }
    }
}
