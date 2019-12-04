namespace PetClinic.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Newtonsoft.Json;
    using PetClinic.Data;

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

            return JsonConvert.SerializeObject(animals, Formatting.Indented);
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            return null;

        }
    }
}
