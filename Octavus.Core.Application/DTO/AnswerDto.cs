using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class CreateAnswerDto
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class AnswerDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

}
