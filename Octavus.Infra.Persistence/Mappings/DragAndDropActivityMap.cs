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
    public class DragAndDropActivityMap : IEntityTypeConfiguration<DragAndDropActivity>
    {
        public void Configure(EntityTypeBuilder<DragAndDropActivity> builder)
        {
            builder.ToTable("DragAndDropActivities");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.ActivityId)
                .IsRequired();

            builder.Property(a => a.Text)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasOne<Activity>().WithMany().HasForeignKey(a => a.ActivityId);
        }
    }

}
