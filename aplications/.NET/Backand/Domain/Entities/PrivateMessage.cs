using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PrivateMessage
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public User? Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
