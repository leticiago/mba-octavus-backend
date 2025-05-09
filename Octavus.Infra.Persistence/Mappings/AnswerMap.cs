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
    public class AnswerMap : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable("Answers");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.QuestionId)
                .IsRequired();

            builder.Property(a => a.Text)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(a => a.IsCorrect)
                .IsRequired();

            builder.HasOne<Question>().WithMany().HasForeignKey(a => a.QuestionId);
        }
    }

}
