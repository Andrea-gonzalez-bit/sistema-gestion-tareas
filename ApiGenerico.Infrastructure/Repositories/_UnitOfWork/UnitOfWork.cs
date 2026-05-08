using System;
using System.Data;

namespace ApiGenerico.Infrastructure.Repositories._UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        #region Propiedades

        private IDbConnection _connection = null;
        private IDbTransaction _transaction = null;
        private Guid _id = Guid.Empty;

        public IDbConnection Connection
        {
            get { return _connection; }
        }
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }
        public Guid Id
        {
            get { return _id; }
        }

        #endregion Propiedades

        #region Constructor

        public UnitOfWork(IDbConnection connection)
        {
            _id = Guid.NewGuid();
            _connection = connection;
        }

        #endregion Constructor

        #region Metodos

        public void Begin()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            //Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            //Dispose();
        }

        public void Dispose()
        {
            //if (_transaction != null)
            //    _transaction.Dispose();
            //_transaction = null;
        }

        #endregion Metodos

    }
}
