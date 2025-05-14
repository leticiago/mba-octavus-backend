using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context)
        {
        }
    }

}
