namespace Agile3.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeGiven { get; set; }
        public DateTime OrderDate { get; set; }

        public List<OrderItem> Items { get; set; }
    }
}
