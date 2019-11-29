namespace Cinema.DataProcessor.ExportDto
{
    using System.Collections.Generic;

    public class ExportMovieDto
    {
        public string MovieName { get; set; }

        public string Rating { get; set; }

        public string TotalIncomes { get; set; }

        public List<ExportCustomerDto> Customers { get; set; }
    }
}
