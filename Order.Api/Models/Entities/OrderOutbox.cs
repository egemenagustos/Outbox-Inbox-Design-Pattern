namespace Order.Api.Models.Entities
{
    public class OrderOutbox
    {
        public int Id { get; set; }

        public DateTime OccuredOn { get; set; }

        public DateTime? ProcessedDate { get; set; }

        //Hangi event olacak...
        public string Type { get; set; }

        //Gönderilecek event bilgileri...
        public string Payload { get; set; }
    }
}
