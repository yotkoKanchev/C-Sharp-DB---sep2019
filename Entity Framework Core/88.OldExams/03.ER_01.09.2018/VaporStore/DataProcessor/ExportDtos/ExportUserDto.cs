namespace VaporStore.DataProcessor.ExportDtos
{
    using System.Xml.Serialization;

    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        public ExportPurchaseDto[] Purchases { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
