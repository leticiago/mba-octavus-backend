using Octavus.Core.Application.Repositories;
using Octavus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Persistence.Repositories
{
    public class OpenTextAnswerRepository : RepositoryBase<OpenTextAnswer>, IOpenTextAnswerRepository
    {
        public OpenTextAnswerRepository(Context context) : base(context) { }
    }
}
