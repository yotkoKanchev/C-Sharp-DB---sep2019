namespace Cinema.DataProcessor.ImportDto
{
    using System.Xml.Serialization;

    [XmlType("Customer")]
    public class ImportCustomerTicketDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public decimal Balance { get; set; }

        public ImportTicketDto[] Tickets { get; set; }
    }
}
