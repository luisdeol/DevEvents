namespace DevEvents.API.Infrastructure.Persistence.Models
{
    public class ConferencesFilterCriteria : ConferenceFilterCriteriaBase
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ConferencesFilterCriteriaModel
    {
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
