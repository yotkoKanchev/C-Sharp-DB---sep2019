namespace PetClinic.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("AnimalAid")]
    public class ImportAnimalAidProcedureDto
    {
        [XmlElement]
        [MinLength(3), MaxLength(30), Required]
        public string Name { get; set; }
    }
}
