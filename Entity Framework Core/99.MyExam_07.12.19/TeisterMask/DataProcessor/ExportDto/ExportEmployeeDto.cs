namespace TeisterMask.DataProcessor.ExportDto
{
    public class ExportEmployeeDto
    {
        public string Username { get; set; }

        public ExportTaskEmployeeDto[] Tasks { get; set; }
    }
}
