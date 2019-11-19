namespace CarDealer.DTO
{
    public class ExportCarsWithParts
    {
        public CarInfoDto car { get; set; }

        public PartInfoDto[] parts { get; set; }
    }
}
