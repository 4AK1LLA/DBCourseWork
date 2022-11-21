namespace CarRental.Models
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string Name { get; set; }
        public string DrivingLicense { get; set; }
        public BankCard BankCard { get; set; }
    }
}
