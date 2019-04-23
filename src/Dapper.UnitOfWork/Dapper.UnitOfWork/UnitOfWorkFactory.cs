using System.Data;
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
		private readonly string _connectionString;

		public UnitOfWorkFactory(string connectionString)
			=> _connectionString = connectionString;

		public IUnitOfWork Create(bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, RetryOptions retryOptions = null)
		{
			var conn = new SqlConnection(_connectionString);
			conn.Open();
			return new UnitOfWork(conn, transactional, isolationLevel, retryOptions);
		}

        public async Task<IUnitOfWork> CreateAsync(bool transactional = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, RetryOptions retryOptions = null, CancellationToken cancellationToken = default)
        {
            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);
            return new UnitOfWork(conn, transactional, isolationLevel, retryOptions);
        }
	}
}
