namespace ProductShop.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("User")]
    public class UserProductsDto
    {
        [XmlElement("firstName")]
        public string firstName { get; set; }

        [XmlElement("lastName")]
        public string lastName { get; set; }

        [XmlArray("soldProducts")]
        public ExportProductDto[] SoldProducts { get; set; }
    }
}
