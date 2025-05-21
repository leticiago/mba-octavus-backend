using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class PendingActivityReviewDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid ActivityId { get; set; }
        public string ActivityName { get; set; }
    }

}
