namespace DevEvents.API.Infrastructure.Persistence.Models
{
    public class SingleConferenceCriteria : ConferenceFilterCriteriaBase
    {
        public SingleConferenceCriteria(int id, bool includeSpeakers = false, bool includeRegistrations = false)
        {
            Id = id;
            IncludeSpeakers = includeSpeakers;
            IncludeRegistrations = includeRegistrations;
        }

        public int Id { get; set; }
        
    }

    public class ConferenceFilterCriteriaBase
    {
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IncludeSpeakers { get; set; }
        public bool? IncludeRegistrations { get; set; }
    }
}
