using ApiGenerico.Application.Dtos;
using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Entities;
using ApiGenerico.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApiGenerico.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ContextSql _context;

        public TaskService(ContextSql context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<TaskItemDto>> GetAllAsync(TaskQueryDto query, CancellationToken cancellationToken = default)
        {
            if (query.DueDateFrom.HasValue && query.DueDateTo.HasValue && query.DueDateFrom > query.DueDateTo)
            {
                throw new InvalidOperationException("La fecha inicial no puede ser mayor que la fecha final.");
            }

            var tasksQuery = _context.Tasks
                .AsNoTracking()
                .Include(task => task.State)
                .AsQueryable();

            if (query.StateId.HasValue)
            {
                tasksQuery = tasksQuery.Where(task => task.StateId == query.StateId.Value);
            }

            if (query.DueDateFrom.HasValue)
            {
                tasksQuery = tasksQuery.Where(task => task.DueDate >= query.DueDateFrom.Value);
            }

            if (query.DueDateTo.HasValue)
            {
                tasksQuery = tasksQuery.Where(task => task.DueDate <= query.DueDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                tasksQuery = tasksQuery.Where(task =>
                    task.Title.ToLower().Contains(search) ||
                    (task.Description != null && task.Description.ToLower().Contains(search)));
            }

            var totalCount = await tasksQuery.CountAsync(cancellationToken);
            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var orderedQuery = ApplyOrdering(tasksQuery, query.SortBy, query.SortDirection);

            var items = await orderedQuery
                .Skip((pageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(task => new TaskItemDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    StateId = task.StateId,
                    StateName = task.State != null ? task.State.Name : string.Empty,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TaskItemDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }

        public async Task<TaskItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(task => task.State)
                .Where(task => task.Id == id)
                .Select(task => new TaskItemDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    StateId = task.StateId,
                    StateName = task.State != null ? task.State.Name : string.Empty,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto, CancellationToken cancellationToken = default)
        {
            await using var transaction = await BeginTransactionIfSupportedAsync(cancellationToken);
            await EnsureStateExistsAsync(dto.StateId, cancellationToken);

            var now = DateTime.UtcNow;
            var entity = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                DueDate = dto.DueDate,
                StateId = dto.StateId,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return (await GetByIdAsync(entity.Id, cancellationToken))!;
        }

        public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemDto dto, CancellationToken cancellationToken = default)
        {
            await using var transaction = await BeginTransactionIfSupportedAsync(cancellationToken);
            var entity = await _context.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
            if (entity is null)
            {
                return null;
            }

            await EnsureStateExistsAsync(dto.StateId, cancellationToken);

            entity.Title = dto.Title.Trim();
            entity.Description = dto.Description?.Trim();
            entity.DueDate = dto.DueDate;
            entity.StateId = dto.StateId;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var transaction = await BeginTransactionIfSupportedAsync(cancellationToken);
            var entity = await _context.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
            if (entity is null)
            {
                return false;
            }

            _context.Tasks.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            return true;
        }

        public async Task<IReadOnlyCollection<StateDto>> GetStatesAsync(CancellationToken cancellationToken = default)
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

        private async Task EnsureStateExistsAsync(int stateId, CancellationToken cancellationToken)
        {
            var exists = await _context.States.AnyAsync(state => state.Id == stateId, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException("El estado seleccionado no existe.");
            }
        }

        private static IQueryable<TaskItem> ApplyOrdering(IQueryable<TaskItem> query, string? sortBy, string? sortDirection)
        {
            var descending = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.Trim().ToLowerInvariant() switch
            {
                "title" => descending ? query.OrderByDescending(task => task.Title) : query.OrderBy(task => task.Title),
                "duedate" => descending ? query.OrderByDescending(task => task.DueDate) : query.OrderBy(task => task.DueDate),
                "state" or "statename" => descending
                    ? query.OrderByDescending(task => task.State != null ? task.State.Name : string.Empty)
                    : query.OrderBy(task => task.State != null ? task.State.Name : string.Empty),
                "createdat" => descending ? query.OrderByDescending(task => task.CreatedAt) : query.OrderBy(task => task.CreatedAt),
                "updatedat" => descending ? query.OrderByDescending(task => task.UpdatedAt) : query.OrderBy(task => task.UpdatedAt),
                _ => descending ? query.OrderByDescending(task => task.Id) : query.OrderBy(task => task.Id)
            };
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
