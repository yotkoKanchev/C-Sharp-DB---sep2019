using Newtonsoft.Json;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportHallDto
    {
        public string Name { get; set; }

        public bool Is4Dx { get; set; }

        public bool Is3D { get; set; }

        [JsonProperty("Seats")]
        public int SeatsCount { get; set; }
    }
}
