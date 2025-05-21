using Octavus.Core.Domain.Enums;
using System;

namespace Octavus.Core.Domain.Entities;
public class ActivityStudent : Entity
{
    public Guid StudentId { get; set; }
    public Guid ActivityId { get; set; }
    public ActivityStatus Status { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; } = string.Empty;
    public bool IsCorrected { get; set; } = false;
    public DateTime? CorrectionDate { get; set; }

    public Activity Activity { get; set; }

}
