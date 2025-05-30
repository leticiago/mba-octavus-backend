using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class DragAndDropActivityDto
    {
        public Guid ActivityId { get; set; }
        public string OriginalSequence { get; set; }
        public List<string> ShuffledOptions { get; set; } = new();
    }

    public class CreateDragAndDropActivityDto
    {
        public Guid ActivityId { get; set; }
        public string OriginalSequence { get; set; }
    }

    public class DragAndDropSubmissionDto
    {
        public Guid ActivityId { get; set; }
        public Guid StudentId { get; set; }
        public string Answer { get; set; }
    }

    public class ActivityScoreResultDto
    {
        public int Score { get; set; }
        public int Total { get; set; }
        public int Correct { get; set; }
    }
}
