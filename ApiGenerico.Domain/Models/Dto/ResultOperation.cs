using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerico.Domain.Models.Dto
{
    public class ResultOperation
    {
        public bool stateOperation;
        public string MessageResult;
        public string MessageExceptionUser;
        public string MessageExceptionTechnical;
        public bool RollBack;
    }
    public class ResultOperation<T> : ResultOperation
    {
        public T Result;
        public List<T> Results;
    }
}
