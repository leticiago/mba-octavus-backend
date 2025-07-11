using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _repository;

        public ActivityService(IActivityRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActivityDto> CreateAsync(CreateActivityDto dto)
        {
            var entity = new Activity()
            {
                Id = Guid.NewGuid(),
                Date = dto.Date,
                Description = dto.Description,
                InstrumentId = dto.InstrumentId,
                Level = dto.Level.ToString(),
                Name = dto.Name,
                ProfessorId = dto.ProfessorId,
                IsPublic = dto.IsPublic,
                Type = dto.Type.ToString(),
            };

            await _repository.AddAsync(entity);

            return new ActivityDto()
            {
                Type = dto.Type,
                IsPublic = dto.IsPublic,
                ProfessorId = dto.ProfessorId,
                Name = dto.Name,
                Level = dto.Level,
                InstrumentId = dto.InstrumentId,
                Description = dto.Description,
                Date = dto.Date,
                Id = entity.Id
            };
        }

        public async Task<IEnumerable<ActivityDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var response = new List<ActivityDto>();

            foreach (var entity in entities)
            {
                response.Add(new ActivityDto()
                {
                    Id = entity.Id,
                    Date = entity.Date,
                    Description = entity.Description,
                    InstrumentId = entity.InstrumentId,
                    Level = ActivityLevelExtensions.FromString(entity.Level),
                    Name = entity.Name,
                    ProfessorId = entity.ProfessorId,
                    IsPublic = entity.IsPublic,
                    Type = ActivityTypeExtensions.FromString(entity.Type)
                });
            }
            return response;
        }

        public async Task<ActivityDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return new ActivityDto()
            {
                Description = entity.Description,
                InstrumentId = entity.InstrumentId,
                IsPublic = entity.IsPublic,
                Type = ActivityTypeExtensions.FromString(entity.Type),
                ProfessorId = entity.ProfessorId,
                Date = entity.Date,
                Id = entity.Id,
                Level = ActivityLevelExtensions.FromString(entity.Level),
                Name = entity.Name,
            };
        }

        public async Task<IEnumerable<ActivityDto>> GetByProfessorIdAsync(Guid professorId, Guid? instrumentId)
        {
            var entities = await _repository.GetByProfessorIdAsync(professorId, instrumentId);
            var response = new List<ActivityDto>();

            foreach (var entity in entities)
            {
                response.Add(new ActivityDto()
                {
                    Id = entity.Id,
                    Date = entity.Date,
                    Description = entity.Description,
                    InstrumentId = entity.InstrumentId,
                    Level = ActivityLevelExtensions.FromString(entity.Level),
                    Name = entity.Name,
                    ProfessorId = entity.ProfessorId,
                    IsPublic = entity.IsPublic,
                    Type = ActivityTypeExtensions.FromString(entity.Type)
                });
            }
            return response;
        }

        public async Task UpdateAsync(Guid id, CreateActivityDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Atividade não encontrada");

            existing.Id = id;
            existing.Date = dto.Date;
            existing.Description = dto.Description;
            existing.InstrumentId = dto.InstrumentId;
            existing.Level = dto.Level.ToString();
            existing.Name = dto.Name;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Atividade não encontrada");

            await _repository.DeleteAsync(entity.Id);
        }

        public async Task<List<ActivityDto>> GetPublicActivitiesAsync()
        {
            var activities = await _repository.GetPublicActivitiesAsync();

            return activities.Select(a => new ActivityDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Type = ActivityTypeExtensions.FromString(a.Type),
                Date = a.Date,
                Level = ActivityLevelExtensions.FromString(a.Level),
                InstrumentId = a.InstrumentId,

            }).ToList();
        }
    }

}
