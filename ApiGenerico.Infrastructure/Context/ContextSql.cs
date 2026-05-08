using ApiGenerico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiGenerico.Infrastructure.Context
{
    public class ContextSql : DbContext
    {
        private readonly IConfiguration Config;

        public ContextSql(DbContextOptions<ContextSql> options, IConfiguration config) : base(options)
        {
            Config = config;
        }

        public DbSet<State> States => Set<State>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new StateConfiguration());
            modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
        }
    }
}
