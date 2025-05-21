using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Infra.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Core.Services
{
        public class OpenTextAnswerService : IOpenTextAnswerService
    {
            private readonly IOpenTextAnswerRepository _openTextAnswerRepository;

            public OpenTextAnswerService(IOpenTextAnswerRepository openTextAnswerRepository)
            {
                _openTextAnswerRepository = openTextAnswerRepository;
            }

            public async Task<OpenTextAnswer?> GetByIdAsync(Guid id)
            {
                return await _openTextAnswerRepository.GetByIdAsync(id);
            }

        public async Task<OpenTextAnswer> CreateAsync(OpenTextAnswer answer)
        {
            await _openTextAnswerRepository.AddAsync(answer);
            await _openTextAnswerRepository.SaveChangesAsync();
            return answer;
        }

    }
}
