using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		T Query<T>(IQuery<T> query);
        Task<T> QueryAsync<T>(IAsyncQuery<T> query, CancellationToken cancellationToken = default);
		void Execute(ICommand command);
        Task ExecuteAsync(IAsyncCommand command, CancellationToken cancellationToken = default);
		T Execute<T>(ICommand<T> command);
        Task<T> ExecuteAsync<T>(IAsyncCommand<T> command, CancellationToken cancellation = default);
		void Commit();
		void Rollback();
	}

	public class UnitOfWork : IUnitOfWork
	{
		private bool _disposed;
		private IDbConnection _connection;
		private readonly RetryOptions _retryOptions;
		private IDbTransaction _transaction;

        internal UnitOfWork(
			IDbConnection connection,
			bool transactional = false,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
			RetryOptions retryOptions = null)
		{
			_connection = connection;
			_retryOptions = retryOptions;

			if (transactional)
				_transaction = connection.BeginTransaction(isolationLevel);
        }

		public T Query<T>(IQuery<T> query)
			=> Retry.Do(() => query.Execute(_connection, _transaction), _retryOptions);

        public Task<T> QueryAsync<T>(IAsyncQuery<T> query, CancellationToken cancellationToken = default)
            => Retry.DoAsync(() => query.ExecuteAsync(_connection, _transaction, cancellationToken), _retryOptions);

		public void Execute(ICommand command)
		{
			if (command.RequiresTransaction && _transaction == null)
				throw new Exception($"The command {command.GetType()} requires a transaction");

			Retry.Do(() => command.Execute(_connection, _transaction), _retryOptions);
		}

		public T Execute<T>(ICommand<T> command)
		{
			if (command.RequiresTransaction && _transaction == null)
				throw new Exception($"The command {command.GetType()} requires a transaction");
            
			return Retry.Do(() => command.Execute(_connection, _transaction), _retryOptions);
		}

        public Task ExecuteAsync(IAsyncCommand command, CancellationToken cancellationToken = default)
        {
            if (command.RequiresTransaction && _transaction == null)
                throw new Exception($"The command {command.GetType()} requires a transaction");

            return Retry.DoAsync(() => command.ExecuteAsync(_connection, _transaction, cancellationToken), _retryOptions);
        }

        public Task<T> ExecuteAsync<T>(IAsyncCommand<T> command, CancellationToken cancellationToken = default)
        {
            if (command.RequiresTransaction && _transaction == null)
                throw new Exception($"The command {command.GetType()} requires a transaction");
            
            return Retry.DoAsync(() => command.ExecuteAsync(_connection, _transaction, cancellationToken), _retryOptions);
        }

		public void Commit()
			=> _transaction?.Commit();

		public void Rollback()
			=> _transaction?.Rollback();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~UnitOfWork()
			=> Dispose(false);

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				_transaction?.Dispose();
				_connection?.Dispose();
			}

            _transaction = null;
			_connection = null;

			_disposed = true;
		}
	}
}
