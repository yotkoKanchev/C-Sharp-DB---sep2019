namespace Cinema.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Customer")]
    public class ImportCustomerDto
    {
        [XmlElement]
        [MinLength(3), MaxLength(20), Required]
        public string FirstName { get; set; }

        [XmlElement]
        [MinLength(3), MaxLength(20), Required]
        public string LastName { get; set; }

        [XmlElement]
        [Range(12, 110)]
        public int Age { get; set; }

        [XmlElement]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [XmlArray]
        public ImportTicketDto[] Tickets { get; set; }
    }
}
