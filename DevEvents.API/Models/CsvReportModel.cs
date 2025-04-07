namespace DevEvents.API.Models
{
    public class CsvReportModel
    {
        public byte[] Bytes { get; set; }
        public IEnumerable<AttendeeModel> Attendees { get; set; }
    }
}
