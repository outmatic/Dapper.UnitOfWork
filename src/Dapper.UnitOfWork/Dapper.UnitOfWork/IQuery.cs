using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork
{
    public interface IQuery<out T>
    {
        T Execute(IDbConnection connection, IDbTransaction transaction);
    }

    public interface IAsyncQuery<T>
    {
        Task<T> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    }
}
