using System;

namespace Octavus.Core.Domain.Entities;
public class ProfessorStudent : Entity
{
    public Guid StudentId { get; set; }
    public string ProfessorId { get; set; }
    public DateTime Date { get; set; }
    public bool Active { get; set; }
    public Guid InstrumentId { get; set; }
}
