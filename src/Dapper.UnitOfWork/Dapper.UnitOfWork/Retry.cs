using System;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork
{
	public class Retry
	{
		private static void HandleException(RetryOptions retryOptions, Exception ex, int retryCount)
		{
			if (!retryOptions.ExceptionDetector.ShouldRetryOn(ex) || retryCount >= retryOptions.MaxRetries)
				throw ex;

			var sleepTime = TimeSpan.FromMilliseconds(Math.Pow(retryOptions.WaitMillis, retryCount));
			Task.Delay(sleepTime).Wait();
		}

		public static T Do<T>(Func<T> func, RetryOptions retryOptions)
		{
			if (func == null)
				throw new ArgumentNullException(nameof(func));

			if (retryOptions?.ExceptionDetector == null)
				return func();

			var retryCount = 1;
			while (retryCount <= retryOptions.MaxRetries)
			{
				try
				{
					return func();
				}
				catch (Exception ex)
				{
					HandleException(retryOptions, ex, retryCount);
				}

				retryCount++;
			}

			return default(T);
		}

		public static void Do(Action action, RetryOptions retryOptions)
			=> Do(() =>
				{
					action();
					return true;
				}, retryOptions);
	}
}