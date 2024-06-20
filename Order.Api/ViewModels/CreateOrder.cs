namespace Order.Api.ViewModels
{
    public class CreateOrder
    {
        public int BuyerId { get; set; }
        public List<CreateOrderItem> OrderItems { get; set; }
    }
}
