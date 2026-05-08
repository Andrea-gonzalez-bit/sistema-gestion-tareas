using System.ComponentModel.DataAnnotations;

namespace ApiGenerico.Application.Dtos
{
    public class UpdateTaskItemDto
    {
        [Required(ErrorMessage = "El título de la tarea es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres.")]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido para la tarea.")]
        public int StateId { get; set; }
    }
}
