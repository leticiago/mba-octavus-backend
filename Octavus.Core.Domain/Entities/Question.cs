using System;

namespace Octavus.Core.Domain.Entities;
public class Question : Entity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    //public Blob Midia { get; set; }
    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; }

    public ICollection<Answer> Answers { get; set; }
}
