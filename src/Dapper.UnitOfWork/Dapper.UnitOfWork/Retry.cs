using System;
using System.Threading;

namespace Dapper.UnitOfWork
{
	public class Retry
	{
		private static void HandleException(RetryOptions retryOptions, IExceptionDetector exceptionDetector, Exception ex, int retryCount)
		{
			if (!exceptionDetector.ShouldRetryOn(ex) || retryOptions.MaxRetries <= 0)
			{
				throw ex;
			}

			var sleepTime = TimeSpan.FromMilliseconds(Math.Pow(retryOptions.WaitMillis, retryCount));
			Thread.Sleep(sleepTime);
		}

		public static T Do<T>(Func<T> func, RetryOptions retryOptions, IExceptionDetector exceptionDetector)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			var retryCount = 1;
			while (retryCount <= retryOptions.MaxRetries)
			{
				try
				{
					return func();
				}
				catch (Exception ex)
				{
					HandleException(retryOptions, exceptionDetector, ex, retryCount);
				}

				retryCount++;
			}

			return default(T);
		}

		public static void Do(Action action, RetryOptions retryOptions, IExceptionDetector exceptionDetector)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			var retryCount = 1;
			while (retryCount <= retryOptions.MaxRetries)
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					HandleException(retryOptions, exceptionDetector, ex, retryCount);
				}

				retryCount++;
			}
		}
	}
}