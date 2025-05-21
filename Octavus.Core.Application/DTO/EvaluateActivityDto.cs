using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class EvaluateActivityDto
    {
        public Guid StudentId { get; set; }
        public Guid ActivityId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

}
