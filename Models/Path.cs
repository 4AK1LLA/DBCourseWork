namespace CarRental.Models
{
    public class Path
    {
        public int PathId { get; set; }
        public int Kilometrage { get; set; }
        public Area? DepartureArea { get; set; }
        public Area? DestinationArea { get; set; }
    }
}
