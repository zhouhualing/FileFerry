using System;
using WD.Library.Threading;

namespace WD.Library.Threading
{
    public interface IThreadLifetime
    {
        /// 线程初始化：只执行一次
        /// </summary>
        void OnInit(ThreadContext context);

        /// <summary>
        /// 执行业务
        /// </summary>
        /// <param name="arg"></param>
        void OnExecute(ThreadContext arg);

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="e"></param>
        void OnException(Exception e);

        /// <summary>
        /// 停止前
        /// </summary>
        void OnBeforeStop(ThreadContext context);

        /// <summary>
        /// 停止后
        /// </summary>
        void OnAfterStop(ThreadContext context);

        /// <summary>
        /// 开始前
        /// </summary>
        void OnBeforeStart(ThreadContext context);

        /// <summary>
        /// 开始后
        /// </summary>
        void OnAfterStart(ThreadContext context);
    }
}
