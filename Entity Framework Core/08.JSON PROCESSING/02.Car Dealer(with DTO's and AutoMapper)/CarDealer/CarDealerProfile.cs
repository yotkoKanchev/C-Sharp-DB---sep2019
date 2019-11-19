namespace CarDealer
{
    using AutoMapper;
    using CarDealer.DTO;
    using CarDealer.Models;
    using System.Globalization;
    using System.Linq;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //14
            CreateMap<Customer, ExportCustomersDto>()
                .ForMember(x => x.BirthDate, y => y.MapFrom(x => x.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));

            //15 , 17, 19
            CreateMap<CarInfoDto, Car>();

            //16
            CreateMap<Supplier, ExportLocalSuppliersDto>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(x => x.Parts.Count));

            //17
            CreateMap<Part, PartInfoDto>()
                .ForMember(x => x.Price, y => y.MapFrom(x => $"{x.Price:f2}"));

            //17
            CreateMap<Car, ExportCarsWithParts>()
                .ForMember(x => x.car, y => y.MapFrom(x => x))
                .ForMember(x => x.parts, y => y.MapFrom(x => x.PartCars.Select(p => p.Part)));

            //18
            CreateMap<Customer, CustomerTotalSalesDto>()
                .ForMember(x => x.FullName, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.BoughtCars, y => y.MapFrom(x => x.Sales.Count))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(x => x.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))));

            //19
            CreateMap<Sale, CustomerInfoDto>()
                .ForMember(x => x.customerName, y => y.MapFrom(x => x.Customer.Name))
                .ForMember(x => x.Discount, y => y.MapFrom(x => $"{ x.Discount:f2}"))
                .ForMember(x => x.price, y => y.MapFrom(x => $"{x.Car.PartCars.Sum(pc => pc.Part.Price):f2}"))
                .ForMember(x => x.priceWithDiscount, y => y.MapFrom(x => $"{x.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - x.Discount / 100):f2}"));
        }
    }
}
