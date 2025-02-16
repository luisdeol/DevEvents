namespace DevEvents.API.Domain.Entities
{
    public class Conference : BaseEntity
    {
        public Conference(string title, string description, DateTime startDate, DateTime endDate) : base()
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;

            Speakers = [];
            Registrations = [];
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<Speaker> Speakers { get; private set; }
        public List<Registration> Registrations { get; private set; }

        internal void Update(string title, string description, DateTime startDate, DateTime endDate)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
