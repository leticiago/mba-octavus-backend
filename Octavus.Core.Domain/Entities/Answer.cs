using System;

namespace Octavus.Core.Domain.Entities;
public class Answer : Entity
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}
