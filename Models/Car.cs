using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int MaxSpeed { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
        public Photo Photo { get; set; }
        public Area CurrentLocation { get; set; }
    }
}
