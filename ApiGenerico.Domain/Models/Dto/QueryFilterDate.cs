using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Dto
{
    public class QueryFilterDate
    {
        public int PageNumber { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFinal { get; set; }
    }
}
