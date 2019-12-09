namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Data;
    using ExportDto;

    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projectDtos = context.Projects
                .Where(p => p.Tasks.Any())
                .OrderByDescending(p => p.Tasks.Count)
                .ThenBy(p => p.Name)
                .Select(p => new ExportProjectDto
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate == null ? "No" : "Yes",
                    Tasks = p.Tasks
                            .Select(t => new ExportTaskDto
                            {
                                Name = t.Name,
                                Label = t.LabelType.ToString()
                            })
                            .OrderBy(t => t.Name)
                            .ToArray()
                })
                .ToArray();

            var stringBuilder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ExportProjectDto[]), new XmlRootAttribute("Projects"));
            var nameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(stringBuilder))
            {
                serializer.Serialize(writer, projectDtos, nameSpaces);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employeeDtos = context.Employees
                .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .OrderByDescending(e => e.EmployeesTasks.Count(et => et.Task.OpenDate >= date))
                .ThenBy(e => e.Username)
                .Take(10)
                .Select(e => new ExportEmployeeDto
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                        .Where(et => et.Task.OpenDate >= date)
                        .OrderByDescending(et => et.Task.DueDate)
                        .ThenBy(et => et.Task.Name)
                        .Select(et => new ExportTaskEmployeeDto
                        {
                            TaskName = et.Task.Name,
                            ExecutionType = et.Task.ExecutionType.ToString(),
                            LabelType = et.Task.LabelType.ToString(),
                            OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                            DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        })

                        .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(employeeDtos, Newtonsoft.Json.Formatting.Indented);
        }
    }
}