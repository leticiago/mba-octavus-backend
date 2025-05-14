using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Octavus.Core.Domain.Entities;

namespace Octavus.Infra.Persistence.Mappings
{
    public class QuestionMap : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200);

           
            builder.Property(q => q.ActivityId)
                .IsRequired();

            builder.HasOne(q => q.Activity)
                .WithMany(a => a.Questions)
                .HasForeignKey(q => q.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
