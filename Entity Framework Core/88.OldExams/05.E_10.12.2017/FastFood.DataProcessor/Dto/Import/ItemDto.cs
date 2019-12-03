namespace FastFood.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Item")]
    public class ItemDto
    {
        [XmlElement]
        [MinLength(3), MaxLength(30), Required]
        public string Name { get; set; }

        [XmlElement]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
