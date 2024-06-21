using MassTransit;
using Order.Outbox.Table.Publisher.Entities;
using Quartz;
using Shared.Events;
using System.Text.Json;

namespace Order.Outbox.Table.Publisher.Jobs
{
    public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderOutboxSingletonDatabase.DataReaderState) 
            {
                OrderOutboxSingletonDatabase.DataReaderBusy();

                List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM OrderOutboxes WHERE PROCESSEDDATE IS NULL ORDER BY OCCUREDON ASC")).ToList();

                foreach(var orderOutbox in orderOutboxes)
                {
                    if(orderOutbox.Type == nameof(OrderCreatedEvent))
                    {
                        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);

                        if(orderCreatedEvent is not null)
                        {
                            await publishEndpoint.Publish(orderCreatedEvent);
                            await OrderOutboxSingletonDatabase
                                .ExecuteAsync($"UPDATE OrderOutboxes SET PROCESSEDDATE= GETDATE() WHERE ID ='{orderOutbox.Id}'");
                        }
                    }
                }

                OrderOutboxSingletonDatabase.DataReaderReady();
                Console.WriteLine("Order outbox table checked!");
            }
        }
    }
}
