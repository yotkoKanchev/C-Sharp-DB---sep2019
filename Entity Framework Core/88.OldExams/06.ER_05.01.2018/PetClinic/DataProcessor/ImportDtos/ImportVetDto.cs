namespace PetClinic.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Vet")]
    public class ImportVetDto
    {
        [XmlElement]
        [MinLength(3), MaxLength(40), Required]
        public string Name { get; set; }

        [XmlElement]
        [MinLength(3), MaxLength(50), Required]
        public string Profession { get; set; }

        [XmlElement]
        [Range(22, 65)]
        public int Age { get; set; }

        [XmlElement]
        [RegularExpression(@"^((\+359)|(0))[0-9]{9}$"), Required]
        public string PhoneNumber { get; set; }
    }
}
