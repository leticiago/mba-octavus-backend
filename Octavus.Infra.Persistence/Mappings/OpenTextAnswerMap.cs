using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Mappings
{
    public class OpenTextAnswerMap : IEntityTypeConfiguration<OpenTextAnswer>
    {
        public void Configure(EntityTypeBuilder<OpenTextAnswer> builder)
        {
            builder.ToTable("OpenTextAnswers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.QuestionId)
                .IsRequired();

            builder.Property(x => x.StudentId)
                .IsRequired();

            builder.Property(x => x.ResponseText)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.SubmittedAt)
                .IsRequired();

            builder.HasOne<Question>()
                .WithMany()
                .HasForeignKey(x => x.QuestionId);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.StudentId);
        }
    }
}
