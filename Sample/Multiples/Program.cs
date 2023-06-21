using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Multiples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine("Start");
                Do();
                Console.ReadLine();
            }
        }
        static async void Do()
        {
            List<Action> actions = new List<Action>();

            for (int i = 0; i < 100; i++)
            {
                var l = i;

                actions.Add(() =>
                {
                    //
                    Thread.Sleep(1000);
                    Console.WriteLine(l);
                    if (l % 5 == 0)
                    {
                        throw new Exception("test exception");
                    }
                });
            }

            MultipleTaskManager multipleTaskManager = new MultipleTaskManager(10);
            var objects = await multipleTaskManager.Do(actions);

            Console.WriteLine("object count:" + objects.Count);
            objects.ForEach(o =>
            {
                Console.WriteLine(o.State);
            });
        }
    }
}
public class MultipleTaskManager
{

    private ConcurrentQueue<Action> actionQueue = new ConcurrentQueue<Action>();
    private ConcurrentQueue<Tuple<Task, Action>> taskQueue = new ConcurrentQueue<Tuple<Task, Action>>();

    public ConcurrentQueue<TaskCompleteObject> taskCompleteObjects = new ConcurrentQueue<TaskCompleteObject>();
    public int MaxThreads { get; set; }
    public MultipleTaskManager(int _maxThreads)
    {
        this.MaxThreads = _maxThreads;
    }

    public async Task<List<TaskCompleteObject>> Do(List<Action> actions)
    {
        if (actions == null || actions.Count == 0)
        {
            return taskCompleteObjects.ToList();
        }
        this.PreLoad(actions);
        while (actionQueue.Count > 0)
        {
            while (taskQueue.Count < MaxThreads && actionQueue.Count > 0)
            {
                var action = actionQueue.FirstOrDefault(a => !taskQueue.Any(t => t.Item2.Equals(a)));
                if (action != null)
                {
                    var task = this.StartTask(action);
                    taskQueue.Enqueue(new Tuple<Task, Action>(task, action));
                }
            }
        }

        await Task.WhenAll(taskQueue.Select(t => t.Item1).ToArray());


        var exectime = DateTime.Now.Second + 1;
        while (this.taskCompleteObjects.Count < actions.Count && exectime >= DateTime.Now.Second)
        {
        }

        return this.taskCompleteObjects.ToList();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Task StartTask(Action action)
    {
        Task task = new Task(ac =>
        {
            if (ac != null)
            {
                ((Action)ac)();
            }
        }, action);
        //var task = tf.StartNew(ac =>
        //{
        //    if (ac != null)
        //    {
        //        ((Action)ac)();
        //    }
        //}, action);

        Action<Task, object?> baseOperation = (t, o) =>
        {
            if (t.AsyncState != null)
            {
                var ac = t.AsyncState as Action;
                this.actionQueue.TryDequeue(out ac);
            }
            var tc = taskQueue.Where(t => t.Item1.Equals(t)).FirstOrDefault();
            this.taskQueue.TryDequeue(out tc);
        };

        task.ContinueWith(continueItem =>
        {
            baseOperation(continueItem, continueItem.AsyncState);
            this.taskCompleteObjects.Enqueue(new TaskCompleteObject((Action)continueItem.AsyncState!) { State = TaskCompleteState.Complete });
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

        task.ContinueWith(continueItem =>
        {
            baseOperation(continueItem, continueItem.AsyncState);
            this.taskCompleteObjects.Enqueue(new TaskCompleteObject((Action)continueItem.AsyncState!) { State = TaskCompleteState.Falied, Exception = continueItem.Exception?.InnerExceptions.FirstOrDefault()! });
        }, TaskContinuationOptions.OnlyOnFaulted);

        task.Start();
        return task;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actions"></param>
    private void PreLoad(List<Action> actions)
    {
        actions.ForEach(action =>
        {
            actionQueue.Enqueue(action);
        });
    }
}
/// <summary>
/// 
/// </summary>
public class TaskCompleteObject
{
    public TaskCompleteObject(Action _action)
    {
        this.Action = _action;
    }
    public Action Action {
        get;
        private set;
    }
    public Exception Exception {
        get; set;
    }
    public TaskCompleteState State { get; set; }
}
/// <summary>
/// 
/// </summary>
public enum TaskCompleteState
{
    Complete = 1,
    Falied = 2,
    Cancellation = 3
}