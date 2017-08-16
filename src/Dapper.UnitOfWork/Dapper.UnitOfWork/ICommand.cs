using System.Data;

namespace Dapper.UnitOfWork
{
    public interface ICommand
    {
        bool RequiresTransaction { get; }
        void Execute(IDbConnection connection, IDbTransaction transaction);
    }

    public interface ICommand<out T>
    {
        bool RequiresTransaction { get; }
        T Execute(IDbConnection connection, IDbTransaction transaction);
    }
}
