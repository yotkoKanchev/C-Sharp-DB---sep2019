namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(p => ids.Contains(p.Id))
                .OrderBy(p => p.FullName)
                .ThenBy(p => p.Id)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                        .Select(o => new
                        {
                            OfficerName = o.Officer.FullName,
                            Department = o.Officer.Department.Name,
                        })
                        .OrderBy(o => o.OfficerName)
                        .ToList(),
                    TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Sum(po => po.Officer.Salary), 2)
                })
                .ToList();

            return JsonConvert.SerializeObject(prisoners, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var prisoners = context.Prisoners
                .Where(p => names.Contains(p.FullName))
                .OrderBy(p => p.FullName)
                .ThenBy(p => p.Id)
                .Select(p => new ExportPrisonerDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails.Select(m => new ExportMessageDto
                    {
                        Description =  ReverseString(m.Description)
                    }).ToArray()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportPrisonerDto[]), new XmlRootAttribute("Prisoners"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, prisoners, ns);
            }

            return sb.ToString().TrimEnd();
        }

        private static string ReverseString(string description)
        {
            var reversed = string.Empty;

            for (int i = description.Length - 1; i >= 0; i--)
            {
                reversed += description[i];
            }

            return reversed;
        }
    }
}