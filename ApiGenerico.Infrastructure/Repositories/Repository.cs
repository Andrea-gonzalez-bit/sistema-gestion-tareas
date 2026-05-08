using Dapper;
using ApiGenerico.Infrastructure.Repositories._UnitOfWork;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGenerico.Infrastructure.Repositories
{
    public class Repository : IRepository
    {
        protected const string _errUnitOfWork = "Name: No hay un objeto de conexion presente en la actual transaccion. La propiedad _unitOfWork aparece nula";
        protected IUnitOfWork _unitOfWork { get; set; }

        #region Constructor

        public Repository()
        {

        }

        public Repository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion Constructor

        #region Metodos

        public void SetUnitOfWork(IUnitOfWork uow)
        {
            this._unitOfWork = uow;
        }

        /// <summary>
        /// Obtiene un dato de la base de datos, utilizando parametros.
        /// </summary>
        public T Get<T>(string query, DynamicParameters @params)
        {
            if (this._unitOfWork == null)
                throw new System.NullReferenceException(_errUnitOfWork);

            return _unitOfWork.Connection.QueryFirstOrDefault<T>(query, @params, transaction: _unitOfWork.Transaction);
        }

        /// <summary>
        /// Obtiene  un dato de la base de datos, sin utilizar parametros.
        /// </summary>
        public T Get<T>(string query)
        {
            return this.Get<T>(query, null);
        }


        public DateTime GetServerDate()
        {
            return this.Get<DateTime>("SELECT GETDATE()", null);
        }

        /// <summary>
        /// Obtiene una lista de datos, utilizando parametros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string query, DynamicParameters @params)
        {
            if (this._unitOfWork == null)
                throw new System.NullReferenceException(_errUnitOfWork);
            return _unitOfWork.Connection.Query<T>(query, @params, transaction: _unitOfWork.Transaction).AsList();
        }


        public List<dynamic> GetList(string query, DynamicParameters @params)
        {
            if (this._unitOfWork == null)
                throw new System.NullReferenceException(_errUnitOfWork);
            return _unitOfWork.Connection.Query(query, @params, transaction: _unitOfWork.Transaction).AsList();
        }

        /// <summary>
        /// Obtiene una lista de datos, sin utilizar parametros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string query)
        {
            return this.GetList<T>(query, null);
        }

        /// <summary>
        /// Obtiene datos de un Stored Procedure utilizando parametros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public T GetDataOfProcedure<T>(string query, DynamicParameters @params)
        {
            if (this._unitOfWork == null)
                throw new System.NullReferenceException(_errUnitOfWork);
            return _unitOfWork.Connection.QueryFirstOrDefault<T>(query, @params, transaction: _unitOfWork.Transaction, commandType: System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// Obtiene datos de un Stored Procedure sin parametros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public T GetDataOfProcedure<T>(string query)
        {
            return this.GetDataOfProcedure<T>(query, null);
        }

        /// <summary>
        /// Obtiene una lista de datos de un Stored Procedure utilizando parámetros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public List<T> GetDataListOfProcedure<T>(string query, DynamicParameters @params)
        {
            if (this._unitOfWork == null)
                throw new System.NullReferenceException(_errUnitOfWork);
            return _unitOfWork.Connection.Query<T>(query, @params, transaction: _unitOfWork.Transaction, commandType: System.Data.CommandType.StoredProcedure).AsList();
        }

        /// <summary>
        /// Obtiene una lista de datos de un Stored Procedure sin parámetros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> GetDataListOfProcedure<T>(string query)
        {
            return this.GetDataListOfProcedure<T>(query, null);
        }

        /// <summary>
        /// Devuelve la existencia de un dato.
        /// </summary>
        /// <param name="pTabla"></param>
        /// <param name="pWhere"></param>
        /// <returns></returns>
        public bool GetExists(string tabla, string where, DynamicParameters @params)
        {
            try
            {
                string sql =
                    string.Format("SELECT TOP 1 CAST(CASE WHEN EXISTS(SELECT 1 FROM {0} WHERE {1}) THEN 1 ELSE 0 END AS BIT);",
                            tabla, where);

                return this.Get<bool>(sql, @params);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta instruccion sql
        /// </summary>
        ///<returns>Cantidad de registros afectados.</returns>
        public int Execute<T>(string query, DynamicParameters @params)
        {
            if (this._unitOfWork == null)
                throw new System.NullReferenceException(_errUnitOfWork);
            return _unitOfWork.Connection.Execute(query, @params, transaction: _unitOfWork.Transaction);
        }
        #endregion Metodos
    }
}
