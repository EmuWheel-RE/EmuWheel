#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Forza_EmuWheel;

public class FeederTask : IDisposable
{
    private readonly Feeder _feeder = new Feeder();
    private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
    private readonly Task _task;

    public FeederTask()
    {
        var token = _cancellationSource.Token;
        _task = Task.Factory.StartNew(() => _feeder.PollControllers(InputCollector.GameControllers, token),
            token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void CancelAndWait()
    {
        _cancellationSource.Cancel();
        _task.Wait();
    }

    public void Dispose()
    {
        CancelAndWait();
    }
}