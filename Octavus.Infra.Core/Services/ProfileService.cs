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
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<IEnumerable<Profile>> GetAllAsync()
        {
            return await _profileRepository.GetAllAsync();
        }

        public async Task<Profile?> GetByIdAsync(Guid id)
        {
            return await _profileRepository.GetByIdAsync(id);
        }

        public async Task<Profile> CreateAsync(Profile profile)
        {
            await _profileRepository.AddAsync(profile);
            await _profileRepository.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> UpdateAsync(Profile profile)
        {
            var existingProfile = await _profileRepository.GetByIdAsync(profile.Id);
            if (existingProfile == null) return false;

            existingProfile.Name = profile.Name;

            _profileRepository.UpdateAsync(existingProfile);
            return await _profileRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingProfile = await _profileRepository.GetByIdAsync(id);
            if (existingProfile == null) return false;

            _profileRepository.DeleteAsync(existingProfile.Id);
            return await _profileRepository.SaveChangesAsync();
        }
    }
}
