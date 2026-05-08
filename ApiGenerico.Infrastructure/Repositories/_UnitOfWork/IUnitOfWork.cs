using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ApiGenerico.Infrastructure.Repositories._UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Begin();
        void Commit();
        void Rollback();
    }
}
