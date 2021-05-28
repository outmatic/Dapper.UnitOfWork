using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, RetryOptions retryOptions = null);
        Task<IUnitOfWork> CreateAsync(bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, RetryOptions retryOptions = null, CancellationToken cancellationToken = default);
    }

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly string _connectionString;

        public UnitOfWorkFactory(string connectionString, DbProviderFactory dbProviderFactory = null)
        {
            _connectionString = connectionString;
            _dbProviderFactory = dbProviderFactory ?? SqlClientFactory.Instance;
        }

        public IUnitOfWork Create(bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, RetryOptions retryOptions = null)
        {
            var conn = BuildConnection();
            conn.Open();

            return new UnitOfWork(conn, transactional, isolationLevel, retryOptions);
        }

        public async Task<IUnitOfWork> CreateAsync(bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, RetryOptions retryOptions = null, CancellationToken cancellationToken = default)
        {
            var conn = BuildConnection();
            await conn.OpenAsync(cancellationToken);

            return new UnitOfWork(conn, transactional, isolationLevel, retryOptions);
        }

        private DbConnection BuildConnection()
        {
            var conn = _dbProviderFactory.CreateConnection();
            if (conn == null)
                throw new Exception("Error initializing connection");

            conn.ConnectionString = _connectionString;

            return conn;
        }
    }
}
