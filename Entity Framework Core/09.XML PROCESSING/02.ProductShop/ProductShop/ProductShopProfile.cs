namespace ProductShop
{
    using ProductShop.Dtos.Import;
    using ProductShop.Dtos.Export;
    using ProductShop.Models;

    using AutoMapper;
    using System.Linq;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //1
            CreateMap<ImportUserDto, User>();

            //2
            CreateMap<ImportProductDto, Product>();

            //3
            CreateMap<ImportCategoryDto, Category>();

            //4
            CreateMap<ImportCategoryProductDto, CategoryProduct>();

            //5
            CreateMap<Product, ExportProductInRangeDto>()
                .ForMember(x => x.Buyer, y => y.MapFrom(x => $"{x.Buyer.FirstName} {x.Buyer.LastName}"));

            //6
            CreateMap<User, ExportUserSoldProductsDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold));

            //7
            CreateMap<Category, ExportCategoryByProductsCountDto>()
                .ForMember(x => x.Count, y => y.MapFrom(x => x.CategoryProducts.Count))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(x => x.CategoryProducts.Select(cp => cp.Product.Price).Average()))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(x => x.CategoryProducts.Sum(cp => cp.Product.Price)));

            //8
            this.CreateMap<Product, ProductDto>();

            this.CreateMap<User, SoldProductsWithCountDto>()
                .ForMember(x => x.Count, y => y.MapFrom(x => x.ProductsSold.Count()))
                .ForMember(x => x.Products, y => y.MapFrom(x => x.ProductsSold.OrderByDescending(ps => ps.Price)));

            this.CreateMap<User, UserDetailsDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x));

            this.CreateMap<UserDetailsDto[], UserInfoDto>()
                .ForMember(x => x.Users, y => y.MapFrom(x => x));
        }
    }
}
