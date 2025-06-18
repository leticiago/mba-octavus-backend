using Octavus.Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, CreateUserDto dto);
        Task DeleteAsync(Guid id);
        Task<UserDto> GetByEmailAsync(string email);
    }

}
