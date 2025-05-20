
using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
    public class DragAndDropActivityService : IDragAndDropActivityService
    {
        private readonly IDragAndDropActivityRepository _repository;

        public DragAndDropActivityService(IDragAndDropActivityRepository repository)
        {
            _repository = repository;
        }

        public async Task<DragAndDropActivityDto> CreateAsync(Guid activityId, string sequence)
        {
            var activity = new DragAndDropActivity
            {
                Id = Guid.NewGuid(),
                ActivityId = activityId,
                Text = sequence
            };

            await _repository.AddAsync(activity);
            await _repository.SaveChangesAsync();

            return new DragAndDropActivityDto
            {
                ActivityId = activity.Id,
                OriginalSequence = sequence,
                ShuffledOptions = Shuffle(sequence)
            };
        }

        public async Task<List<DragAndDropActivityDto>> GetAllAsync()
        {
            var all = await _repository.GetAllAsync();

            return all.Select(a => new DragAndDropActivityDto
            {
                ActivityId = a.Id,
                OriginalSequence = a.Text,
                ShuffledOptions = Shuffle(a.Text)
            }).ToList();
        }

        public async Task<DragAndDropActivityDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id) ?? throw new Exception("Atividade não encontrada.");

            return new DragAndDropActivityDto
            {
                ActivityId = entity.Id,
                OriginalSequence = entity.Text,
                ShuffledOptions = Shuffle(entity.Text)
            };
        }

        private List<string> Shuffle(string input)
        {
            var items = input.Split(';').ToList();
            var rnd = new Random();
            return items.OrderBy(x => rnd.Next()).ToList();
        }
    }

}
