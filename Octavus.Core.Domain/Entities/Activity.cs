using System;

namespace Octavus.Core.Domain.Entities;
public class Activity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }
    public string Level { get; set; }
    public bool IsPublic { get; set; }
    public Guid InstrumentId { get; set; }
    public Guid? ProfessorId { get; set; }
    public ICollection<Question> Questions { get; set; }
}
