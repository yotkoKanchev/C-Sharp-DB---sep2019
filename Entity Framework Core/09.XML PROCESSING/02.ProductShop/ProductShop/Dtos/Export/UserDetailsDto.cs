using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class UserDetailsDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public SoldProductsWithCountDto SoldProducts { get; set; }
    }
}
