using ApiGenerico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiGenerico.Infrastructure.Context
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Task");

            builder.HasKey(task => task.Id);

            builder.Property(task => task.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(task => task.Description)
                .HasMaxLength(2000);

            builder.Property(task => task.DueDate)
                .HasColumnType("datetime");

            builder.Property(task => task.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(task => task.UpdatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.HasOne(task => task.State)
                .WithMany(state => state.Tasks)
                .HasForeignKey(task => task.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(task => task.StateId);
            builder.HasIndex(task => task.DueDate);
        }
    }
}
