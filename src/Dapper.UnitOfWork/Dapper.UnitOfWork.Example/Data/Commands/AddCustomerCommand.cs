using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork.Example.Data.Commands
{
	public class AddCustomerCommand : ICommand, IAsyncCommand
	{
		private const string Sql = @"
			INSERT INTO Customers(
				CustomerID,
				CompanyName)
			VALUES(
				@customerId,
				@companyName)
		";

		private readonly CustomerEntity _entity;

		// Set this to true prevents invoking the command without an explicit transaction
		public bool RequiresTransaction => false;

        public AddCustomerCommand(CustomerEntity entity)
			=> _entity = entity;

		// this is pure Dapper code
		public void Execute(IDbConnection connection, IDbTransaction transaction)
			=> connection.Execute(Sql, new
				{
					customerId = _entity.CustomerId,
					companyName = _entity.CompanyName
				}, transaction);

        public Task ExecuteAsync(IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
            => connection.ExecuteAsync(Sql, new
            {
                customerId = _entity.CustomerId,
                companyName = _entity.CompanyName
            }, transaction);
    }
}
