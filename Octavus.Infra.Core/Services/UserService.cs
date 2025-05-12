using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Octavus.Core.Application.DTO.KeycloakUser;

namespace Octavus.Infra.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IKeycloakService _keycloakService;

        public UserService(IUserRepository repository, IKeycloakService keycloakService)
        {
            _repository = repository;
            _keycloakService = keycloakService;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Name = dto.Name,
                Username = dto.Username,
                Password = dto.Password,
                Contact = dto.Contact,
                InstrumentId = dto.InstrumentId,
                ProfileId = dto.ProfileId
            };

            var kcUser = new KeycloakUser
            {
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.Name,
                LastName = "",
                Enabled = true,
                Credentials = new() {
                new Credential { Type = "password", Value = dto.Password, Temporary = false }
            },
                Roles = dto.Roles
            };

            var kcResult = await _keycloakService.CreateUserAndAssignRolesAsync(kcUser);
            if (!kcResult)
                throw new Exception("Erro ao cadastrar usuário no Keycloak.");

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Username = user.Username,
                Contact = user.Contact
            };
        }

        public async Task<List<UserDto>> GetAllAsync() =>
            (await _repository.GetAllAsync()).Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Username = u.Username,
                Contact = u.Contact
            }).ToList();

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id) ?? throw new Exception("Usuário não encontrado.");
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Username = user.Username,
                Contact = user.Contact
            };
        }

        public async Task UpdateAsync(Guid id, CreateUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id) ?? throw new Exception("Usuário não encontrado.");
            user.Name = dto.Name;
            user.Contact = dto.Contact;
            user.Password = dto.Password;
            user.Email = dto.Email;
            user.Username = dto.Username;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id) ?? throw new Exception("Usuário não encontrado.");
            await _repository.DeleteAsync(user.Id);
            await _repository.SaveChangesAsync();

        }
    }

}
