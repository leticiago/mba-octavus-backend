using Moq;
using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octavus.Tests.Services
{
    [TestFixture]
    public class QuestionServiceTests
    {
        private Mock<IQuestionRepository> _mockRepo = null!;
        private QuestionService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IQuestionRepository>();
            _service = new QuestionService(_mockRepo.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldAddQuestionAndReturnIt()
        {
            var dto = new QuestionOpenTextDto
            {
                Title = "Qual a nota dó?",
                ActivityId = Guid.NewGuid()
            };

            var result = await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<Question>(q =>
                q.Title == dto.Title &&
                q.ActivityId == dto.ActivityId)), Times.Once);

            Assert.That(result.Title, Is.EqualTo(dto.Title));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedQuestions()
        {
            var list = new List<Question>
            {
                new Question
                {
                    Id = Guid.NewGuid(),
                    Title = "Pergunta 1",
                    Answers = new List<Answer>
                    {
                        new Answer { Id = Guid.NewGuid(), Text = "A", IsCorrect = true },
                        new Answer { Id = Guid.NewGuid(), Text = "B", IsCorrect = false }
                    }
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Pergunta 1"));
            Assert.That(result[0].Answers.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedQuestions_WhenExists()
        {
            var id = Guid.NewGuid();
            var questions = new List<Question>
            {
                new Question
                {
                    Id = Guid.NewGuid(),
                    Title = "Qual a escala maior?",
                    Answers = new List<Answer>
                    {
                        new Answer { Id = Guid.NewGuid(), Text = "C", IsCorrect = true }
                    }
                }
            };

            _mockRepo.Setup(r => r.GetByActivityIdAsync(id)).ReturnsAsync(questions);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result!.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetByIdAsync_ShouldThrowException_WhenActivityNotFound()
        {
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByActivityIdAsync(id)).ReturnsAsync((List<Question>?)null);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetByIdAsync(id));
            Assert.That(ex!.Message, Is.EqualTo("Atividade não encontrada."));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateQuestion_WhenExists()
        {
            var id = Guid.NewGuid();
            var existing = new Question
            {
                Id = id,
                Title = "Velho título",
                Answers = new List<Answer>()
            };

            var dto = new CreateQuestionDto
            {
                Title = "Novo título",
                Answers = new List<CreateAnswerDto>
                {
                    new CreateAnswerDto { Text = "Resposta 1", IsCorrect = true },
                    new CreateAnswerDto { Text = "Resposta 2", IsCorrect = false }
                }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            await _service.UpdateAsync(id, dto);

            Assert.That(existing.Title, Is.EqualTo("Novo título"));
            Assert.That(existing.Answers.Count, Is.EqualTo(2));
        }

        [Test]
        public void UpdateAsync_ShouldThrowException_WhenQuestionNotFound()
        {
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Question?)null);

            var dto = new CreateQuestionDto
            {
                Title = "Nova",
                Answers = new List<CreateAnswerDto>()
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.UpdateAsync(id, dto));
            Assert.That(ex!.Message, Is.EqualTo("Pergunta não encontrada."));
        }

        [Test]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task AddQuestionsBatchAsync_ShouldAddAllQuestions()
        {
            var dto = new CreateQuestionBatchDto
            {
                ActivityId = Guid.NewGuid(),
                Questions = new List<CreateQuestionDto>
                {
                    new CreateQuestionDto
                    {
                        Title = "Questão 1",
                        Answers = new List<CreateAnswerDto>
                        {
                            new CreateAnswerDto { Text = "A", IsCorrect = true }
                        }
                    },
                    new CreateQuestionDto
                    {
                        Title = "Questão 2",
                        Answers = new List<CreateAnswerDto>
                        {
                            new CreateAnswerDto { Text = "B", IsCorrect = false }
                        }
                    }
                }
            };

            await _service.AddQuestionsBatchAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Question>()), Times.Exactly(2));
        }

        [TestFixture]
        public class QuestionServiceAdditionalTests
        {
            private Mock<IQuestionRepository> _mockRepo = null!;
            private QuestionService _service = null!;

            [SetUp]
            public void Setup()
            {
                _mockRepo = new Mock<IQuestionRepository>();
                _service = new QuestionService(_mockRepo.Object);
            }

            [Test]
            public async Task CreateAsync_ShouldGenerateNewGuidForQuestion()
            {
                var dto = new QuestionOpenTextDto
                {
                    Title = "Teste Guid",
                    ActivityId = Guid.NewGuid()
                };

                var result = await _service.CreateAsync(dto);

                Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            }

            [Test]
            public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoQuestions()
            {
                _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Question>());

                var result = await _service.GetAllAsync();

                Assert.IsNotNull(result);
                Assert.That(result.Count, Is.EqualTo(0));
            }

            [Test]
            public async Task GetByIdAsync_ShouldReturnEmptyList_WhenNoQuestionsForActivity()
            {
                var activityId = Guid.NewGuid();
                _mockRepo.Setup(r => r.GetByActivityIdAsync(activityId)).ReturnsAsync(new List<Question>());

                var result = await _service.GetByIdAsync(activityId);

                Assert.IsNotNull(result);
                Assert.That(result.Count, Is.EqualTo(0));
            }

            [Test]
            public async Task UpdateAsync_ShouldUpdateAnswersWithNewGuids()
            {
                var id = Guid.NewGuid();
                var existing = new Question
                {
                    Id = id,
                    Title = "Título Antigo",
                    Answers = new List<Answer>()
                };

                var dto = new CreateQuestionDto
                {
                    Title = "Título Atualizado",
                    Answers = new List<CreateAnswerDto>
            {
                new CreateAnswerDto { Text = "Resp 1", IsCorrect = true },
                new CreateAnswerDto { Text = "Resp 2", IsCorrect = false }
            }
                };

                _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                _mockRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

                await _service.UpdateAsync(id, dto);

                Assert.That(existing.Title, Is.EqualTo(dto.Title));
                Assert.That(existing.Answers.Count, Is.EqualTo(dto.Answers.Count));
                Assert.That(existing.Answers.All(a => a.Id != Guid.Empty), Is.True);
            }

            [Test]
            public void DeleteAsync_ShouldThrowException_WhenRepositoryThrows()
            {
                var id = Guid.NewGuid();
                _mockRepo.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("Erro ao deletar"));

                var ex = Assert.ThrowsAsync<Exception>(async () => await _service.DeleteAsync(id));
                Assert.That(ex!.Message, Is.EqualTo("Erro ao deletar"));
            }

            [Test]
            public async Task AddQuestionsBatchAsync_ShouldHandleEmptyQuestionList()
            {
                var dto = new CreateQuestionBatchDto
                {
                    ActivityId = Guid.NewGuid(),
                    Questions = new List<CreateQuestionDto>()
                };

                await _service.AddQuestionsBatchAsync(dto);

                _mockRepo.Verify(r => r.AddAsync(It.IsAny<Question>()), Times.Never);
            }
        }

    }
}