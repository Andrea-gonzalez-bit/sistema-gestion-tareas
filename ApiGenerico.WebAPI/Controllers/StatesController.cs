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
    public class StatesController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StatesController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<StateDto>>> GetAll(CancellationToken cancellationToken)
        {
            var states = await _stateService.GetAllAsync(cancellationToken);
            return Ok(states);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StateDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var state = await _stateService.GetByIdAsync(id, cancellationToken);
            if (state is null)
            {
                return NotFound(CreateErrorResponse($"No existe un estado con el id {id}."));
            }

            return Ok(state);
        }

        [HttpPost]
        public async Task<ActionResult<StateDto>> Create([FromBody] CreateStateDto dto, CancellationToken cancellationToken)
        {
            var created = await _stateService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<StateDto>> Update(int id, [FromBody] UpdateStateDto dto, CancellationToken cancellationToken)
        {
            var updated = await _stateService.UpdateAsync(id, dto, cancellationToken);
            if (updated is null)
            {
                return NotFound(CreateErrorResponse($"No fue posible actualizar el estado porque no existe un estado con el id {id}."));
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _stateService.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound(CreateErrorResponse($"No fue posible eliminar el estado porque no existe un estado con el id {id}."));
            }

            return NoContent();
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
