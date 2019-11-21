namespace ProductShop
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using ProductShop.Data;
    using ProductShop.Dtos.Export;
    using ProductShop.Dtos.Import;
    using ProductShop.Models;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(conf => conf.AddProfile<ProductShopProfile>());

            using (var db = new ProductShopContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                //var users = File.ReadAllText("./../../../Datasets/users.xml");
                //Console.WriteLine(ImportUsers(db, users));

                //var products = File.ReadAllText("./../../../Datasets/products.xml");
                //Console.WriteLine(ImportProducts(db, products));

                //var categories = File.ReadAllText("./../../../Datasets/categories.xml");
                //Console.WriteLine(ImportCategories(db, categories));

                //var categoryProducts = File.ReadAllText("./../../../Datasets/categories-products.xml");
                //Console.WriteLine(ImportCategoryProducts(db, categoryProducts));

                //Console.WriteLine(GetProductsInRange(db));
                //Console.WriteLine(GetSoldProducts(db));
                //Console.WriteLine(GetCategoriesByProductsCount(db));
                Console.WriteLine(GetUsersWithProducts(db));
            }
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            ImportUserDto[] importUserDtos;

            using (var reader = new StringReader(inputXml))
            {
                importUserDtos = (ImportUserDto[])serializer.Deserialize(reader);
            }

            var users = Mapper.Map<User[]>(importUserDtos);

            context.Users.AddRange(users);
            var insertedUsersRecorsCount = context.SaveChanges();

            return $"Successfully imported {insertedUsersRecorsCount}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            ImportProductDto[] importProductDtos;

            using (var reader = new StringReader(inputXml))
            {
                importProductDtos = (ImportProductDto[])serializer.Deserialize(reader);
            }

            var products = Mapper.Map<Product[]>(importProductDtos).ToArray();

            context.Products.AddRange(products);
            var insertedProductRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedProductRecordsCount}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));

            ImportCategoryDto[] importCategoryDtos;

            using (var reader = new StringReader(inputXml))
            {
                importCategoryDtos = (ImportCategoryDto[])serializer.Deserialize(reader);
            }

            var categories = Mapper.Map<Category[]>(importCategoryDtos).Where(c => c.Name != null).ToArray();

            context.Categories.AddRange(categories);
            var importedCategoryRecordsCount = context.SaveChanges();

            return $"Successfully imported {importedCategoryRecordsCount}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var validCategoryIds = context.Categories.Select(c => c.Id).ToArray();
            var validProductIds = context.Products.Select(p => p.Id).ToArray();

            var serializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));

            ImportCategoryProductDto[] importCategoryProductDtos;

            using (var reader = new StringReader(inputXml))
            {
                importCategoryProductDtos = (ImportCategoryProductDto[])serializer.Deserialize(reader);
            }

            var categoryProducts = Mapper.Map<CategoryProduct[]>(importCategoryProductDtos)
                .Where(cp => validCategoryIds.Contains(cp.CategoryId) &&
                             validProductIds.Contains(cp.ProductId))
                .ToArray();

            context.CategoryProducts.AddRange(categoryProducts);
            var insertedCategoryProductRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedCategoryProductRecordsCount}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var sb = new StringBuilder();

            var exportProductInRangeDtos = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductInRangeDto>()
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportProductInRangeDto[]), new XmlRootAttribute("Products"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, exportProductInRangeDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var sb = new StringBuilder();

            var exportUserSoldProductsDtos = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserSoldProductsDto>()
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportUserSoldProductsDto[]), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, exportUserSoldProductsDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var sb = new StringBuilder();

            var exportCategoryByProductsCountDtos = context.Categories
                .ProjectTo<ExportCategoryByProductsCountDto>()
                .OrderByDescending(ec => ec.Count)
                .ThenBy(ec => ec.TotalRevenue)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportCategoryByProductsCountDto[]), new XmlRootAttribute("Categories"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, exportCategoryByProductsCountDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any());

            var filteredUsers = users
            .ProjectTo<UserDetailsDto>()
            .OrderByDescending(u => u.SoldProducts.Count)
            .Take(10)
            .ToArray();

            var userInfoDto = Mapper.Map<UserInfoDto>(filteredUsers);

            userInfoDto.Count = users.Count();

            var serializer = new XmlSerializer(typeof(UserInfoDto), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, userInfoDto, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}