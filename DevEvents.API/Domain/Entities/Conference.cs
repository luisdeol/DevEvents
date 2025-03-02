namespace DevEvents.API.Domain.Entities
{
    public class Conference : BaseEntity
    {
        public Conference()
        {
            Speakers = [];
            Registrations = [];
        }

        public Conference(string title, string description, DateTime startDate, DateTime endDate) : base()
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;

            Speakers = [];
            Registrations = [];
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Speaker> Speakers { get; set; }
        public List<Registration> Registrations { get; set; }

        internal void Update(string title, string description, DateTime startDate, DateTime endDate)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
