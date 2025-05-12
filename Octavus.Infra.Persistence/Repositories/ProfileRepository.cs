using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Interfaces;

namespace Octavus.Infra.Persistence.Repositories
{
    public class ProfileRepository : RepositoryBase<Profile>, IProfileRepository
    {
        public ProfileRepository(Context context) : base(context) { }
    }

}
