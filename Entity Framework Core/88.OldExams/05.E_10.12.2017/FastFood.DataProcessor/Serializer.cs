namespace FastFood.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using FastFood.Data;
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


            return JsonConvert.SerializeObject(employee, Formatting.Indented);

        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            return null;

        }
    }
}