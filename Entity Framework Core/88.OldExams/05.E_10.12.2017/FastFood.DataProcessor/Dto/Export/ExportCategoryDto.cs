namespace FastFood.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("Category")]
    public class ExportCategoryDto
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public MostPopularItemDto MostPopularItem { get; set; }
    }
}
