namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

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

            return JsonConvert.SerializeObject(prisoners, Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            return null;

        }
    }
}