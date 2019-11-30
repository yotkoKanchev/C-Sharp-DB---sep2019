namespace VaporStore.DataProcessor.ExportDtos
{
    public class ExportGenreDto
    {
        public int Id { get; set; }

        public string Genre { get; set; }

        public ExportGameDto[] Games { get; set; }

        public int TotalPlayers { get; set; }
    }
}
