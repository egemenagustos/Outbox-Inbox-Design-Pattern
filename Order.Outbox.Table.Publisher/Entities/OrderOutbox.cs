using System.ComponentModel.DataAnnotations;

namespace Order.Outbox.Table.Publisher.Entities
{
    public class OrderOutbox
    {
        [Key]
        public Guid IdempotentToken { get; set; }

        public DateTime OccuredOn { get; set; }

        public DateTime? ProcessedDate { get; set; }

        //Hangi event olacak...
        public string Type { get; set; }

        //Gönderilecek event bilgileri...
        public string Payload { get; set; }
    }
}
