using MassTransit;
using Order.Outbox.Table.Publisher.Entities;
using Quartz;

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
            }
        }
    }
}
