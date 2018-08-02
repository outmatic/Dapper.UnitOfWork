using System;

namespace Dapper.UnitOfWork
{
	public class RetryOptions
	{
		public static RetryOptions Default { get; set; }

		static RetryOptions()
		{
			Default = new RetryOptions(1, 100);
		}

		public int MaxRetries { get; }
		public int WaitMillis { get; }

		public RetryOptions(int maxRetries, int waitMillis)
		{
			if (maxRetries < 1)
				throw new ArgumentOutOfRangeException(nameof(maxRetries), maxRetries, $"{nameof(maxRetries)} cannot be less than 1");
			if (waitMillis < 1)
				throw new ArgumentOutOfRangeException(nameof(waitMillis), waitMillis, $"{nameof(waitMillis)} cannot be less than 1");

			MaxRetries = maxRetries;
			WaitMillis = waitMillis;
		}
	}
}