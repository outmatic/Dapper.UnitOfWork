using System;
using System.Threading.Tasks;

namespace Dapper.UnitOfWork
{
    public class Retry
    {
        private static Task HandleExceptionAsync(RetryOptions retryOptions, Exception ex, int retryCount)
        {
            if (!retryOptions.ExceptionDetector.ShouldRetryOn(ex) || retryCount >= retryOptions.MaxRetries)
                throw ex;

            var sleepTime = TimeSpan.FromMilliseconds(Math.Pow(retryOptions.WaitMillis, retryCount));
            return Task.Delay(sleepTime);
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
                    Task.Run(async() => await HandleExceptionAsync(retryOptions, ex, retryCount));
                }

                retryCount++;
            }

            return default;
        }

        public static async Task<T> DoAsync<T>(Func<Task<T>> func, RetryOptions retryOptions)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (retryOptions?.ExceptionDetector == null)
                return await func();

            var retryCount = 1;
            while (retryCount <= retryOptions.MaxRetries)
            {
                try
                {
                    return await func();
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(retryOptions, ex, retryCount);
                }

                retryCount++;
            }

            return default;
        }

        public static void Do(Action action, RetryOptions retryOptions)
            => Do(() =>
            {
                action();
                return true;
            }, retryOptions);

        public static async Task DoAsync(Func<Task> action, RetryOptions retryOptions)
            => await DoAsync(async () =>
            {
                await action();
                return true;
            }, retryOptions);
    }
}