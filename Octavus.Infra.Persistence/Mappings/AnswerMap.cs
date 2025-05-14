using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Octavus.Core.Domain.Entities;

public class AnswerMap : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Text)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.IsCorrect)
            .IsRequired();

        builder.Property(a => a.QuestionId)
            .IsRequired();

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
