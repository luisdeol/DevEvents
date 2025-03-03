namespace DevEvents.API.Domain.Entities
{
    public class Speaker : BaseEntity
    {
        public Speaker()
        {
            
        }
        public Speaker(string name, string bio, string website, int idConference) : base()
        {
            Name = name;
            Bio = bio;
            Website = website;
            IdConference = idConference;
        }

        public string Name { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }
        public int IdConference { get; set; }
    }
}
