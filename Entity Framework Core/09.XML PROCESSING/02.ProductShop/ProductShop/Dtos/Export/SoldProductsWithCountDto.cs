using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class SoldProductsWithCountDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public ProductDto[] Products { get; set; }
    }
}
