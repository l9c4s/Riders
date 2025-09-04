
namespace Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<UserGroup> Members { get; set; } = new List<UserGroup>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
