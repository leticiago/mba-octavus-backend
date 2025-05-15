using Octavus.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class CreateActivityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        public Level Level { get; set; }
        public bool IsPublic { get; set; }
        public Guid InstrumentId { get; set; }
        public Guid? ProfessorId { get; set; }
    }

    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ActivityType Type { get; set; }
        public DateTime Date { get; set; }
        public Level Level { get; set; }
        public bool IsPublic { get; set; }
        public Guid InstrumentId { get; set; }
        public Guid? ProfessorId { get; set; }
    }
}
