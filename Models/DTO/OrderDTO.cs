namespace CarRental.Models.DTO
{
    public class OrderDTO
    {
        public DateTime OrderDate { get; set; }
        public string DriverName { get; set; }
        public string DrivingLicense { get; set; }
        public string BankCardNumber { get; set; }
        public string BankCardValidity { get; set; }
        public int BankCardCVC { get; set; }
        public int CarId { get; set; }
        public int DepartureAreaId { get; set; }
        public int DestinationAreaId { get; set; }
        public int Kilometrage { get; set; }
    }
}
