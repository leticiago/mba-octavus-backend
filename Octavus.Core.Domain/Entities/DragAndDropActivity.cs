using System;

namespace Octavus.Core.Domain.Entities;
public class DragAndDropActivity : Entity
{
    public Guid Id { get; set; }
    public Guid ActivityId { get; set; }
    public string Text { get; set; } = string.Empty;

    public Activity Activity { get; set; }
}
