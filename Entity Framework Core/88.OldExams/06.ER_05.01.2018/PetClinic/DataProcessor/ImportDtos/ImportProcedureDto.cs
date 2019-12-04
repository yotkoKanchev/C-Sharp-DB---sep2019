namespace PetClinic.DataProcessor.ImportDtos
{
    using System.Xml.Serialization;

    [XmlType("Procedure")]
    public class ImportProcedureDto
    {
        [XmlElement]
        public string Vet { get; set; }

        [XmlElement]
        public string Animal { get; set; }

        [XmlElement]
        public string DateTime { get; set; }

        [XmlArray]
        public ImportAnimalAidProcedureDto[] AnimalAids { get; set; }
    }
}
