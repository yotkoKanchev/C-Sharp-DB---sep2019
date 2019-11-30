namespace VaporStore.DataProcessor.ImportDtos
{
    public class ImportGameDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ReleaseDate { get; set; }

        public string Developer { get; set; }

        public string Genre { get; set; }

        public string[] Tags { get; set; }
    }
}
