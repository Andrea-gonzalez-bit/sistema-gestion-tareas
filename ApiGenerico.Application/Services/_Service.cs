using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Infrastructure.Context;
using ApiGenerico.Infrastructure.Repositories._UnitOfWork;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGenerico.Application.Services
{
    public class _Service
    {
        private string _Cnx = null;
        public _Service(string BaseCnx) { this._Cnx = BaseCnx; }

        protected virtual string ListParametersToStringParameter (List<int> list)
        {    
        var res = string.Empty;
            for (int i = 0; i < list.Count(); i++)
            {
                res += string.Format(",{0},", list[i]);
            }
            return res;
        }

        protected TResult WrapExecute<TResult, TRepository>(Func<TRepository, IUnitOfWork, TResult> exWp)
        where TRepository : IRepository, new()
        where TResult : ResultOperation, new()
        {
            TResult result = new TResult();
            using (var ctx = new ContextFactory(_Cnx))
            {
                try
                {
                    var repo = new TRepository();
                    repo.SetUnitOfWork(ctx.UnitOfWork);

                    result = exWp(repo, ctx.UnitOfWork);

                }
                catch (Exception ex)
                {
                   
                    result.stateOperation = false;

                    StringBuilder exceptionDetails = new StringBuilder();

                    exceptionDetails.AppendLine($"Message: {ex.Message}");
                    exceptionDetails.AppendLine($"StackTrace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        exceptionDetails.AppendLine($"InnerException Message: {ex.InnerException.Message}");
                        exceptionDetails.AppendLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                    }
                    result.MessageExceptionTechnical = exceptionDetails.ToString();
                }
            }
            return result;
        }

        protected TResult WrapExecuteTrans<TResult, TRepository>(Func<TRepository, IUnitOfWork, TResult> exWp)
        where TRepository : IRepository, new()
        where TResult : ResultOperation, new()
        {
            TResult result = new TResult();
            using (var ctx = new ContextFactory(_Cnx))
            {
                try
                {
                    //ctx.UnitOfWork.Begin();

                    var repo = new TRepository();
                    repo.SetUnitOfWork(ctx.UnitOfWork);

                    result = exWp(repo, ctx.UnitOfWork);
                    if (result.RollBack) { throw new Exception(); }
                    //ctx.UnitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    ctx.UnitOfWork.Rollback();
                    result.stateOperation = false;

                    StringBuilder exceptionDetails = new StringBuilder();

                    exceptionDetails.AppendLine($"Message: {ex.Message}");
                    exceptionDetails.AppendLine($"StackTrace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        exceptionDetails.AppendLine($"InnerException Message: {ex.InnerException.Message}");
                        exceptionDetails.AppendLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                    }
                    result.MessageExceptionTechnical = exceptionDetails.ToString();
                }
            }
            return result;
        }
    }
}
