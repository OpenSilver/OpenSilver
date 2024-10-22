using System.Collections.Concurrent;

namespace OpenSilver.Maui;

public class SingleThreadTaskScheduler : TaskScheduler, IDisposable
{
    private readonly Thread _thread;
    private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();

    public SingleThreadTaskScheduler()
    {
        _thread = new Thread(Run)
        {
            IsBackground = true
        };
        _thread.Start();
    }

    private void Run()
    {
        foreach (var task in _tasks.GetConsumingEnumerable())
        {
            TryExecuteTask(task);
        }
    }

    protected override void QueueTask(Task task)
    {
        _tasks.Add(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    protected override IEnumerable<Task> GetScheduledTasks() => _tasks.ToArray();

    public void Dispose()
    {
        _tasks.CompleteAdding();
        _thread.Join();
        _tasks.Dispose();
    }
}