namespace TeisterMask.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Task")]
    public class ExportTaskDto
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Label { get; set; }
    }
}
