namespace FastFood.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using FastFood.Data;
    using FastFood.DataProcessor.Dto.Import;
    using FastFood.Models;
    using FastFood.Models.Enums;
    using Newtonsoft.Json;

    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var importEmployeeDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            var employees = new List<Employee>();
            var positions = new HashSet<Position>();

            foreach (var employeeDto in importEmployeeDtos)
            {
                var employeeDtoIsValid = IsValid(employeeDto);

                if (!employeeDtoIsValid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var position = positions.Any(p => p.Name == employeeDto.Position)
                    ? positions.First(p => p.Name == employeeDto.Position)
                    : new Position { Name = employeeDto.Position };

                positions.Add(position);

                var employee = new Employee
                {
                    Name = employeeDto.Name,
                    Age = employeeDto.Age,
                    Position = position
                };

                sb.AppendLine(String.Format(SuccessMessage, employee.Name));
                employees.Add(employee);
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var items = new List<Item>();
            var categories = new HashSet<Category>();

            var importItemsDto = JsonConvert.DeserializeObject<ImportItemDto[]>(jsonString);

            foreach (var itemDto in importItemsDto)
            {
                if (!IsValid(itemDto) || items.Any(i => i.Name == itemDto.Name))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var category = categories.Any(c => c.Name == itemDto.Category)
                    ? categories.First(c => c.Name == itemDto.Category)
                    : new Category { Name = itemDto.Category };

                categories.Add(category);

                var item = new Item
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Category = category,
                };

                sb.AppendLine(String.Format(SuccessMessage, item.Name));
                items.Add(item);
            }

            context.Items.AddRange(items);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var employees = context.Employees.ToList();
            var employeeNames = employees.Select(e => e.Name).ToList();
            var items = context.Items.ToList();
            var itemNames = items.Select(i => i.Name).ToList();

            var orders = new List<Order>();
            var orderItems = new List<OrderItem>();

            var serializer = new XmlSerializer(typeof(ImportOrderDto[]), new XmlRootAttribute("Orders"));
            ImportOrderDto[] importOrderDtos;

            using (var reader = new StringReader(xmlString))
            {
                importOrderDtos = (ImportOrderDto[])serializer.Deserialize(reader);
            }

            foreach (var orderDto in importOrderDtos)
            {
                var itemsExists = orderDto.Items.Select(i => i.Name).ToList().All(n => itemNames.Contains(n));

                if (!IsValid(orderDto) ||
                    !employeeNames.Contains(orderDto.Employee) ||
                    !itemsExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var order = new Order
                {
                    Customer = orderDto.Customer,
                    DateTime = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Employee = employees.First(e => e.Name == orderDto.Employee),
                    Type = (OrderType)Enum.Parse(typeof(OrderType), orderDto.Type),
                };

                foreach (var item in orderDto.Items)
                {
                    orderItems.Add(new OrderItem
                    {
                        Order = order,
                        Item = items.First(i => i.Name == item.Name),
                        Quantity = item.Quantity,
                    });
                }

                sb.AppendLine($"Order for {order.Customer} on {order.DateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} added");
                orders.Add(order);
            }

            context.Orders.AddRange(orders);
            context.OrderItems.AddRange(orderItems);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}