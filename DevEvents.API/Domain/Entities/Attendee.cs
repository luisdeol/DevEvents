namespace DevEvents.API.Domain.Entities
{
    public class Attendee  : BaseEntity
    {
        public Attendee(string name, string email) : base()
        {
            Name = name;
            Email = email;;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public Registration Registration { get; }
    }
}
