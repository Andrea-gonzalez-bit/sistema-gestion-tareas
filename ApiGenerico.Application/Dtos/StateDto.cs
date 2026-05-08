namespace ApiGenerico.Application.Dtos
{
    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TasksCount { get; set; }
    }
}
