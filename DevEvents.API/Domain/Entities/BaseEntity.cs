namespace DevEvents.API.Domain.Entities
{
    public abstract class BaseEntity : IEntity
    {
        protected BaseEntity() 
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
