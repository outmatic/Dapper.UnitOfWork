using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork.Example.Data.Queries
{
    public class GetCustomerByIdQuery : IQuery<CustomerEntity>, IAsyncQuery<CustomerEntity>
    {
        private const string Sql = @"
			SELECT
				CustomerID AS CustomerId,
				CompanyName
			FROM
				Customers
			WHERE
				CustomerID = @customerId
		";

        private readonly string _customerId;

        public GetCustomerByIdQuery(string customerId)
            => _customerId = customerId;

        // this is pure Dapper code
        public CustomerEntity Execute(IDbConnection connection, IDbTransaction transaction)
            => connection.Query<CustomerEntity>(Sql, new
            {
                customerId = _customerId
            }, transaction).FirstOrDefault();

        public Task<CustomerEntity> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
            => connection.QueryFirstOrDefaultAsync<CustomerEntity>(Sql, new
            {
                customerId = _customerId
            }, transaction);
    }
}
