using ApiGenerico.Application.Dtos;
using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Entities;
using ApiGenerico.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApiGenerico.Application.Services
{
    public class StateService : IStateService
    {
        private readonly ContextSql _context;

        public StateService(ContextSql context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<StateDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.States
                .AsNoTracking()
                .OrderBy(state => state.Id)
                .Select(state => new StateDto
                {
                    Id = state.Id,
                    Name = state.Name,
                    CreatedAt = state.CreatedAt,
                    UpdatedAt = state.UpdatedAt,
                    TasksCount = state.Tasks.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<StateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.States
                .AsNoTracking()
                .Where(state => state.Id == id)
                .Select(state => new StateDto
                {
                    Id = state.Id,
                    Name = state.Name,
                    CreatedAt = state.CreatedAt,
                    UpdatedAt = state.UpdatedAt,
                    TasksCount = state.Tasks.Count
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<StateDto> CreateAsync(CreateStateDto dto, CancellationToken cancellationToken = default)
        {
            await using var transaction = await BeginTransactionIfSupportedAsync(cancellationToken);
            var normalizedName = dto.Name.Trim();
            var exists = await _context.States.AnyAsync(
                state => state.Name.ToLower() == normalizedName.ToLower(),
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("Ya existe un estado con ese nombre.");
            }

            var now = DateTime.UtcNow;
            var entity = new State
            {
                Name = normalizedName,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.States.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new StateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                TasksCount = 0
            };
        }

        public async Task<StateDto?> UpdateAsync(int id, UpdateStateDto dto, CancellationToken cancellationToken = default)
        {
            await using var transaction = await BeginTransactionIfSupportedAsync(cancellationToken);
            var entity = await _context.States.FirstOrDefaultAsync(state => state.Id == id, cancellationToken);
            if (entity is null)
            {
                return null;
            }

            var normalizedName = dto.Name.Trim();
            var exists = await _context.States.AnyAsync(
                state => state.Id != id && state.Name.ToLower() == normalizedName.ToLower(),
                cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("Ya existe un estado con ese nombre.");
            }

            entity.Name = normalizedName;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new StateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                TasksCount = await _context.Tasks.CountAsync(task => task.StateId == entity.Id, cancellationToken)
            };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var transaction = await BeginTransactionIfSupportedAsync(cancellationToken);
            var entity = await _context.States.FirstOrDefaultAsync(state => state.Id == id, cancellationToken);
            if (entity is null)
            {
                return false;
            }

            var hasTasks = await _context.Tasks.AnyAsync(task => task.StateId == id, cancellationToken);
            if (hasTasks)
            {
                throw new InvalidOperationException("No se puede eliminar un estado con tareas asociadas.");
            }

            _context.States.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            return true;
        }

        private async Task<IDbContextTransaction?> BeginTransactionIfSupportedAsync(CancellationToken cancellationToken)
        {
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                return null;
            }

            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}
