namespace FastFood.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using FastFood.Data;
    using FastFood.DataProcessor.Dto.Import;
    using FastFood.Models;
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
                var positionDtoIsValid = IsValid(employeeDto.Position);

                if (!employeeDtoIsValid || !positionDtoIsValid)
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
            return null;

        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            return null;

        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}