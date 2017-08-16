using System.Data;

namespace Dapper.UnitOfWork
{
    public interface IQuery<out T>
    {
        T Execute(IDbConnection connection, IDbTransaction transaction);
    }
}
