using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrackingService.HostedService
{
    public interface ICustomHostedService : IHostedService
    {
        Task StartAsync<T>(T args, CancellationToken cancellationToken) where T : class;
        Task StopAsync(Task task, CancellationTokenSource cancellationToken);
    }
    public class HostedService : ICustomHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask = ExecuteAsync(_cts.Token);

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public Task StartAsync<T>(T args, CancellationToken cancellationToken) where T : class
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask = ExecuteAsync<T>(args, _cts.Token);

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        public async Task StopAsync(Task task, CancellationTokenSource cancellationTokenSource)
        {
            // Stop called without start
            if (task == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            cancellationTokenSource.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(task, Task.Delay(-1, cancellationTokenSource.Token));

            // Throw if cancellation triggered
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        public TaskStatus GetStatusOfTasks()
        {
            return _executingTask?.Status ?? TaskStatus.WaitingToRun;
        }


        // Derived classes should override this and execute a long running method until 
        // cancellation is requested

        public virtual Task ExecuteAsync(CancellationToken cancellationToken) { throw new NotImplementedException("Method not implemented"); }

        public virtual Task ExecuteAsync<T>(T args, CancellationToken cancellationToken) where T : class { throw new NotImplementedException("Method not implemented"); }

    }
}
