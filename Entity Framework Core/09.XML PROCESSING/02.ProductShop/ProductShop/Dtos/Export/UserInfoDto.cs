using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class UserInfoDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UserDetailsDto[] Users { get; set; }
    }
}
