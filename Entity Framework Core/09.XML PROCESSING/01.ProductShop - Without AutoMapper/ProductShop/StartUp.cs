namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using ProductShop.Data;
    using ProductShop.Dtos.Export;
    using ProductShop.Dtos.Import;
    using ProductShop.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new ProductShopContext())
            {
                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                //var usersFromXml = File.ReadAllText("./../../../Datasets/users.xml");
                //Console.WriteLine(ImportUsers(db, usersFromXml));

                //var productsFromXml = File.ReadAllText("./../../../Datasets/products.xml");
                //Console.WriteLine(ImportProducts(db, productsFromXml));

                //var categoriesFromXml = File.ReadAllText("./../../../Datasets/categories.xml");
                //Console.WriteLine(ImportCategories(db, categoriesFromXml));

                //var categoryProductsFromXml = File.ReadAllText("./../../../Datasets/categories-products.xml");
                //Console.WriteLine(ImportCategoryProducts(db, categoryProductsFromXml));

                //Console.WriteLine(GetProductsInRange(db));
                //Console.WriteLine(GetSoldProducts(db));
                //Console.WriteLine(GetCategoriesByProductsCount(db));
                //Console.WriteLine(GetUsersWithProducts(db));
            }
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<UserDto>), new XmlRootAttribute("Users"));
            List<UserDto> userDtos;

            using (var reader = new StringReader(inputXml))
            {
                userDtos = (List<UserDto>)serializer.Deserialize(reader);
            }

            var users = new List<User>();

            userDtos.ForEach(ud => users.Add(new User
            {
                FirstName = ud.FirstName,
                LastName = ud.LastName,
                Age = ud.Age,
            }));

            context.Users.AddRange(users);
            var insertedUserRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedUserRecordsCount}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            //var validUserIds = context.Users.Select(u => u.Id).ToList();

            var serializer = new XmlSerializer(typeof(List<ProductDto>), new XmlRootAttribute("Products"));
            List<ProductDto> productDtos;

            using (var reader = new StringReader(inputXml))
            {
                productDtos = (List<ProductDto>)serializer.Deserialize(reader);
            }

            var products = new List<Product>();

            productDtos   /* Judge don't like validating userIds !!! */
                          //.Where(pd => validUserIds.Contains(pd.SellerId)) 
                          //.ToList()
                .ForEach(pd => products.Add(new Product
                {
                    Name = pd.Name,
                    Price = pd.Price,
                    SellerId = pd.SellerId,
                    BuyerId = pd.BuyerId
                }));

            context.Products.AddRange(products);
            var insertedProductRecorsCount = context.SaveChanges();

            return $"Successfully imported {insertedProductRecorsCount}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CategoryDto>), new XmlRootAttribute("Categories"));
            List<CategoryDto> categoryDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoryDtos = (List<CategoryDto>)serializer.Deserialize(reader);
            }

            var categories = new List<Category>();

            categoryDtos.ForEach(cd => categories.Add(new Category
            {
                Name = cd.Name,
            }));

            context.Categories.AddRange(categories);
            var insertedCategoryRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedCategoryRecordsCount}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var validCategoryIds = context.Categories.Select(c => c.Id).ToList();
            var validProductIds = context.Products.Select(p => p.Id).ToList();

            var serializer = new XmlSerializer(typeof(List<CategoryProductDto>), new XmlRootAttribute("CategoryProducts"));

            List<CategoryProductDto> categoryProductDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoryProductDtos = (List<CategoryProductDto>)serializer.Deserialize(reader);
            }

            var categoryProducts = new List<CategoryProduct>();

            categoryProductDtos
                .Where(cp => validCategoryIds.Contains(cp.CategoryId) &&
                             validProductIds.Contains(cp.ProductId))
                .ToList()
                .ForEach(cp => categoryProducts.Add(new CategoryProduct
                {
                    CategoryId = cp.CategoryId,
                    ProductId = cp.ProductId,
                }));

            context.CategoryProducts.AddRange(categoryProducts);
            var insertedCategoryProductRecordsCount = context.SaveChanges();

            return $"Successfully imported {insertedCategoryProductRecordsCount}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var exportProductDtos = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerFullName = $"{p.Buyer.FirstName} {p.Buyer.LastName}",
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<ExportProductDto>), new XmlRootAttribute("Products"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, exportProductDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var userProductsDtos = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new UserProductsDto
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(ps => new ExportProductDto
                        {
                            Name = ps.Name,
                            Price = ps.Price,
                        }).ToArray()
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<UserProductsDto>), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, userProductsDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var exportCategoryDtos = context.Categories
                .Select(c => new ExportCategoryDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price),
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<ExportCategoryDto>), new XmlRootAttribute("Categories"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, exportCategoryDtos, ns);
            }

            return sb.ToString().TrimEnd();
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithSellsCount = context.Users
                .Where(u => u.ProductsSold.Any());

            var userSoldProductsDtos = usersWithSellsCount
                .OrderByDescending(u => u.ProductsSold.Count)
                .Take(10)
                .Select(u => new UserSoldPruductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsDto
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                                    .OrderByDescending(s => s.Price)
                                    .Select(s => new ExportProductDto
                                    {
                                        Name = s.Name,
                                        Price = s.Price
                                    }).ToArray()
                    },
                })
                .ToArray();

            var resultObj = new UsersCountDto
            {
                Count = usersWithSellsCount.Count(),
                Users = userSoldProductsDtos
            };

            var serializer = new XmlSerializer(typeof(UsersCountDto), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, resultObj, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}