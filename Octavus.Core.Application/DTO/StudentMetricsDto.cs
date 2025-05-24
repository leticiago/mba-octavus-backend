using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class StudentMetricsDto
    {
        public int TotalActivitiesDone { get; set; }
        public double AverageScore { get; set; }
        public Dictionary<string, double> AverageScoreByActivityType { get; set; }
    }
}
