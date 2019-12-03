namespace SoftJail.DataProcessor
{

    using System;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Globalization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    using Data;
    using Data.Models;
    using DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using SoftJail.Data.Models.Enums;

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
            var sb = new StringBuilder();
            var prisoners = new List<Prisoner>();

            var importPrisonerMailDtos = JsonConvert.DeserializeObject<ImportPrisonerMailDto[]>(jsonString);

            foreach (var prisonerDto in importPrisonerMailDtos)
            {
                var mailsAreValid = prisonerDto.Mails.All(IsValid);

                if (!IsValid(prisonerDto) || !mailsAreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var mails = new List<Mail>();

                foreach (var mailDto in prisonerDto.Mails)
                {
                    mails.Add(new Mail
                    {
                        Description = mailDto.Description,
                        Address = mailDto.Address,
                        Sender = mailDto.Sender,
                    });
                }

                var releaseDate = prisonerDto.ReleaseDate == null
                    ? (DateTime?)null
                    : DateTime.ParseExact(prisonerDto.ReleaseDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture);

                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.NickName,
                    CellId = prisonerDto.CellId,
                    Age = prisonerDto.Age,
                    Bail = prisonerDto.Bail,
                    IncarcerationDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReleaseDate = releaseDate,
                    Mails = mails.ToArray(),
                };

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");

                prisoners.Add(prisoner);
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();
            return sb.ToString();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));
            ImportOfficerDto[] importOfficerDtos;

            using (var reader = new StringReader(xmlString))
            {
                importOfficerDtos = (ImportOfficerDto[])serializer.Deserialize(reader);
            }

            var officers = new List<Officer>();
            var prisoners = context.Prisoners.ToList();

            foreach (var officerDto in importOfficerDtos)
            {
                var dtoIsValid = IsValid(officerDto);
                var positionIsValid = Enum.IsDefined(typeof(Position), officerDto.Position);
                var weaponIsValid = Enum.IsDefined(typeof(Weapon), officerDto.Weapon);

                if (!dtoIsValid || !positionIsValid || !weaponIsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var currPrisoners = new List<Prisoner>();

                foreach (var prisonerDto in officerDto.Prisoners)
                {
                    var prisoner = prisoners.FirstOrDefault(p => p.Id == int.Parse(prisonerDto.Id));

                    currPrisoners.Add(prisoner);
                }

                var officer = new Officer
                {
                    FullName = officerDto.Name,
                    Salary = officerDto.Money,
                    Position = Enum.Parse<Position>(officerDto.Position),
                    Weapon = Enum.Parse<Weapon>(officerDto.Weapon),
                    DepartmentId = officerDto.DepartmentId,
                    OfficerPrisoners = currPrisoners.Select(cp => new OfficerPrisoner { Prisoner = cp }).ToList(),
                };

                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                officers.Add(officer);
            }

            context.AddRange(officers);
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