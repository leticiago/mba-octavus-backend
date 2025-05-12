using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Interfaces;

namespace Octavus.Infra.Persistence.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context)
        {
        }
    }

}
