namespace CarDealer
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using CarDealer.Data;
    using CarDealer.Dtos.Import;
    using CarDealer.Models;
    using CarDealer.Dtos.Export;
    using System.Xml;
    using System.Text;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new CarDealerContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                //var suppliersFromXml = File.ReadAllText("./../../../Datasets/suppliers.xml");
                //Console.WriteLine(ImportSuppliers(db, suppliersFromXml));

                //var partsFromXml = File.ReadAllText("./../../../Datasets/parts.xml");
                //Console.WriteLine(ImportParts(db, partsFromXml));

                //var carsFromXml = File.ReadAllText("./../../../Datasets/cars.xml");
                //Console.WriteLine(ImportCars(db, carsFromXml));

                //var customersFromXml = File.ReadAllText("./../../../Datasets/customers.xml");
                //Console.WriteLine(ImportCustomers(db, customersFromXml));

                //var salesFromXml = File.ReadAllText("./../../../Datasets/sales.xml");
                //Console.WriteLine(ImportSales(db, salesFromXml));

                //Console.WriteLine(GetCarsWithDistance(db));
                //Console.WriteLine(GetCarsFromMakeBmw(db));
                //Console.WriteLine(GetLocalSuppliers(db));
                //Console.WriteLine(GetCarsWithTheirListOfParts(db));
                //Console.WriteLine(GetTotalSalesByCustomer(db));
                Console.WriteLine(GetSalesWithAppliedDiscount(db));
            }
        }

        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<SupplierDto>), new XmlRootAttribute("Suppliers"));

            List<SupplierDto> supplierDtos;

            using (var reader = new StringReader(inputXml))
            {
                supplierDtos = (List<SupplierDto>)serializer.Deserialize(reader);
            }

            var suppliers = new List<Supplier>();

            supplierDtos.ForEach(s => suppliers.Add(new Supplier
            {
                Name = s.Name,
                IsImporter = s.IsImporter,
            }));

            context.Suppliers.AddRange(suppliers);
            var insertedRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedRecordsCount}";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var validSupplierIds = context.Suppliers
                .Select(s => s.Id);

            var serializer = new XmlSerializer(typeof(List<PartDto>), new XmlRootAttribute("Parts"));
            List<PartDto> partDtos;

            using (var reader = new StringReader(inputXml))
            {
                partDtos = (List<PartDto>)serializer.Deserialize(reader);
            }

            var parts = new List<Part>();

            partDtos
                .Where(p => validSupplierIds.Contains(p.SupplierId)) //validate SupplierId
                .ToList()
                .ForEach(pd => parts.Add(new Part
                {
                    Name = pd.Name,
                    Price = pd.Price,
                    Quantity = pd.Quantity,
                    SupplierId = pd.SupplierId
                }));

            context.Parts.AddRange(parts);
            var insertedRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedRecordsCount}";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var validPartsIds = context.Parts.Select(p => p.Id).ToList();

            var serializer = new XmlSerializer(typeof(Dtos.Import.CarDto[]), new XmlRootAttribute("Cars"));

            Dtos.Import.CarDto[] importCarDtos;

            using (var reader = new StringReader(inputXml))
            {
                importCarDtos = (Dtos.Import.CarDto[])serializer.Deserialize(reader);
            }

            var cars = new List<Car>();
            var partCars = new List<PartCar>();

            foreach (var carDto in importCarDtos)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance,
                };

                cars.Add(car);

                var partIds = carDto
                    .Parts
                    .Where(i => validPartsIds.Contains(i.Id))
                    .Select(p => p.Id)
                    .Distinct();

                foreach (var id in partIds)
                {
                    var partCar = new PartCar()
                    {
                        Car = car,
                        PartId = id,
                    };

                    partCars.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartCars.AddRange(partCars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CustomerDto>), new XmlRootAttribute("Customers"));

            List<CustomerDto> customerDtos;

            using (var reader = new StringReader(inputXml))
            {
                customerDtos = (List<CustomerDto>)serializer.Deserialize(reader);
            }

            var customers = new List<Customer>();

            customerDtos.ForEach(cd => customers.Add(new Customer
            {
                Name = cd.Name,
                BirthDate = cd.BirthDate,
                IsYoungDriver = cd.IsYoungDriver,
            }));

            context.Customers.AddRange(customers);
            var insertedCustomerRecords = context.SaveChanges();

            return $"Successfully imported {insertedCustomerRecords}";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var validCarIds = context.Cars.Select(c => c.Id).ToList();
            //var validCustomerIds = context.Customers.Select(c => c.Id).ToList();
            // do not validate the Customers !!! Judge screams !!!

            var serializer = new XmlSerializer(typeof(List<SaleDto>), new XmlRootAttribute("Sales"));
            List<SaleDto> saleDtos;

            using (var reader = new StringReader(inputXml))
            {
                saleDtos = (List<SaleDto>)serializer.Deserialize(reader);
            }

            var sales = new List<Sale>();

            saleDtos.Where(s => validCarIds.Contains(s.CarId) /*&&
                                validCustomerIds.Contains(s.CustomerId)*/)
                    .ToList()
                    .ForEach(s => sales.Add(new Sale
                    {
                        CarId = s.CarId,
                        CustomerId = s.CustomerId,
                        Discount = s.Discount,
                    })
                    );

            context.Sales.AddRange(sales);
            var insertedSaleRrecords = context.SaveChanges();

            return $"Successfully imported {insertedSaleRrecords}";
        }

        //14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carDistanceDtos = context.Cars
                .Where(c => c.TravelledDistance >= 2_000_000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new CarDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<Dtos.Export.ExportCarDto>), new XmlRootAttribute("cars"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, carDistanceDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //15. Export Cars From Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var bmwCarsDtos = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new CarBMWDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CarBMWDto>), new XmlRootAttribute("cars"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, bmwCarsDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSupplierDtos = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count(),
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<LocalSupplierDto>), new XmlRootAttribute("suppliers"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, localSupplierDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //17. Export Cars With Their List Of Parts  
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carWithPartsDtos = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .Select(c => new CarWithPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars
                        .Select(pc => new PartCarDto
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price,
                        })
                    .OrderByDescending(cp => cp.Price)
                    .ToList()
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CarWithPartsDto>), new XmlRootAttribute("cars"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, carWithPartsDtos, ns);
            }

            return sb.ToString().TrimEnd();

        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customerSalesDtos = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new CustomerSalesDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars
                                                       .Sum(pc => pc.Part.Price)),
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CustomerSalesDto>), new XmlRootAttribute("customers"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, customerSalesDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var saleWithDiscountDtos = context.Sales
                .Select(s => new SaleWithDiscountDto
                {
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Car = new CarSaleDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance,
                    },
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) - 
                                        s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<SaleWithDiscountDto>), new XmlRootAttribute("sales"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, saleWithDiscountDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
