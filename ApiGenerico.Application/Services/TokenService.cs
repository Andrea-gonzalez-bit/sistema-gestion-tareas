using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Models;
using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerico.Application.Services
{
    public class TokenService : _Service, ITokenService
    {
        private readonly SectionConfiguration _config;
        public TokenService(IOptions<SectionConfiguration> Config, IOptions<ConnectionStrings> connectionStrings) : base(connectionStrings.Value.ConnetionToken)
        {
            _config = Config.Value;
        }

        public bool Authentication(string User, string Password)
        {
            JwtUser user = GetUserByUserName(User).Result;
            if (user != null)
            {
                return VerifyPasswordHash(Password, user.Password, user.Salt);
            }
            else
            {
                return false;
            }
        }

        private ResultOperation<JwtUser> GetUserByUserName(string User)
        {
            ResultOperation<JwtUser> result =
                WrapExecuteTrans<ResultOperation<JwtUser>, TokenRepository>((repo, uow) =>
                {
                    ResultOperation<JwtUser> rst = new ResultOperation<JwtUser>();

                    try
                    {
                        rst.Result = repo.GetUserByUserName(User);
                    }
                    catch (Exception err)
                    {
                        rst.RollBack = true;
                        rst.Result = null;
                        rst.MessageExceptionTechnical = err.Message + Environment.NewLine + err.StackTrace;
                    }

                    return rst;
                });

            return result;
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");
            }

            using (System.Security.Cryptography.HMACSHA512 hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public ResultOperation<Claims> GetRolByUser(string User)
        {
            ResultOperation<Claims> result =
                WrapExecuteTrans<ResultOperation<Claims>, TokenRepository>((repo, uow) =>
                {
                    ResultOperation<Claims> rst = new ResultOperation<Claims>();
                    try
                    {
                        rst.Result = repo.GetRolByUser(User);
                    }
                    catch (Exception err)
                    {
                        rst.RollBack = true;
                        rst.Result = null;
                        rst.MessageExceptionTechnical = err.Message + Environment.NewLine + err.StackTrace;
                    }
                    return rst;
                });
            return result;
        }
    }
}
