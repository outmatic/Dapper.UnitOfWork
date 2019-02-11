using System;
using System.Data;
using Dapper.UnitOfWork.Example.Data;
using Dapper.UnitOfWork.Example.Data.Commands;
using Dapper.UnitOfWork.Example.Data.Queries;

namespace Dapper.UnitOfWork.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var factory = new UnitOfWorkFactory(@"Data Source=.\SQLExpress;database=NORTHWND;Integrated Security=true");

			PrintCustomer("ALFKI");

			const string newCustomerId = "OUTM";

			// data manipulation with explicit transaction
			using (var uow = factory.Create(true))
			{
				uow.Execute(new DeleteCustomerCommand(newCustomerId));
				uow.Execute(new AddCustomerCommand(new CustomerEntity
				{
					CustomerId = newCustomerId,
					CompanyName = "Outmatic"
				}));

				uow.Commit(); // don't forget to explicitly commit the transaction, otherwise it will get rolled back
			}

			PrintCustomer(newCustomerId);

			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

			void PrintCustomer(string customerId)
			{
				// simple query
				using (var uow = factory.Create())
				{
					var customer = uow.Query(new GetCustomerByIdQuery(customerId));

					Console.WriteLine($"Retrieved: {customer.CompanyName}");
				}
			}
		}
	}
}
