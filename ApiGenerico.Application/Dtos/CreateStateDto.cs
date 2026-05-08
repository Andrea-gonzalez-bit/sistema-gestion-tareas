using System.ComponentModel.DataAnnotations;

namespace ApiGenerico.Application.Dtos
{
    public class CreateStateDto
    {
        [Required(ErrorMessage = "El nombre del estado es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre del estado no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}
