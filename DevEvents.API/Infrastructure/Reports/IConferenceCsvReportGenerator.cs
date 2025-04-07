using CsvHelper;
using DevEvents.API.Domain.Entities;
using DevEvents.API.Models;
using System.Globalization;

namespace DevEvents.API.Infrastructure.Reports
{
    public interface IConferenceCsvReportGenerator
    {
        Task Generate(Conference conference);
        Task<CsvReportModel> Read(int id);
    }

    public class ConferenceCsvReportGenerator : IConferenceCsvReportGenerator
    {
        public async Task Generate(Conference conference)
        {
            var attendees = conference.Registrations.Select(r => new AttendeeModel
            {
                Name = r.Attendee.Name,
                Email = r.Attendee.Email
            }).ToList();

            var fileName = $"attendees-{conference.Id}.csv";

            using (var writer = new StreamWriter(fileName))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csvWriter.WriteRecordsAsync(attendees);
                }
            }
        }

        public async Task<CsvReportModel> Read(int id)
        {
            var fileName = $"attendees-{id}.csv";

            var bytes = await File.ReadAllBytesAsync(fileName);

            using (var reader = new StreamReader(fileName))
            {
                using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csvReader.GetRecords<AttendeeModel>();

                    return new CsvReportModel { Attendees = records, Bytes = bytes };
                }
            }
        }
    }
}
