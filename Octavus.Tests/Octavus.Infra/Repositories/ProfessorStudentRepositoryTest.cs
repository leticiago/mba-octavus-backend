using NUnit.Framework;
using Octavus.Core.Application.DTO;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence;
using Octavus.Infra.Persistence.Repositories;
using Octavus.Tests.Helpers;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Octavus.Tests.Repositories
{
    [TestFixture]
    public class ProfessorStudentRepositoryTests
    {
        private Context _context = null!;
        private ProfessorStudentRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            _context = TestContextProvider.CreateContext();
            _repository = new ProfessorStudentRepository(_context);

            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedData()
        {
            var professorId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var instrumentId = Guid.NewGuid();

            _context.Set<User>().AddRange(
                new User { Id = professorId, Name = "Prof. X", Email = "prof.x@example.com", Password = "password", Username = "user" },
                new User { Id = studentId, Name = "Aluno Y", Email = "aluno.y@example.com", Password = "password", Username = "user" }
            );

            _context.Set<Instrument>().Add(new Instrument { Id = instrumentId, Name = "Piano" });

            _context.Set<ProfessorStudent>().Add(new ProfessorStudent
            {
                Id = Guid.NewGuid(),
                ProfessorId = professorId,
                StudentId = studentId,
                InstrumentId = instrumentId,
                Active = true
            });

            _context.SaveChanges();
        }

        [Test]
        public void GetBond_ShouldReturnBond_WhenExists()
        {
            var professorStudent = _context.Set<ProfessorStudent>().First();

            var result = _repository.GetBond(professorStudent.StudentId, professorStudent.ProfessorId);

            Assert.IsNotNull(result);
            Assert.That(result!.StudentId, Is.EqualTo(professorStudent.StudentId));
            Assert.That(result.ProfessorId, Is.EqualTo(professorStudent.ProfessorId));
        }

        [Test]
        public void GetBond_ShouldReturnNull_WhenNotExists()
        {
            var result = _repository.GetBond(Guid.NewGuid(), Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetStudentsByProfessorAsync_ShouldReturnStudents_ForProfessor()
        {
            var professorStudent = _context.Set<ProfessorStudent>().First();

            var students = await _repository.GetStudentsByProfessorAsync(professorStudent.ProfessorId);

            Assert.IsNotNull(students);
            Assert.That(students.Count, Is.EqualTo(1));
            Assert.That(students[0].Name, Is.EqualTo("Aluno Y"));
            Assert.That(students[0].Instrument, Is.EqualTo("Piano"));
            Assert.That(students[0].Email, Is.EqualTo("aluno.y@example.com"));
        }

        [Test]
        public async Task GetStudentsByProfessorAsync_ShouldReturnEmpty_WhenNoActiveStudents()
        {
            var professorStudent = _context.Set<ProfessorStudent>().First();
            professorStudent.Active = false;
            _context.Update(professorStudent);
            await _context.SaveChangesAsync();

            var students = await _repository.GetStudentsByProfessorAsync(professorStudent.ProfessorId);

            Assert.IsNotNull(students);
            Assert.IsEmpty(students);
        }

        [Test]
        public async Task GetStudentsByProfessorAsync_ShouldReturnEmpty_WhenProfessorHasNoStudents()
        {
            var newProfessorId = Guid.NewGuid();
            _context.Set<User>().Add(new User { Id = newProfessorId, Name = "Prof. Novo", Email = "novo.prof@example.com", Password = "pass", Username = "user" });
            await _context.SaveChangesAsync();

            var students = await _repository.GetStudentsByProfessorAsync(newProfessorId);

            Assert.IsNotNull(students);
            Assert.IsEmpty(students);
        }

        [Test]
        public void GetBond_ShouldReturnNull_WhenStudentAndProfessorDoNotMatch()
        {
            var professorStudent = _context.Set<ProfessorStudent>().First();


            var unrelatedStudentId = Guid.NewGuid();
            var unrelatedProfessorId = Guid.NewGuid();

            var result = _repository.GetBond(professorStudent.StudentId, unrelatedProfessorId);
            Assert.IsNull(result);

            result = _repository.GetBond(unrelatedStudentId, professorStudent.ProfessorId);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetStudentsByProfessorAsync_ShouldReturnOnlyActiveStudents()
        {
            var professorStudent = _context.Set<ProfessorStudent>().First();

            var inactiveStudentId = Guid.NewGuid();
            var instrumentId = _context.Set<Instrument>().First().Id;

            _context.Set<User>().Add(new User { Id = inactiveStudentId, Name = "Aluno Inativo", Email = "inativo@example.com", Password = "pass", Username = "user" });
            _context.Set<ProfessorStudent>().Add(new ProfessorStudent
            {
                Id = Guid.NewGuid(),
                ProfessorId = professorStudent.ProfessorId,
                StudentId = inactiveStudentId,
                InstrumentId = instrumentId,
                Active = false
            });
            await _context.SaveChangesAsync();

            var students = await _repository.GetStudentsByProfessorAsync(professorStudent.ProfessorId);

            Assert.That(students.Count, Is.EqualTo(1));
            Assert.That(students.Any(s => s.Name == "Aluno Inativo"), Is.False);
        }

        [Test]
        public async Task GetStudentsByProfessorAsync_ShouldReturnCorrectData()
        {
            var professorStudent = _context.Set<ProfessorStudent>().First();
            var students = await _repository.GetStudentsByProfessorAsync(professorStudent.ProfessorId);

            var studentFromDb = _context.Set<User>().Find(students[0].Id);
            var instrumentFromDb = _context.Set<Instrument>().Find(professorStudent.InstrumentId);

            Assert.That(students[0].Email, Is.EqualTo(studentFromDb!.Email));
            Assert.That(students[0].Instrument, Is.EqualTo(instrumentFromDb!.Name));
        }

    }
}
