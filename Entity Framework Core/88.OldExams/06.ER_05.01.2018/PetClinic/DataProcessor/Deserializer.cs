namespace PetClinic.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;
    using DataProcessor.ImportDtos;
    using Models;
    using Newtonsoft.Json;
    using System.Xml.Serialization;
    using System.IO;

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
            var sb = new StringBuilder();
            var animals = new List<Animal>();
            var passports = new HashSet<Passport>();

            var importAnimalDtos = JsonConvert.DeserializeObject<ImportAnimalDto[]>(jsonString);

            foreach (var dto in importAnimalDtos)
            {
                if (!IsValid(dto) || !IsValid(dto.Passport) ||
                    animals.Any(a => a.PassportSerialNumber == dto.Passport.SerialNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var passport = passports.Any(p => p.SerialNumber == dto.Passport.SerialNumber)
                    ? passports.First(p => p.SerialNumber == dto.Passport.SerialNumber)
                    : new Passport
                    {
                        SerialNumber = dto.Passport.SerialNumber,
                        OwnerName = dto.Passport.OwnerName,
                        OwnerPhoneNumber = dto.Passport.OwnerPhoneNumber,
                        RegistrationDate = DateTime.ParseExact(dto.Passport.RegistrationDate, @"dd-MM-yyyy", CultureInfo.InvariantCulture),
                    };

                passports.Add(passport);

                var animal = new Animal
                {
                    Name = dto.Name,
                    Age = dto.Age,
                    Type = dto.Type,
                    Passport = passport,
                    PassportSerialNumber = dto.Passport.SerialNumber
                };

                sb.AppendLine($"Record {animal.Name} Passport №: {passport.SerialNumber} successfully imported.");
                animals.Add(animal);
            }

            context.Animals.AddRange(animals);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var vets = new List<Vet>();
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportVetDto[]), new XmlRootAttribute("Vets"));

            ImportVetDto[] importVetDtos;

            using (var reader = new StringReader(xmlString))
            {
                importVetDtos = (ImportVetDto[])serializer.Deserialize(reader);
            }

            foreach (var dto in importVetDtos)
            {
                if (!IsValid(dto) || vets.Any(v =>v.PhoneNumber == dto.PhoneNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var vet = new Vet
                {
                    Name = dto.Name,
                    Profession = dto.Profession,
                    Age = dto.Age,
                    PhoneNumber = dto.PhoneNumber,
                };

                sb.AppendLine($"Record {vet.Name} successfully imported.");
                vets.Add(vet);
            }

            context.Vets.AddRange(vets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

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
