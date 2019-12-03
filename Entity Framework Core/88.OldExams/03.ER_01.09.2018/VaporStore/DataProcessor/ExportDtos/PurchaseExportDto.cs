namespace VaporStore.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("Purchase")]
    public class PurchaseExportDto
    {
        [XmlElement]
        public string Card { get; set; }

        [XmlElement]
        public string Cvc { get; set; }

        [XmlElement]
        public string Date { get; set; }

        [XmlElement]
        public GameExportDto Game { get; set; }
    }
}
