using Octavus.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class ActivityStudentDto
    {
        public Guid ActivityId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ActivityStatus Status { get; set; }
        public int? Score { get; set; }
        public string Comment { get; set; }
        public bool IsCorrected { get; set; }
        public DateTime? CorrectionDate { get; set; }
    }

    public class StudentCompletedActivityDto
    {
        public Guid ActivityId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int? Score { get; set; }
        public DateTime CorrectionDate { get; set; }
    }

}
