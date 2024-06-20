using Quartz;

namespace Order.Outbox.Table.Publisher.Jobs
{
    public class OrderOutboxPublishJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("sa");
        }
    }
}
