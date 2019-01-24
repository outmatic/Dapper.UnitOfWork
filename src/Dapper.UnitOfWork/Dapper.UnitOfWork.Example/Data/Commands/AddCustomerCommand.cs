using System.Data;

namespace Dapper.UnitOfWork.Example.Data.Commands
{
	public class AddCustomerCommand : ICommand
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
	}
}
