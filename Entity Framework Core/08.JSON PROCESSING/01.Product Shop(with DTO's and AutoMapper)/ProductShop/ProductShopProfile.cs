using AutoMapper;
using ProductShop.DTO;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, ProductDto>()
                .ForMember(x => x.Seller, y => y.MapFrom(x => $"{x.Seller.FirstName} {x.Seller.LastName}"));

            this.CreateMap<Product, SoldProductsDto>()
                .ForMember(x => x.BuyerFirstName, y => y.MapFrom(x => x.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, y => y.MapFrom(x => x.Buyer.LastName));

            this.CreateMap<User, UsersSoldProductsDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x.ProductsSold));

            this.CreateMap<Category, CategoryByProductDto>()
                .ForMember(x => x.Category, y => y.MapFrom(x => x.Name))
                .ForMember(x => x.ProductsCount, y => y.MapFrom(x => x.CategoryProducts.Count))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(x => $"{x.CategoryProducts.Average(cp => cp.Product.Price):f2}"))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(x => $"{x.CategoryProducts.Sum(cp => cp.Product.Price):f2}"));

            this.CreateMap<Product, ProductDetailsDto>();
            this.CreateMap<User, SoldProductsWithCountDto>()
                .ForMember(x => x.Count, y => y.MapFrom(x => x.ProductsSold.Where(ps => ps.Buyer != null).Count()))
                .ForMember(x => x.Products, y => y.MapFrom(x => x.ProductsSold.Where(ps => ps.Buyer != null)));
            this.CreateMap<User, UserDetailsDto>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(x => x));
            this.CreateMap<UserDetailsDto[], UserInfoDto>()
                .ForMember(x => x.UsersCount, y => y.MapFrom(x => x.Length))
                .ForMember(x => x.Users, y => y.MapFrom(x => x));
        }
    }
}
