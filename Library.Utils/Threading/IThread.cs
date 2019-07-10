using System;
using WD.Library.Threading;

namespace WD.Library.Threading
{
    public interface IThread
    {
        event Action<ThreadContext> OnExecuteEvent;
        event Action<Exception> OnExceptionEvent;
        event Action<ThreadContext> OnBeforeStartEvent;
        event Action<ThreadContext> OnAfterStartEvent;
        event Action<ThreadContext> OnBeforeStopEvent;
        event Action<ThreadContext> OnAfterStopEvent;

        /// <summary>
        /// 线程循环周期（单位：毫秒）
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool Start(ThreadContext context);

        /// <summary>
        /// 重启线程
        /// </summary>
        /// <returns></returns>
        bool Restart();

        /// <summary>
        /// 重启线程
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool Restart(ThreadContext context);

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <returns></returns>
        bool Stop();
    }
}
