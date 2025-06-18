using Octavus.Core.Application.DTO;
using Octavus.Core.Application.Repositories;
using Octavus.Core.Application.Services;
using Octavus.Core.Domain.Entities;
using System.Text;
using System.Text.RegularExpressions;
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
            ValidateEmail(dto.Email);
            ValidatePassword(dto.Password);

            var hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Name = dto.Name,
                Username = dto.Username,
                Password = hashedPassword,
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
                Credentials = new()
                {
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

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email) ?? throw new Exception("Usuário não encontrado.");
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

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail é obrigatório.");

            var isValidEmail = Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);

            if (!isValidEmail)
                throw new ArgumentException("e-mail inválido.");
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new ArgumentException("A senha deve ter pelo menos 8 caracteres.");

            var hasUpperCase = Regex.IsMatch(password, "[A-Z]");
            var hasSymbol = Regex.IsMatch(password, "[^a-zA-Z0-9]");

            if (!hasUpperCase || !hasSymbol)
                throw new ArgumentException("A senha deve conter ao menos uma letra maiúscula e um símbolo.");
        }
    }

}
