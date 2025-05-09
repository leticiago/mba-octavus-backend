using System;

namespace Octavus.Core.Domain.Entities;
public class ActivityStudent : Entity
{
    public Guid StudentId { get; set; }
    public Guid ActivityId { get; set; }
    public int Score { get; set; }
    public string Comment { get; set; } = string.Empty;

}
