using System;

namespace Dapper.UnitOfWork
{
	public interface IExceptionDetector
	{
		bool ShouldRetryOn(Exception ex);
	}
}