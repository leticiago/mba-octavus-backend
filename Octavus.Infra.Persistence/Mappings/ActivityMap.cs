using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Mappings
{
    public class ActivityMap : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .IsRequired();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Date)
                .IsRequired();

            builder.Property(a => a.Level)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.IsPublic)
                .IsRequired();

            builder.Property(a => a.InstrumentId)
                .IsRequired();

            builder.Property(a => a.ProfessorId)
            .IsRequired(false);

            builder.HasOne<Instrument>()
            .WithMany()
            .HasForeignKey(a => a.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Questions)
                .WithOne(q => q.Activity)
                .HasForeignKey(q => q.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
