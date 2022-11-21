using CarRental.Models;
using CarRental.Models.DTO;
using CarRental.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly SqlConnection connection;
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
            connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection"));
        }
        public IActionResult Index(bool? wasRedirected)
        {
            ViewBag.CarViewModels = GetCarViewModelsFromDB();
            ViewBag.WasRedirected = wasRedirected;
            return View();
        }
        public IActionResult NearestCar(int id)
        {
            ViewBag.Areas = GetAreasFromDB();
            ViewBag.NearestCars = GetCarsByAreaId(id);
            return View(id);
        }
        public IActionResult BookCar(int id)
        {
            ViewBag.Areas = GetAreasFromDB();
            ViewBag.Car = GetCarByIdFromDB(id);
            return View(id);
        }
        [HttpPost]
        public IActionResult SubmitOrder(int carId, string driverName, string drivingLicense, string cardNumber, string validity, int cvc, int departureId, int destinationId, int kilometrage)
        {
            var order = new OrderDTO
            {
                OrderDate = DateTime.Now,
                DriverName = driverName,
                DrivingLicense = drivingLicense,
                BankCardNumber = cardNumber,
                BankCardValidity = validity,
                BankCardCVC = cvc,
                CarId = carId,
                DepartureAreaId = departureId,
                DestinationAreaId = destinationId,
                Kilometrage = kilometrage
            };
            InsertOrderDetailsToDB(order);
            return RedirectToAction("Index", new { wasRedirected = true });
        }
        private List<CarViewModel> GetCarViewModelsFromDB()
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT C.Brand \"Brand\", C.Model \"Model\", P.URL \"URL\" " +
                "FROM Cars C, Photos P " +
                "WHERE C.PhotoId = P.PhotoId;", connection);
            var cars = new List<CarViewModel>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                do
                {
                    foreach (DbDataRecord record in reader.Cast<DbDataRecord>())
                    {
                        cars.Add(new CarViewModel
                        {
                            Brand = (string)record["Brand"],
                            Model = (string)record["Model"],
                            PhotoURL = (string)record["URL"]
                        });
                    }
                } while (reader.Read());
                reader.Close();
            }
            connection.Close();
            return cars;
        }
        private List<Area> GetAreasFromDB()
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Areas;", connection);
            var areas = new List<Area>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                do
                {
                    foreach (DbDataRecord record in reader.Cast<DbDataRecord>())
                    {
                        areas.Add(new Area
                        {
                            AreaId = (int)record["AreaId"],
                            Name = (string)record["Name"],
                            City = (string)record["City"]
                        });
                    }
                } while (reader.Read());
                reader.Close();
            }
            connection.Close();
            return areas;
        }
        private List<Car> GetCarsByAreaId(int areaId)
        {
            connection.Open();
            SqlCommand command = new SqlCommand($"SELECT C.*, P.URL, A.Name, A.City " +
                $"FROM Cars C, Photos P, Areas A " +
                $"WHERE C.CurrentLocationAreaId = {areaId} AND C.IsBooked = 0 AND C.PhotoId = P.PhotoId AND C.CurrentLocationAreaId = A.AreaId; ", connection);
            var cars = new List<Car>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                do
                {
                    foreach (DbDataRecord record in reader.Cast<DbDataRecord>())
                    {
                        cars.Add(new Car
                        {
                            CarId = (int)record["CarId"],
                            Brand = (string)record["Brand"],
                            Model = (string)record["Model"],
                            Year = (int)record["Year"],
                            MaxSpeed = (int)record["MaxSpeed"],
                            Category = (string)record["Category"],
                            Price = (decimal)record["Price"],
                            IsBooked = (bool)record["IsBooked"],
                            Photo = new Photo { URL = (string)record["URL"] },
                            CurrentLocation = new Area { Name = (string)record["Name"], City = (string)record["City"] }
                        });
                    }
                } while (reader.Read());
                reader.Close();
            }
            connection.Close();
            return cars;
        }
        private Car GetCarByIdFromDB(int carId)
        {
            connection.Open();
            SqlCommand command = new SqlCommand($"SELECT C.*, P.URL, A.AreaId, A.Name, A.City " +
                $"FROM Cars C, Photos P, Areas A " +
                $"WHERE C.CarId = {carId} AND C.PhotoId = P.PhotoId AND C.CurrentLocationAreaId = A.AreaId; ", connection);
            Car car = new Car();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                do
                {
                    foreach (DbDataRecord record in reader.Cast<DbDataRecord>())
                    {
                        car = new Car()
                        {
                            CarId = (int)record["CarId"],
                            Brand = (string)record["Brand"],
                            Model = (string)record["Model"],
                            Year = (int)record["Year"],
                            MaxSpeed = (int)record["MaxSpeed"],
                            Category = (string)record["Category"],
                            Price = (decimal)record["Price"],
                            IsBooked = (bool)record["IsBooked"],
                            Photo = new Photo { URL = (string)record["URL"] },
                            CurrentLocation = new Area { AreaId = (int)record["AreaId"], Name = (string)record["Name"], City = (string)record["City"] }
                        };
                    }
                } while (reader.Read());
                reader.Close();
            }
            connection.Close();
            return car;
        }
        private void InsertOrderDetailsToDB(OrderDTO order)
        {
            connection.Open();
            var format = "yyyy-MM-dd HH:mm:ss:fff";
            var stringDate = order.OrderDate.ToString(format);
            SqlCommand command = new SqlCommand(
                $"INSERT INTO BankCards VALUES " +
                    $"('{ order.BankCardNumber }', '{ order.BankCardValidity }', { order.BankCardCVC }) " +
                $"INSERT INTO Drivers VALUES ('{ order.DriverName }', '{ order.DrivingLicense }', " +
                    $"(SELECT DISTINCT BankCardId FROM BankCards " +
                    $"WHERE Number = '{ order.BankCardNumber }' AND CVC = { order.BankCardCVC })) " +
                $"INSERT INTO Paths VALUES " +
                    $"({ order.Kilometrage }, { order.DepartureAreaId }, { order.DestinationAreaId }) " +
                $"INSERT INTO Orders VALUES " +
                    $"('{ stringDate }', " +
                    $"(SELECT DISTINCT DriverId FROM Drivers " +
                    $"WHERE Name = '{ order.DriverName }' AND DrivingLicense = '{ order.DrivingLicense }'), " +
                    $"{ order.CarId }, " +
                    $"(SELECT DISTINCT PathId FROM Paths " +
                    $"WHERE Kilometrage = { order.Kilometrage } AND DepartureAreaAreaId = { order.DepartureAreaId } " +
                    $"AND DestinationAreaAreaId = { order.DestinationAreaId })) " +
                $"UPDATE Cars SET IsBooked = 1 WHERE CarId = { order.CarId }; ", connection);
            int rows = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}