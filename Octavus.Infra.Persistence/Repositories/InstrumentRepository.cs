using Octavus.Core.Domain.Entities;
using Octavus.Core.Domain.Interfaces;

namespace Octavus.Infra.Persistence.Repositories
{
    public class InstrumentRepository : RepositoryBase<Instrument>, IInstrumentRepository
    {
        public InstrumentRepository(Context context) : base(context) { }
    }

}
