using System;
using System.Threading;
using System.Threading.Tasks;

namespace WD.Library.Threading
{
    public class ThreadExecutor : IThread, IDisposable
    {
        internal ThreadExecutor(ThreadContext context = null, IThreadStopSignal threadStopSignal = null)
        {
            if (context != null)
            {
                this._context = context;
            }
            else
            {
                this._context = ContextFactory.GetLoopThreadContext();
            }
            if (threadStopSignal != null)
            {
                this._stopSignal = threadStopSignal;
            }
            else
            {

                this._stopSignal = StopSignlaFactory.CreateNew();
                this._stopSignal.Interval = this._context.Interval;
            }
        }

        ~ThreadExecutor()
        {
            Dispose(false);
        }

        #region 事件
        public event Action<ThreadContext> OnExecuteEvent;
        public event Action<Exception> OnExceptionEvent;
        public event Action<ThreadContext> OnBeforeStartEvent;
        public event Action<ThreadContext> OnAfterStartEvent;
        public event Action<ThreadContext> OnBeforeStopEvent;
        public event Action<ThreadContext> OnAfterStopEvent;
        #endregion

        #region 线程接口
        /// <summary>
        /// 线程循环周期（单位：毫秒）
        /// </summary>
        public int Interval
        {
            get
            {
                return this._context.Interval;
            }

            set
            {
                // 安全检查
                if (value < 0)
                {
                    value = 0;
                }

                // 更新周期
                Interlocked.Exchange(ref this._context.Interval, value);

                // 通知：信号周期改变了
                _stopSignal.Interval = this._context.Interval;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            // 手动释放资源
            Dispose(true);
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            return Start(this._context);
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Start(ThreadContext context)
        {
            lock (_syncObject)
            {
                // 状态检查：不重复启动
                if (!this.Disposed)
                {
                    return false;
                }

                // 参数检查
                if (context != null)
                {
                    // 更新上下文（必须是引用，不能clone，上下文贯穿生命周期）
                    this._context = context;

                    // 更新周期 
                    this.Interval = this._context.Interval;
                }

                // 通知启动开始
                if (OnBeforeStartEvent != null)
                {
                    OnBeforeStartEvent(this._context);
                }

                // 创建停止信号
                _stopSignal.Build();

                // 创建并运行线程
                _thread = Run(this._context.LoopType);

                // 通知启动结束
                if (OnAfterStartEvent != null)
                {
                    OnAfterStartEvent(this._context);
                }

                return true;
            }
        }

        /// <summary>
        /// 重启线程
        /// </summary>
        /// <returns></returns>
        public bool Restart()
        {
            return Restart(this._context);
        }

        /// <summary>
        /// 重启线程
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Restart(ThreadContext context)
        {
            Stop();

            return Start(context);
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <returns>线程已停止，返回false</returns>
        public bool Stop()
        {
            lock (_syncObject)
            {
                // 状态检查：只能停止已经启动的线程
                if (this.Disposed)
                {
                    return false;
                }

                // 通知停止开始
                if (OnBeforeStopEvent != null)
                {
                    OnBeforeStopEvent(this._context);
                }

                // 发送停止信号
                _stopSignal.Send();

                // 同步等待线程停止完毕
                _thread.Wait();

                // 释放资源
                Dispose();

                return true;
            }
        }
        #endregion

        #region 线程池管理接口
        /// <summary>
        /// 获取长时间运行线程数量
        /// </summary>
        /// <returns></returns>

        public static int GetLongRunningThreadCount()
        {
            return _LongRunningThreadCount;
        }

        /// <summary>
        /// 获取线程池运行线程数量
        /// </summary>
        /// <returns></returns>
        public static int GetPoolRunningThreadCount()
        {
            // 获得最大的线程数量 
            int MaxWorkerThreads, miot;
            ThreadPool.GetMaxThreads(out MaxWorkerThreads, out miot);

            // 获得可用的线程数量  
            int AvailableWorkerThreads, aiot;
            AvailableWorkerThreads = aiot = 0;
            ThreadPool.GetAvailableThreads(out AvailableWorkerThreads, out aiot);

            // 返回线程池中活动的线程数  
            return MaxWorkerThreads - AvailableWorkerThreads;
        }
        #endregion

        #region 上下文
        /// <summary>
        /// 上下文
        /// </summary>
        private ThreadContext _context;
        #endregion

        #region 停止信号
        /// <summary>
        /// 停止信号
        /// </summary>
        private IThreadStopSignal _stopSignal;

        /// <summary>
        /// 释放停止信号
        /// </summary>
        private void DisposeStopSignal()
        {
            if (_stopSignal == null) return;

            _stopSignal.Dispose();
        }
        #endregion

        #region 线程
        private static int _LongRunningThreadCount;

        /// <summary>
        /// 运行线程
        /// </summary>
        /// <param name="loopType"></param>
        private Task Run(EnumThreadSchedule loopType)
        {
            Task thread = null;

            if (loopType == EnumThreadSchedule.Once)
            {
                // 启动线程池线程：延续任务在前面的任务完成后以异步方式运行（防止两个任务运行在通一个线程上，造成阻塞）
                // 使用自定义调度器，将Task都放在线程池的全局队列去运行，而不是局部队列中
                thread = Task.Factory.StartNew(Execute, CancellationToken.None, TaskCreationOptions.None, ThreadSchedulerEx.Instance);
            }
            else
            {
                // 启动长期线程：不占用线程池数量
                thread = Task.Factory.StartNew(Execute, CancellationToken.None, TaskCreationOptions.LongRunning, ThreadSchedulerEx.Instance);
            }

            // 增加计数
            Interlocked.Increment(ref _LongRunningThreadCount);

            return thread;
        }

        /// <summary>
        /// 同步锁
        /// </summary>
        private object _syncObject = new object();

        /// <summary>
        /// 线程
        /// </summary>
        private Task _thread;

        /// <summary>
        /// 执行业务
        /// </summary>
        private void Execute()
        {
            while (true)
            {
                try
                {
                    // 先执行等待，后执行业务
                    if (this._context.ExecuteSequence == EnumThreadAction.WaitFirst)
                    {
                        // 停止信号来了吗？
                        if (_stopSignal.Wait())
                        {
                            break;
                        }
                    }

                    // 初始化                    
                    this._context.IsBreakLoop = false;

                    // 通知上层执行业务
                    NotifyExecute(this._context);

                    // 上层主动中断循环
                    if (this._context.IsBreakLoop)
                    {
                        // 释放资源
                        Dispose();
                        break;
                    }

                    // 更新周期
                    this.Interval = this._context.Interval;

                    // 先执行业务，后执行等待
                    if (this._context.ExecuteSequence == EnumThreadAction.ActionFirst)
                    {
                        // 停止信号来了吗？
                        if (_stopSignal.Wait())
                        {
                            break;
                        }
                    }

                    // 只执行一次
                    if (this._context.LoopType == EnumThreadSchedule.Once)
                    {
                        // 释放资源
                        Dispose();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //LogHelper.Error("线程业务异常！", ex);

                    // 触发异常事件
                    if (OnExceptionEvent != null)
                    {
                        OnExceptionEvent(ex);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 通知上层执行业务
        /// </summary>
        private void NotifyExecute(ThreadContext context)
        {
            if (OnExecuteEvent != null)
            {
                try
                {
                    OnExecuteEvent(context);
                }
                catch (Exception ex)
                {
                    //LogHelper.Error("线程业务异常！", ex);

                    // 触发异常事件
                    if (OnExceptionEvent != null)
                    {
                        OnExceptionEvent(ex);
                    }
                }
            }
        }

        /// <summary>
        /// 是否已经被释放
        /// </summary>
        private bool Disposed { get { return _thread == null; } }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isManual">是否手动触发</param>
        private void Dispose(bool isManual)
        {
            // 状态检查：避免重复释放
            if (!this.Disposed)
            {
                // 释放停止信号（非托管资源）
                DisposeStopSignal();

                // 释放线程（不做dispose，让GC自己处理）
                _thread = null;

                // 减少计数
                Interlocked.Decrement(ref _LongRunningThreadCount);

                // 是否手动释放
                if (isManual)
                {
                    // 告诉GC无需再处理本对象的终结队列了=>不会再调用析构函数
                    System.GC.SuppressFinalize(this);
                }

                // 通知停止结束
                if (OnAfterStopEvent != null)
                {
                    OnAfterStopEvent(this._context);
                }
            }
        }
        #endregion
    }
}
