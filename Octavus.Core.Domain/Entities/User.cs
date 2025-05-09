using System;

namespace Octavus.Core.Domain.Entities;

public class User : Entity
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; } 
    public required string Username { get; set; }
    public string Contact { get; set; } = string.Empty;
    public Guid InstrumentId { get; set; } = Guid.NewGuid();
    public Guid ProfileId { get; set; } = Guid.NewGuid();
}
