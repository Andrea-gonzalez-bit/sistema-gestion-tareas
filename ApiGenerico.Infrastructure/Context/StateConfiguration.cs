using ApiGenerico.Domain.Constants;
using ApiGenerico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiGenerico.Infrastructure.Context
{
    public class StateConfiguration : IEntityTypeConfiguration<State>
    {
        public void Configure(EntityTypeBuilder<State> builder)
        {
            builder.ToTable("State");

            builder.HasKey(state => state.Id);

            builder.Property(state => state.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(state => state.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(state => state.UpdatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.HasIndex(state => state.Name)
                .IsUnique();

            builder.HasData(
                new State
                {
                    Id = DefaultStates.PendingId,
                    Name = "Pendiente",
                    CreatedAt = DefaultStates.SeedDateUtc,
                    UpdatedAt = DefaultStates.SeedDateUtc
                },
                new State
                {
                    Id = DefaultStates.InProgressId,
                    Name = "En Progreso",
                    CreatedAt = DefaultStates.SeedDateUtc,
                    UpdatedAt = DefaultStates.SeedDateUtc
                },
                new State
                {
                    Id = DefaultStates.CompletedId,
                    Name = "Completado",
                    CreatedAt = DefaultStates.SeedDateUtc,
                    UpdatedAt = DefaultStates.SeedDateUtc
                });
        }
    }
}
