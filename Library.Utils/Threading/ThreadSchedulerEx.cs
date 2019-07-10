using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WD.Library.Threading
{
    public class ThreadSchedulerEx : TaskScheduler
    {
        private static ThreadSchedulerEx _instance = new ThreadSchedulerEx();

        private ThreadSchedulerEx()
        {

        }

        public static ThreadSchedulerEx Instance
        {
            get
            {
                return _instance;
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }

        protected override void QueueTask(Task task)
        {
            if (task.CreationOptions == TaskCreationOptions.LongRunning)
            {
                var thread = new Thread(() => { TryExecuteTask(task); });
                thread.IsBackground = true; 
                thread.Start();
            }
            else
            {
                // 让Task都在线程池的全局队列中调度
                ThreadPool.QueueUserWorkItem(t => { TryExecuteTask(task); });
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }
    }
}
