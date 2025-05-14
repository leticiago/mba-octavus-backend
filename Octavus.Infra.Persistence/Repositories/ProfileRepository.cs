using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Repositories
{
    public class ProfileRepository : RepositoryBase<Profile>, IProfileRepository
    {
        public ProfileRepository(Context context) : base(context) { }
    }

}
