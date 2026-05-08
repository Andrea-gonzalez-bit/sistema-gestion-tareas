using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Infrastructure.Repositories._UnitOfWork;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerico.Infrastructure.Repositories
{
    public class TokenRepository : Repository, ITokenRepository
    {
        public TokenRepository()
        {

        }

        public TokenRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public JwtUser GetUserByUserName(string User)
        {
            DynamicParameters prms = new DynamicParameters();
            prms.Add("User", User);
            string sql = @"select Login, Password, Salt from Usuarios where Login = @User";
            return Get<JwtUser>(sql, prms);
        }

        public Claims GetRolByUser(string User)
        {
            DynamicParameters prms = new DynamicParameters();
            prms.Add("User", User);
            string sql = @"SELECT U.Nombres,
                            U.Email,
                            UR.Rol
                            FROM Usuarios U
                            INNER JOIN UsuariosRol UR ON UR.IdUsuariosRol = U.Rol
                            WHERE Login = @User";
            return Get<Claims>(sql, prms);
        }
    }
}
