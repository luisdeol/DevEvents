namespace DevEvents.API.Domain.Entities
{
    public class Registration : BaseEntity
    {
        public Registration()
        {
            
        }
        public Registration(int idConference, int idAttendee) : base()
        {
            IdConference = idConference;
            IdAttendee = idAttendee;
        }

        public int IdConference { get; set; }
        public Conference Conference { get; set; }
        public int IdAttendee { get; set; }
        public Attendee Attendee { get; set; }
    }
}
