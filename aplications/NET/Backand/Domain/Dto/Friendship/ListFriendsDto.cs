using System;
using Domain.Enum;

namespace Domain.Dto.Friendship;

public class ListFriendsDto
{
        public Guid FriendshipId { get; set; }
        public Guid FriendId { get; set; }
        public string? FriendName { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }

        public string StatusLocationShare { get; set; }

        public string IDLocationShare { get; set; }


}
   