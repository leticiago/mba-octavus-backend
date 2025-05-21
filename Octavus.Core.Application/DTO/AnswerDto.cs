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

    public class AnswerOpenTextDto
    {
        public Guid QuestionId { get; set; }
        public Guid StudentId { get; set; }
        public string ResponseText { get; set; } = string.Empty;
    }

    public class SubmitAnswersDto
    {
        public Guid ActivityId { get; set; }
        public Guid StudentId { get; set; }

        public List<AnswerSubmissionDto> Answers { get; set; }
    }

    public class AnswerSubmissionDto
    {
        public Guid QuestionId { get; set; }
        public Guid SelectedAnswerId { get; set; }
    }

    public class SubmitDragAndDropDto
    {
        public Guid ActivityId { get; set; }
        public Guid StudentId { get; set; }
        public List<string> SelectedOptions { get; set; }
    }

}
