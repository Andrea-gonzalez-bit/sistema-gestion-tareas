using ApiGenerico.Application.Dtos;

namespace ApiGenerico.Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<PagedResultDto<TaskItemDto>> GetAllAsync(TaskQueryDto query, CancellationToken cancellationToken = default);
        Task<TaskItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto, CancellationToken cancellationToken = default);
        Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<StateDto>> GetStatesAsync(CancellationToken cancellationToken = default);
    }
}
