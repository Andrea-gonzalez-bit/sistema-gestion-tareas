using ApiGenerico.Application.Dtos;

namespace ApiGenerico.Application.Services.Interfaces
{
    public interface IStateService
    {
        Task<IReadOnlyCollection<StateDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<StateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<StateDto> CreateAsync(CreateStateDto dto, CancellationToken cancellationToken = default);
        Task<StateDto?> UpdateAsync(int id, UpdateStateDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
