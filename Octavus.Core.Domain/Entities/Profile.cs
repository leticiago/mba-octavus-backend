using System;

namespace Octavus.Core.Domain.Entities;
public class Profile : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }

}
