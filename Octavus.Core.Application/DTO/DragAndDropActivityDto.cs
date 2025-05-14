using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class DragAndDropActivityDto
    {
        public Guid Id { get; set; }
        public string OriginalSequence { get; set; }
        public List<string> ShuffledOptions { get; set; } = new();
    }
}
