using System.Collections.Generic;
using System.Data;
using Dapper;

namespace DataProtectionDatabaseKeyStorage.Business.Repositories
{
    public class BaseRepository
    {
        protected readonly IDbConnection _connection;

        public BaseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public virtual int? Execute(string sql, object parameters = null)
        {
            return _connection.Execute(sql, parameters);
        }

        public virtual IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            return _connection.Query<T>(sql, parameters);
        }
    }
}