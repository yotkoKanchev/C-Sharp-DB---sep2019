namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using CarDealer.Data;
    using CarDealer.DTO;
    using CarDealer.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                //db.Database.EnsureCreated();

                //var suppliers = File.ReadAllText("./../../../Datasets/suppliers.json");
                //Console.WriteLine(ImportSuppliers(db, suppliers));

                //var parts = File.ReadAllText("./../../../Datasets/parts.json");
                //Console.WriteLine(ImportParts(db, parts));

                //var cars = File.ReadAllText("./../../../Datasets/cars.json");
                //Console.WriteLine(ImportCars(db, cars));

                //var customers = File.ReadAllText("./../../../Datasets/customers.json");
                //Console.WriteLine(ImportCustomers(db, customers));

                //var sales = File.ReadAllText("./../../../Datasets/sales.json");
                //Console.WriteLine(ImportSales(db, sales));

                //Console.WriteLine(GetOrderedCustomers(db));
                //Console.WriteLine(GetCarsFromMakeToyota(db));
                //Console.WriteLine(GetLocalSuppliers(db));
                //Console.WriteLine(GetCarsWithTheirListOfParts(db));
                //Console.WriteLine(GetTotalSalesByCustomer(db));
                Console.WriteLine(GetSalesWithAppliedDiscount(db));
            }
        }

        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            var importedSuppliersCount = context.SaveChanges();

            return $"Successfully imported {importedSuppliersCount}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson);
            var existingSupplierIds = context.Suppliers.Select(s => s.Id).ToList();

            var validParts = parts.Where(p => existingSupplierIds.Contains(p.SupplierId)).ToList();

            context.Parts.AddRange(validParts);
            var importedPartsCount = context.SaveChanges();

            return $"Successfully imported {importedPartsCount}.";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var partCarDtos = JsonConvert.DeserializeObject<List<ImportPartCarDto>>(inputJson);

            var cars = new List<Car>();
            var partCars = new List<PartCar>();

            foreach (var partCarDto in partCarDtos)
            {
                var car = new Car()
                {
                    Make = partCarDto.Make,
                    Model = partCarDto.Model,
                    TravelledDistance = partCarDto.TravelledDistance
                };

                foreach (var partId in partCarDto.PartsId.Distinct())
                {
                    var partCar = new PartCar()
                    {
                        PartId = partId,
                        Car = car,
                    };

                    partCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partCars);
            context.SaveChanges();

            return $"Successfully imported {partCarDtos.Count}.";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    c.IsYoungDriver,
                })
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //15. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                })
                .ToList();

            return JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count,
                })
                .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance,
                    },
                    parts = c.PartCars
                        .Select(pc => new
                        {
                            Name = pc.Part.Name,
                            Price = $"{pc.Part.Price:f2}",
                        })
                        .ToList()
                })
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .ToList()
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars);

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance,
                    },
                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):f2}",
                    priceWithDiscount = $"{s.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100):f2}",
                })
                .ToList();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}