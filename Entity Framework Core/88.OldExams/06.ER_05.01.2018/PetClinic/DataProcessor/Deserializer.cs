namespace PetClinic.DataProcessor
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;
    using DataProcessor.ImportDtos;
    using Models;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Error: Invalid data.";
        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var animalAidDtos = JsonConvert.DeserializeObject<ImportAnimalAidDto[]>(jsonString);
            var animalAids = new List<AnimalAid>();

            foreach (var dto in animalAidDtos)
            {
                if (!IsValid(dto) || animalAids.Any(a => a.Name == dto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var animalAid = new AnimalAid
                {
                    Name = dto.Name,
                    Price = dto.Price,
                };

                sb.AppendLine($"Record {animalAid.Name} successfully imported.");
                animalAids.Add(animalAid);
            }

            context.AnimalAids.AddRange(animalAids);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            return null;

        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            return null;

        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
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
