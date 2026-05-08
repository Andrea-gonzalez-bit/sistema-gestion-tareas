using ApiGenerico.Domain.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerico.Infrastructure.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        JwtUser GetUserByUserName(string User);
        Claims GetRolByUser(string User);
    }
}
