using System;
using System.Data;

namespace Dapper.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        T Query<T>(IQuery<T> query);
        void Execute(ICommand command);

        T Execute<T>(ICommand<T> command);
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public UnitOfWork(IDbConnection connection, bool transactional = false)
        {
            _connection = connection;
            if (transactional)
            {
                _transaction = connection.BeginTransaction();
            }
        }

        public T Query<T>(IQuery<T> query)
        {
            return query.Execute(_connection, _transaction);
        }

        public void Execute(ICommand command)
        {
            if (command.RequiresTransaction && _transaction == null)
            {
                throw new Exception($"The command {command.GetType()} requires a transaction");
            }

            command.Execute(_connection, _transaction);
        }

        public T Execute<T>(ICommand<T> command)
        {
            if (command.RequiresTransaction && _transaction == null)
            {
                throw new Exception($"The command {command.GetType()} requires a transaction");
            }

            return command.Execute(_connection, _transaction);
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

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
