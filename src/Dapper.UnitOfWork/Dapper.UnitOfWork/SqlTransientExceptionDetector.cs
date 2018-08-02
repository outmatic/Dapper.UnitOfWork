using System;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper.UnitOfWork
{
	// Adapted from https://github.com/aspnet/EntityFrameworkCore/blob/master/src/EFCore.SqlServer/Storage/Internal/SqlServerTransientExceptionDetector.cs
	public class SqlTransientExceptionDetector : IExceptionDetector
	{
		private static readonly int[] HandledErrorNumbers =
		{
			20,    // The instance of SQL Server you attempted to connect to does not support encryption.
			64,    // A connection was successfully established with the server, but then an error occurred during the login process.
			121,   // The semaphore timeout period has expired
			233,   // The client was unable to establish a connection because of an error during connection initialization process before login.
			1205,  // Deadlock
			10053, // A transport-level error has occurred when receiving results from the server.
			10054, // A transport-level error has occurred when sending the request to the server.
			10060, // A network-related or instance-specific error occurred while establishing a connection to SQL Server.
			10928, // Resource ID: %d. The %s limit for the database is %d and has been reached. For more information,
			10929, // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d.
			40197, // The service has encountered an error processing your request. Please try again.
			40501, // The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
			40613, // Database XXXX on server YYYY is not currently available. Please retry the connection later.
			41301, // Dependency failure: a dependency was taken on another transaction that later failed to commit.
			41302, // The current transaction attempted to update a record that has been updated since the transaction started.
			41305, // The current transaction failed to commit due to a repeatable read validation failure.
			41325, // The current transaction failed to commit due to a serializable validation failure.
			41839, // Transaction exceeded the maximum number of commit dependencies.
			49918, // Cannot process request. Not enough resources to process request.
			49919, // Cannot process create or update request. Too many create or update operations in progress for subscription "%ld".
			49920  // Cannot process request. Too many operations in progress for subscription "%ld".
        };

		public bool ShouldRetryOn(Exception ex)
		{
			if (!(ex is SqlException sqlException))
				return ex is TimeoutException;

			foreach (SqlError err in sqlException.Errors)
			{
				return HandledErrorNumbers.Contains(err.Number);
			}

			return false;
		}
	}
}