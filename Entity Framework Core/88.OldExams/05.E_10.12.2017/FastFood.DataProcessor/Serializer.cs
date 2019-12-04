namespace FastFood.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using FastFood.Data;
    using FastFood.DataProcessor.Dto.Export;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var employee = context.Employees
                .Where(e => e.Name == employeeName)
                .Select(e => new
                {
                    Name = e.Name,
                    Orders = e.Orders.Where(o => o.Type.ToString() == orderType)
                        .Select(o => new
                        {
                            Customer = o.Customer,
                            Items = o.OrderItems.Select(oi => new
                            {
                                Name = oi.Item.Name,
                                Price = oi.Item.Price,
                                Quantity = oi.Quantity,
                            }),
                            TotalPrice = o.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity),
                        })
                        .OrderByDescending(o => o.TotalPrice)
                        .ThenByDescending(o => o.Items.Count())
                        .ToList(),
                    TotalMade = e.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity))
                }).First();


            return JsonConvert.SerializeObject(employee, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categories = categoriesString.Split(",", StringSplitOptions.RemoveEmptyEntries);

            var filteredCategories = context.Categories
                .Where(c => categories.Contains(c.Name))
                .Select(c => new ExportCategoryDto
                {
                    Name = c.Name,
                    MostPopularItem = c.Items.Select(i => new MostPopularItemDto
                    {
                        Name = i.Name,
                        TimesSold = i.OrderItems.Sum(oi => oi.Quantity),
                        TotalMade = i.OrderItems.Sum(oi => oi.Quantity * oi.Item.Price)
                    })
                    .OrderByDescending(i => i.TotalMade)
                    .First()
                })
                .OrderByDescending(o => o.MostPopularItem.TotalMade)
                .ThenByDescending(o => o.MostPopularItem.TimesSold)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportCategoryDto[]), new XmlRootAttribute("Categories"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, filteredCategories, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}