using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octavus.Infra.Persistence.Mappings
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Octavus.Core.Domain.Entities;

    public class ProfessorStudentMap : IEntityTypeConfiguration<ProfessorStudent>
    {
        public void Configure(EntityTypeBuilder<ProfessorStudent> builder)
        {
            builder.ToTable("ProfessorStudents");

            builder.HasKey(ps => new { ps.StudentId, ps.ProfessorId });

            builder.Property(ps => ps.StudentId)
                .IsRequired();

            builder.Property(ps => ps.ProfessorId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ps => ps.Date)
                .IsRequired();

            builder.Property(ps => ps.Active)
                .IsRequired();

            builder.Property(ps => ps.InstrumentId)
                .IsRequired();

            builder.HasOne<User>().WithMany().HasForeignKey(ps => ps.StudentId);
        }
    }

}
