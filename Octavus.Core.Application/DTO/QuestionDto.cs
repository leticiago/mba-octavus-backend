using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class CreateQuestionBatchDto
    {
        public Guid ActivityId { get; set; }
        public List<CreateQuestionDto> Questions { get; set; }
    }
    public class CreateQuestionDto
    {
        public string Title { get; set; }
        public List<CreateAnswerDto> Answers { get; set; } = new();
    }

    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<AnswerDto> Answers { get; set; } = new();
    }

}
