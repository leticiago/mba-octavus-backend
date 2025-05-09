using Microsoft.EntityFrameworkCore;

namespace Octavus.Infra.Persistence
{
    public class Context : DbContext
    {
        //public DbSet<Tabela> Nome { get; set; }

        public Context(DbContextOptions<OctavusDbContext> options)
            : base(options)
        {
        }

        public Context() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"");
            base.OnConfiguring(optionsBuilder);
        }

        protected overridevoid OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
            //modelBuilder.ApplyConfiguration(new TabelaMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
