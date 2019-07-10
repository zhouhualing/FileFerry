using System;

namespace WD.Library.Threading
{
    interface IThreadStopSignal : IDisposable
    {
        /// <summary>
        /// 信号周期
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// 等待信号
        /// </summary>
        /// <returns></returns>
        bool Wait();

        /// <summary>
        /// 创建信号
        /// </summary>
        void Build();

        /// <summary>
        /// 发送信号
        /// </summary>
        void Send();
    }
}
