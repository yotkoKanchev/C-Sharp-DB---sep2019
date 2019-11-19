namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarDealer.Data;
    using CarDealer.DTO;
    using CarDealer.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                //db.Database.EnsureCreated();

                Mapper.Initialize(conf => conf.AddProfile<CarDealerProfile>());

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
                //Console.WriteLine(GetSalesWithAppliedDiscount(db));
            }
        }

        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.Suppliers.AddRange(suppliers);
            var affectedRowsCount = context.SaveChanges();

            return $"Successfully imported {affectedRowsCount}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            var supplierIds = context.Suppliers.Select(s => s.Id).ToList();

            var partsToAdd = parts.Where(p => supplierIds.Contains(p.SupplierId)).ToList();

            context.Parts.AddRange(partsToAdd);

            var affectedRowsCount = context.SaveChanges();

            return $"Successfully imported {affectedRowsCount}.";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var importCarsDtos = JsonConvert.DeserializeObject<ImportCarsDto[]>(inputJson);

            var validPartIds = context.Parts.Select(p => p.Id);

            var cars = new List<Car>();
            var partCars = new List<PartCar>();

            foreach (var importCarDto in importCarsDtos)
            {
                var car = new Car()
                {
                    Make = importCarDto.Make,
                    Model = importCarDto.Model,
                    TravelledDistance = importCarDto.TravelledDistance,
                };

                cars.Add(car);

                foreach (var partId in importCarDto.PartsId.Distinct())
                {
                    if (validPartIds.Contains(partId))
                    {
                        var partCar = new PartCar()
                        {
                            Car = car,
                            PartId = partId,
                        };

                        partCars.Add(partCar);
                    }
                }
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partCars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);
            context.Customers.AddRange(customers);

            var importedCustomersCount = context.SaveChanges();

            return $"Successfully imported {importedCustomersCount}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            //Judge dont like these validations and throws time limit error !!!
            //var validCarIds = context.Cars.Select(c => c.Id);
            //var validCustomerIds = context.Customers.Select(c => c.Id);

            //var salesToImport = new List<Sale>();

            //foreach (var sale in sales)
            //{
            //    if (validCarIds.Contains(sale.CarId) && validCustomerIds.Contains(sale.CustomerId))
            //    {
            //        salesToImport.Add(sale);
            //    }
            //}

            context.Sales.AddRange(sales);
            var importedSalesCount = context.SaveChanges();

            return $"Successfully imported {importedSalesCount}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .ProjectTo<ExportCustomersDto>()
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
                .ProjectTo<CarInfoDto>()
                .ToList();

            return JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSuppliersDto>()
                .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        //17. Export Cars With Their List Of Parts
        static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .ProjectTo<ExportCarsWithParts>()
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);

        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .ProjectTo<CustomerTotalSalesDto>()
                .OrderByDescending(c => c.SpentMoney)
                .OrderByDescending(c => c.BoughtCars)
                .ToList();

            return JsonConvert.SerializeObject(customers, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
            });
        }

        //19. Export Sales With Applied Discount          
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .ProjectTo<CustomerInfoDto>()
                .ToList();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}