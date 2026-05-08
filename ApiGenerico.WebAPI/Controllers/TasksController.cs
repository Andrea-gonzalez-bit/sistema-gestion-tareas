using ApiGenerico.Application.Dtos;
using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGenerico.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TaskItemDto>>> GetAll([FromQuery] TaskQueryDto query, CancellationToken cancellationToken)
        {
            var tasks = await _taskService.GetAllAsync(query, cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskItemDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var task = await _taskService.GetByIdAsync(id, cancellationToken);
            if (task is null)
            {
                return NotFound(CreateErrorResponse($"No existe una tarea con el id {id}."));
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskItemDto dto, CancellationToken cancellationToken)
        {
            var created = await _taskService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaskItemDto>> Update(int id, [FromBody] UpdateTaskItemDto dto, CancellationToken cancellationToken)
        {
            var updated = await _taskService.UpdateAsync(id, dto, cancellationToken);
            if (updated is null)
            {
                return NotFound(CreateErrorResponse($"No fue posible actualizar la tarea porque no existe una tarea con el id {id}."));
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _taskService.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound(CreateErrorResponse($"No fue posible eliminar la tarea porque no existe una tarea con el id {id}."));
            }

            return NoContent();
        }

        [HttpGet("states")]
        public async Task<ActionResult<IReadOnlyCollection<StateDto>>> GetStates(CancellationToken cancellationToken)
        {
            var states = await _taskService.GetStatesAsync(cancellationToken);
            return Ok(states);
        }

        private ApiErrorResponse CreateErrorResponse(string message)
        {
            return new ApiErrorResponse
            {
                Message = message,
                TraceId = HttpContext.TraceIdentifier
            };
        }
    }
}
