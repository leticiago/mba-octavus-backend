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
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Contact)
                .HasMaxLength(50);

            builder.Property(u => u.InstrumentId)
                .IsRequired();

            builder.Property(u => u.ProfileId)
                .IsRequired();

            builder.HasOne<Profile>().WithMany().HasForeignKey(u => u.ProfileId);
            builder.HasOne<Instrument>().WithMany().HasForeignKey(u => u.InstrumentId);
        }
    }

}
