using System.Data.SqlClient;

namespace Dapper.UnitOfWork
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(bool transactional = false);
    }

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly string _connectionString;

        public UnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IUnitOfWork Create(bool transactional)
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return new UnitOfWork(conn, transactional);
        }
    }
}
