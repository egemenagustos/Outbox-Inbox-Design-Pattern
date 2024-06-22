using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.Service.Models.Contexts;
using Stock.Service.Models.Entities;
using System.Text.Json;

namespace Stock.Service.Consumers
{
    public class OrderCreatedEventConsumer(StockDbContext stockDbContext) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            if (!await stockDbContext.OrderInboxes.AnyAsync(x => x.IdempotentToken == context.Message.IdempotentToken))
            {
                await stockDbContext.OrderInboxes.AddAsync(new()
                {
                    Processed = false,
                    Payload = JsonSerializer.Serialize(context.Message),
                    IdempotentToken = context.Message.IdempotentToken
                });

                await stockDbContext.SaveChangesAsync();
            }


            List<OrderInbox> orderInboxes = stockDbContext.OrderInboxes
                                                            .Where(x => !x.Processed)
                                                            .ToList();

            foreach (var orderInbox in orderInboxes)
            {
                OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload);

                Console.WriteLine($"{orderCreatedEvent.OrderId} - Stock processing of the order completed!");

                orderInbox.Processed = true;
                /* 
                SaveChanges metodunu forech dışında çağırmak burada çok doğru olmayacaktır.
                Sebebi ise döngüde 100 datada döndük 99 tanesi de işledik ama 100. datada hata aldık varsayalım.
                O zaman hangi data da hata olduğunu bulmamamız epey yorucu olabilir ve mantık olarak da
                çok doğru bir yaklaşım olmayacaktır.
                */
                await stockDbContext.SaveChangesAsync();
            }
        }
    }
}
