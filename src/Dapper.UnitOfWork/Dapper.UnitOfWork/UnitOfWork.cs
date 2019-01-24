using System;
using System.Data;

namespace Dapper.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		T Query<T>(IQuery<T> query);
		void Execute(ICommand command);
		T Execute<T>(ICommand<T> command);
		void Commit();
		void Rollback();
	}

	public class UnitOfWork : IUnitOfWork
	{
		private bool _disposed;
		private IDbConnection _connection;
		private readonly RetryOptions _retryOptions;
		private IDbTransaction _transaction;
		private readonly IExceptionDetector _exceptionDetector;

		internal UnitOfWork(
			IDbConnection connection,
			bool transactional = false,
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
			RetryOptions retryOptions = null)
		{
			_connection = connection;
			_retryOptions = retryOptions ?? RetryOptions.Default;

			if (transactional)
				_transaction = connection.BeginTransaction(isolationLevel);

			_exceptionDetector = new SqlTransientExceptionDetector();
		}

		public T Query<T>(IQuery<T> query)
			=> Retry.Do(() => query.Execute(_connection, _transaction), _retryOptions, _exceptionDetector);

		public void Execute(ICommand command)
		{
			if (command.RequiresTransaction && _transaction == null)
				throw new Exception($"The command {command.GetType()} requires a transaction");

			Retry.Do(() => command.Execute(_connection, _transaction), _retryOptions, _exceptionDetector);
		}

		public T Execute<T>(ICommand<T> command)
		{
			if (command.RequiresTransaction && _transaction == null)
				throw new Exception($"The command {command.GetType()} requires a transaction");

			return Retry.Do(() => command.Execute(_connection, _transaction), _retryOptions, _exceptionDetector);
		}

		public void Commit()
			=> _transaction?.Commit();

		public void Rollback()
			=>
			_transaction?.Rollback();

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
