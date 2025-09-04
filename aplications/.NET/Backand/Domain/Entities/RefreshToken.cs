using System;

namespace Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool RevokedAt { get; set; }
}
