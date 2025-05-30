using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class UpdateProfessorStudentDto
    {
        public Guid StudentId { get; set; }
        public Guid ProfessorId { get; set; }
        public bool? Active { get; set; }
        public Guid InstrumentId { get; set; }
    }
}
