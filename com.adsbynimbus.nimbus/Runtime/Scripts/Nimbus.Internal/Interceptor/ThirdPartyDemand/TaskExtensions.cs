using System.Threading.Tasks;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand
{
    public static class TaskExtensions
    {
        public static async Task<TResult> TimeoutWithResult<TResult>(this Task<TResult> task, int timeoutMs)
        {
            var completed = await Task.WhenAny(task, Task.Delay(timeoutMs));

            if (task.IsFaulted)
                return default(TResult);

            if (completed == task && task.IsCompleted)
            {
                return task.Result;
            }

            return default(TResult);
        }
    }
}