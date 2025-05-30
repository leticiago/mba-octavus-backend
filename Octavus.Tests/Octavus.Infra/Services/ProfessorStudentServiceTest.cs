using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octavus.Core.Application.Services;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Application.DTO;
using Octavus.Infra.Core.Services;

namespace Octavus.Tests.Services
{
    [TestFixture]
    public class ProfessorStudentServiceTests
    {
        private Mock<IProfessorStudentRepository> _mockProfessorStudentRepository = null!;
        private Mock<IUserRepository> _mockUserRepository = null!;
        private ProfessorStudentService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockProfessorStudentRepository = new Mock<IProfessorStudentRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _service = new ProfessorStudentService(
                _mockProfessorStudentRepository.Object,
                _mockUserRepository.Object);
        }

        [Test]
        public async Task LinkByEmailAsync_ShouldCreateLink_WhenEmailIsProvided()
        {
            var dto = new LinkStudentByEmailDto
            {
                StudentEmail = "aluno@email.com",
                ProfessorId = Guid.NewGuid(),
                InstrumentId = Guid.NewGuid()
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.StudentEmail!,
                Username = "Username",
                Password = "Password",
                Name = "Aluno Teste"
            };

            _mockUserRepository
                .Setup(r => r.GetByEmailAsync(dto.StudentEmail!))
                .ReturnsAsync(user);

            _mockProfessorStudentRepository
                .Setup(r => r.AddAsync(It.IsAny<ProfessorStudent>()))
                .Returns(Task.CompletedTask);

            await _service.LinkByEmailAsync(dto);

            _mockUserRepository.Verify(r => r.GetByEmailAsync(dto.StudentEmail!), Times.Once);
            _mockProfessorStudentRepository.Verify(r => r.AddAsync(It.Is<ProfessorStudent>(p =>
                p.StudentId == user.Id &&
                p.ProfessorId == dto.ProfessorId &&
                p.InstrumentId == dto.InstrumentId &&
                p.Active)), Times.Once);
        }

        [Test]
        public void LinkByEmailAsync_ShouldThrowException_WhenStudentNotFound()
        {
            var dto = new LinkStudentByEmailDto
            {
                StudentEmail = "naoencontrado@email.com",
                ProfessorId = Guid.NewGuid(),
                InstrumentId = Guid.NewGuid()
            };

            _mockUserRepository
                .Setup(r => r.GetByEmailAsync(dto.StudentEmail!))
                .ReturnsAsync((User?)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.LinkByEmailAsync(dto));

            Assert.That(ex!.Message, Is.EqualTo("Aluno não encontrado com o e-mail informado."));
        }

        [Test]
        public async Task UpdateLinkAsync_ShouldUpdateActiveStatus_WhenFound()
        {
            var studentId = Guid.NewGuid();
            var professorId = Guid.NewGuid();
            var link = new ProfessorStudent
            {
                StudentId = studentId,
                ProfessorId = professorId,
                Active = true
            };

            var dto = new UpdateProfessorStudentDto
            {
                StudentId = studentId,
                ProfessorId = professorId,
                InstrumentId = Guid.NewGuid(),
                Active = false
            };

            _mockProfessorStudentRepository
                .Setup(r => r.GetBond(studentId, professorId))
                .Returns(link);

            _mockProfessorStudentRepository
                .Setup(r => r.UpdateAsync(link))
                .Returns(Task.CompletedTask);

            await _service.UpdateLinkAsync(dto);

            Assert.IsFalse(link.Active);
            _mockProfessorStudentRepository.Verify(r => r.UpdateAsync(link), Times.Once);
        }

        [Test]
        public void UpdateLinkAsync_ShouldThrowException_WhenBondNotFound()
        {
            var dto = new UpdateProfessorStudentDto
            {
                StudentId = Guid.NewGuid(),
                ProfessorId = Guid.NewGuid(),
                InstrumentId = Guid.NewGuid(),
                Active = true
            };

            _mockProfessorStudentRepository
                .Setup(r => r.GetBond(dto.StudentId, dto.ProfessorId))
                .Returns((ProfessorStudent?)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.UpdateLinkAsync(dto));

            Assert.That(ex!.Message, Is.EqualTo("Vínculo não encontrado."));
        }

        [Test]
        public async Task GetStudentsByProfessorAsync_ShouldReturnStudentDtos()
        {
            var professorId = Guid.NewGuid();

            var students = new List<StudentDto>
            {
                new StudentDto { Id = Guid.NewGuid(), Name = "Aluno 1", Email = "a1@email.com" },
                new StudentDto {Id = Guid.NewGuid(), Name = "Aluno 2", Email = "a2@email.com", Instrument = "Piano"}
            };

            _mockProfessorStudentRepository
                .Setup(r => r.GetStudentsByProfessorAsync(professorId))
                .Returns(Task.FromResult(students));

            var result = await _service.GetStudentsByProfessorAsync(professorId);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Aluno 1", result[0].Name);
            Assert.AreEqual("Piano", result[1].Instrument);
        }
    }
}
