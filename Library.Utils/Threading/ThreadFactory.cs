using System;
using System.Threading.Tasks;
using System.Threading;

namespace WD.Library.Threading
{
    public sealed class ThreadFactory
    {
        private static TaskScheduler _mainThreadTaskScheduler = null;

        internal static TaskScheduler MainThreadTaskScheduler
        {
            get
            {
                if (_mainThreadTaskScheduler != null)
                {
                    return _mainThreadTaskScheduler;
                }
                else
                {
                    try
                    {
                        _mainThreadTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    }
                    catch (Exception)
                    {
                    }
                }
                return _mainThreadTaskScheduler;
            }

            set
            {
                _mainThreadTaskScheduler = value;
            }
        }


        public static int GetPoolRunningThreadCount()
        {
            return ThreadExecutor.GetPoolRunningThreadCount();
        }

        /// <summary>
        /// 获取长时间运行线程数量
        /// </summary>
        /// <returns></returns>
        public static int GetLongRunningThreadCount()
        {
            return ThreadExecutor.GetLongRunningThreadCount();
        }

        /// <summary>
        /// 获取线程池最大线程数
        /// </summary>
        /// <returns></returns>

        public static int GetPoolMaxThreadCount()
        {
            // 获取线程池最大线程数
            int MaxWorkerThreads, miot;
            ThreadPool.GetMaxThreads(out MaxWorkerThreads, out miot);

            return MaxWorkerThreads;
        }

        /// <summary>
        /// 线程睡眠
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        /// <summary>
        /// 时间片睡眠
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public static void SleepSlice(int millisecondsTimeout)
        {
            SliceSleep slice = new SliceSleep();
            slice.Calculate(millisecondsTimeout);
            slice.SleepSlice(null);
        }

        /// <summary>
        /// 设置线程池最大线程数
        /// </summary>
        /// <param name="count"></param>
        public static void SetPoolMaxThreadCount(int count)
        {
            // 获取线程池最大线程数  
            int MaxWorkerThreads, miot;
            ThreadPool.GetMaxThreads(out MaxWorkerThreads, out miot);

            if (count > MaxWorkerThreads)
            {
                throw new ArgumentOutOfRangeException("超出最大线程数限制！");
            }

            // 设置线程池最大线程数
            ThreadPool.SetMaxThreads(count, miot);
        }

        #region 线程管理
        /// <summary>
        /// 创建并启动线程
        /// </summary>
        /// <param name="context">上下文：为null，则创建简单线程；否则创建循环线程</param>
        /// <param name="onExecute">业务</param>
        /// <returns></returns>
        public static IThread StartNew(ThreadContext context, Action<ThreadContext> onExecute)
        {
            // 创建线程
            IThread thread = CreateNew(context, onExecute);

            // 启动线程
            thread.Start();

            return thread;
        }

        /// <summary>
        /// 创建并启动简单线程：只执行一次，立即执行
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="onExecute">业务</param>
        /// <returns></returns>
        public static IThread StartNew(Action<ThreadContext> onExecute)
        {
            // 创建线程
            IThread thread = CreateNew(onExecute);

            // 启动线程
            thread.Start();

            return thread;
        }

        /// <summary>
        /// 创建线程
        /// </summary>
        /// <param name="context">上下文：为null，则创建简单线程；否则创建循环线程</param>
        /// <param name="onExecute">业务</param>
        /// <returns></returns>
		public static IThread CreateNew(ThreadContext context, Action<ThreadContext> onExecute)
        {
            // 创建线程
            IThread thread = CreateNew(context);

            // 事件订阅
            thread.OnExecuteEvent += onExecute;

            return thread;
        }

        /// <summary>
        /// 创建简单线程：只执行一次，立即执行
        /// </summary>
        /// <param name="onExecute"></param>
        /// <returns></returns>
		public static IThread CreateNew(Action<ThreadContext> onExecute)
        {
            IThread thread = CreateNew();

            // 事件订阅
            thread.OnExecuteEvent += onExecute;

            return thread;
        }

        /// <summary>
        /// 创建线程
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public static IThread CreateNew(ThreadContext context)
        {
            // 创建线程
            IThread thread = new ThreadExecutor(context);

            return thread;
        }

        /// <summary>
        /// 创建线程（简单）
        /// </summary>
        /// <returns></returns>
        public static IThread CreateNew()
        {
            // 创建简单线程上下文
            ThreadContext context = ContextFactory.GetSingleThreadContext();

            return CreateNew(context);
        }

        /// <summary>
        /// 创建并启动一个UI线程
        /// </summary>
        /// <param name="onExecute">耗时业务</param>
        /// <param name="onUpdateUI">业务结果回调：更新到ui</param>
        /// <param name="context">上下文参数</param>
        public static void StartNew<T>(T context, Action<T> onExecute, Action<T> onUpdateUI) where T : ThreadContext
        {
            StartNew(context, onExecute, onUpdateUI, null);
        }

        /// <summary>
        /// 创建并启动一个UI线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onUpdateUI"></param>
        public static void StartNew<T>(Action<T> onUpdateUI) where T : ThreadContext
        {
            StartNew<T>(null, null, onUpdateUI);
        }

        /// <summary>
        /// 创建并启动一个UI线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onUpdateUI"></param>
        public static void StartNew<T>(T context, Action<T> onUpdateUI) where T : ThreadContext
        {
            StartNew<T>(context, null, onUpdateUI);
        }


        public static void RegisterMainThreadContext(TaskScheduler mainThreadTaskScheduler)
        {
            MainThreadTaskScheduler = mainThreadTaskScheduler;
        }

        public static void StartNew<T>(T context, Action<T> Executing, Action<T> UpdateUI, Action<Exception> ExceptionAction) where T : ThreadContext
        {
            // 耗时业务线程：不阻塞UI
            Task myTask = Task.Factory.StartNew(() =>
            {
                if (Executing != null)
                {
                    Executing(context);
                }
            });

            // 耗时业务异常
            myTask.ContinueWith((t) =>
            {
                //LogHelper.Error("线程执行耗时业务时，异常！", t.Exception.InnerException);
                if (ExceptionAction != null)
                {
                    ExceptionAction(t.Exception.InnerException);
                }
            },
            TaskContinuationOptions.OnlyOnFaulted);

            // UI更新线程：同步阻塞UI，不要太耗时
            Task uiTask = myTask.ContinueWith((t) =>
            {
                if (UpdateUI != null)
                {
                    UpdateUI(context);
                }
            },
            new System.Threading.CancellationToken(),
            TaskContinuationOptions.OnlyOnRanToCompletion,
            MainThreadTaskScheduler);

            // UI业务异常
            uiTask.ContinueWith((t) =>
            {
                //LogHelper.Error("线程更新UI时，异常！", t.Exception.InnerException);
                if (ExceptionAction != null)
                {
                    ExceptionAction(t.Exception.InnerException);
                }
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }

        #endregion

    }
}
