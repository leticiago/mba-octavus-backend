using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Domain.Entities
{
    public class OpenTextAnswer : Entity
    {
        public Guid QuestionId { get; set; }
        public Guid StudentId { get; set; }
        public string ResponseText { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
