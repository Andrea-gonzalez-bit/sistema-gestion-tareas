using System.ComponentModel.DataAnnotations;

namespace ApiGenerico.Application.Dtos
{
    public class TaskQueryDto
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, MaxPageSize)]
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1)
                {
                    _pageSize = 1;
                    return;
                }

                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        public int? StateId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; } = "Id";
        public string? SortDirection { get; set; } = "Asc";
    }
}
