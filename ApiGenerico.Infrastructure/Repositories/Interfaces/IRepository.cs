using Dapper;
using ApiGenerico.Infrastructure.Repositories._UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGenerico.Infrastructure.Repositories.Interfaces
{
    public interface IRepository
    {
        void SetUnitOfWork(IUnitOfWork uow);
        T Get<T>(string query);
        T Get<T>(string query, DynamicParameters @params);
        List<T> GetList<T>(string query);
        List<T> GetList<T>(string query, DynamicParameters @params);
        T GetDataOfProcedure<T>(string query, DynamicParameters @params);
        T GetDataOfProcedure<T>(string query);
        List<T> GetDataListOfProcedure<T>(string query, DynamicParameters @params);
        List<T> GetDataListOfProcedure<T>(string query);
        bool GetExists(string tabla, string where, DynamicParameters @params);
        int Execute<T>(string query, DynamicParameters @params);
        DateTime GetServerDate();
    }
}
