using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Repositories
{
    public class InstrumentRepository : RepositoryBase<Instrument>, IInstrumentRepository
    {
        public InstrumentRepository(Context context) : base(context) { }
    }

}
