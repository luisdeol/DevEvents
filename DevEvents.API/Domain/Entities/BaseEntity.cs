namespace DevEvents.API.Domain.Entities
{
    public abstract class BaseEntity : IEntity
    {
        protected BaseEntity() 
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public int Id { get; }
        public DateTime CreatedAt { get; }
        public bool IsDeleted { get; }
    }
}
