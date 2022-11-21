namespace CarRental.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public Driver Driver { get; set; }
        public Car Car { get; set; }
        public Path Path { get; set; }
    }
}
