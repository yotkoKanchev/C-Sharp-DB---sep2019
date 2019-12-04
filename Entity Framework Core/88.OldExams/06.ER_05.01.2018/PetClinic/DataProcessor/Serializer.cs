namespace PetClinic.DataProcessor
{
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;
    using PetClinic.Data;
    using PetClinic.DataProcessor.ExportDtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .OrderBy(a => a.Age)
                .ThenBy(a => a.PassportSerialNumber)
                .Select(a => new
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.PassportSerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                })
                .ToList();

            return JsonConvert.SerializeObject(animals, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var sb = new StringBuilder();

            var procedures = context.Procedures
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Animal.PassportSerialNumber)
                .Select(p => new ExportProcedureDto
                {
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    Passport = p.Animal.PassportSerialNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.ProcedureAnimalAids.Select(paa => new ExportAnimalAidDto
                    {
                        Name = paa.AnimalAid.Name,
                        Price = paa.AnimalAid.Price
                    }).ToArray(),
                    TotalPrice = p.ProcedureAnimalAids.Sum(paa => paa.AnimalAid.Price)
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportProcedureDto[]), new XmlRootAttribute("Procedures"));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, procedures, ns);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
