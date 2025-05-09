using System;

namespace Octavus.Core.Domain.Entities;
public class Question : Entity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    //public Blob Midia { get; set; }
}
