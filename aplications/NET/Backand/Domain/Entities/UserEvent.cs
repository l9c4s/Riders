namespace Domain.Entities
{
    public class UserEvent
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
