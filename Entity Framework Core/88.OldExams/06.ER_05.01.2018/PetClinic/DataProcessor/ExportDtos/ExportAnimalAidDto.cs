namespace PetClinic.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("AnimalAid")]
    public class ExportAnimalAidDto
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public decimal Price { get; set; }
    }
}
