namespace SoftJail.DataProcessor
{

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;
    using Data.Models;
    using DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var departments = new List<Department>();
            var importDepartmentDtos = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);

            foreach (var departmentDto in importDepartmentDtos)
            {
                var cellsAreValid = departmentDto.Cells.All(c => IsValid(c));

                if (!cellsAreValid || !IsValid(departmentDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var cells = new List<Cell>();

                foreach (var cellDto in departmentDto.Cells)
                {
                    cells.Add(new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow,
                    });
                }

                var department = new Department
                {
                    Name = departmentDto.Name,
                    Cells = cells,
                };

                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
                departments.Add(department);
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            return null;

        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
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