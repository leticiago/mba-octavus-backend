using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
    public class InstrumentService : IInstrumentService
    {
        private readonly IInstrumentRepository _repository;

        public InstrumentService(IInstrumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Instrument>> GetAllAsync()
        {
            var instruments = await _repository.GetAllAsync();
            return instruments;
        }

        public async Task<Instrument?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity;
        }

        public async Task<Instrument> CreateAsync(Instrument dto)
        {
            var entity = dto;
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Instrument dto)
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null) return false;

            existing.Name = dto.Name;
            await _repository.UpdateAsync(existing);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null) return false;

            await _repository.DeleteAsync(entity.Id);
            return await _repository.SaveChangesAsync();
        }
    }
}
