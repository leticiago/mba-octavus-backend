using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Mappings
{
    public class ActivityStudentMap : IEntityTypeConfiguration<ActivityStudent>
    {
        public void Configure(EntityTypeBuilder<ActivityStudent> builder)
        {
            builder.ToTable("ActivityStudents");

            builder.HasKey(a => new { a.StudentId, a.ActivityId });

            builder.Property(a => a.Score)
                .IsRequired();

            builder.Property(a => a.Comment)
                .HasMaxLength(500);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.StudentId);

            builder.HasOne<Activity>()
                .WithMany()
                .HasForeignKey(a => a.ActivityId);
        }
    }
}
