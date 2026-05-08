using ApiGenerico.Infrastructure.Repositories._UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ApiGenerico.Infrastructure.Context
{
    public sealed class ContextFactory : IDisposable
    {
        IDbConnection _connection = null;
        UnitOfWork _unitOfWork = null;
        private bool openConnection;

        public ContextFactory(string cnx, bool openConnection = true)
        {
            this.openConnection = openConnection;
            if (this.openConnection)
            {
                _connection = new ConnectionFactory().GetOpenConnection(cnx, openConnection);
                _unitOfWork = new UnitOfWork(_connection);
            }
        }

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _connection.Dispose();
        }
    }
}
