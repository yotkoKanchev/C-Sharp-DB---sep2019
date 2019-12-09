namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using DataProcessor.ImportDto;

    using Newtonsoft.Json;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var stringBuilder = new StringBuilder();
            var projects = new List<Project>();

            var serializer = new XmlSerializer(typeof(ImportProjectDto[]), new XmlRootAttribute("Projects"));

            ImportProjectDto[] importProjectDtos;

            using (var reader = new StringReader(xmlString))
            {
                importProjectDtos = (ImportProjectDto[])serializer.Deserialize(reader);
            }

            foreach (var importProjectDto in importProjectDtos)
            {
                if (!IsValid(importProjectDto) ||
                    importProjectDto.OpenDate == string.Empty || // TODO may don't need that one !
                    string.IsNullOrEmpty(importProjectDto.OpenDate) ||
                    string.IsNullOrWhiteSpace(importProjectDto.OpenDate))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? dueDate;

                if (!string.IsNullOrWhiteSpace(importProjectDto.DueDate) &&
                    !string.IsNullOrEmpty(importProjectDto.DueDate))
                {
                    dueDate = DateTime.ParseExact(importProjectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else 
                {
                    dueDate = null; // TODO may not need it !!!
                }

                var project = new Project
                {
                    Name = importProjectDto.Name,
                    OpenDate = DateTime.ParseExact(importProjectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DueDate = dueDate,
                };

                foreach (var taskDto in importProjectDto.Tasks)
                {
                    var taskIsValid = IsValid(taskDto);

                    // TODO may not need all validations here
                    var openDateIsValid = !string.IsNullOrWhiteSpace(taskDto.OpenDate) &&
                        !string.IsNullOrEmpty(taskDto.OpenDate) &&
                        DateTime.ParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) > project.OpenDate;

                    // TODO may not need all validations here
                    var dueDateIsValid = (!string.IsNullOrWhiteSpace(taskDto.DueDate) &&
                        !string.IsNullOrEmpty(taskDto.DueDate) &&
                        DateTime.ParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) < project.DueDate) ||
                        project.DueDate == null;

                    // TODO try without these two validations
                    var executionTypeIsValid = Enum.IsDefined(typeof(ExecutionType), taskDto.ExecutionType);
                    var labelTypeIsValid = Enum.IsDefined(typeof(LabelType), taskDto.LabelType);

                    if (!taskIsValid || !openDateIsValid || !dueDateIsValid || executionTypeIsValid || labelTypeIsValid)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    project.Tasks.Add(new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = DateTime.ParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType,
                    });
                }

                projects.Add(project);

                stringBuilder.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
                projects.Add(project);
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sitringBuilder = new StringBuilder();
            var tasks = context.Tasks.ToList();
            var validTaskIds = tasks.Select(t => t.Id);

            var employeeDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);
            var employees = new List<Employee>();

            foreach (var employeesDto in employeeDtos)
            {
                var currentTasks = employeesDto.Tasks.Distinct().ToList();

                if (!IsValid(employeesDto))
                {
                    sitringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee
                {
                    Username = employeesDto.Username,
                    Email = employeesDto.Email,
                    Phone = employeesDto.Phone,
                };

                foreach (var taskId in currentTasks)
                {
                    if (!validTaskIds.Contains(taskId))
                    {
                        sitringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask
                    {
                        Task = tasks.First(t => t.Id == taskId)
                    });
                }

                sitringBuilder.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
                employees.Add(employee);
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sitringBuilder.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}