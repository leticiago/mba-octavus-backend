using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Core.Application.DTO
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Contact { get; set; }
        public Guid InstrumentId { get; set; }
        public Guid ProfileId { get; set; }
        public List<string> Roles { get; set; } = new();
    }

}
