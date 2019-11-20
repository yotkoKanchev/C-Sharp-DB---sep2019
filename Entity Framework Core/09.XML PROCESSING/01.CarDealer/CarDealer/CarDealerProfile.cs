namespace CarDealer
{
    using System.Linq;

    using Dtos.Export;
    using Dtos.Import;
    using Models;

    using AutoMapper;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //P9
            CreateMap<ImportSupplierDto, Supplier>();

            //p10
            CreateMap<ImportPartDto, Part>();

            //P11
            CreateMap<PartIdDto, Part>();
            CreateMap<ImportCarDto, Car>().ForMember(x => x.PartCars, y => y.MapFrom(x => x.Parts));

            //P12
            CreateMap<ImportCustomerDto, Customer>();

            //P13
            CreateMap<ImportSaleDto, Sale>();

            //P16
            CreateMap<Supplier, ExportSupllierDto>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(x => x.Parts.Count));

            //P17
            CreateMap<Part, ExportCarPartDto>();
            CreateMap<Car, ExportCarDto>()
                .ForMember(x => x.CarPartDtos, y => y.MapFrom(x => x.PartCars
                                                                    .Select(pc => pc.Part)
                                                                    .OrderByDescending(pc => pc.Price)));

            //P18
            CreateMap<Customer, ExportCustomerByTotalSalesDto>()
                .ForMember(x => x.FullName, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.BoughtCars, y => y.MapFrom(x => x.Sales.Count))
                .ForMember(x => x.SpentMoney,
                           y => y.MapFrom(x => x.Sales
                                                .Sum(s => s.Car.PartCars
                                                               .Sum(pc => pc.Part.Price))));

            //19
            //CreateMap<Car, CarSaleDiscountDto>();
            CreateMap<Sale, ExportSaleWithDiscontDto>()
                .ForMember(x => x.CustomerName, y => y.MapFrom(x => x.Customer.Name))
                .ForMember(x => x.Price, y => y.MapFrom(x => x.Car.PartCars
                                                              .Sum(pc => pc.Part.Price)))
                .ForMember(x => x.PriceWithDiscount, y => y.MapFrom(x => x.Car.PartCars
                                                              .Sum(pc => pc.Part.Price) -
                                                              (x.Car.PartCars.Sum(pc => pc.Part.Price) * x.Discount / 100)));
        }
    }
}
