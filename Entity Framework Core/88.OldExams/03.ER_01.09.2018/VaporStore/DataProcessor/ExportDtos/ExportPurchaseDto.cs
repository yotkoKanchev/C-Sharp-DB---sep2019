namespace VaporStore.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("Purchase")]
    public class ExportPurchaseDto
    {
        [XmlElement]
        public string Card { get; set; }

        [XmlElement]
        public string Cvc { get; set; }

        [XmlElement]
        public string Date { get; set; }

        [XmlElement]
        public GameDto Game { get; set; }
    }
}
