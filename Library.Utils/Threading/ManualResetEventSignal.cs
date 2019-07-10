using System;
using System.Threading;

namespace WD.Library.Threading
{
    internal class ManualResetEventSignal : ThreadStopSignalBase
    {
        public override bool Wait()
        {
            if (_signal == null) return false;

            bool bStop = ((ManualResetEventSlim)_signal).Wait(this.Interval);

            return bStop;
        }

        public override void Build()
        {
            if (_signal != null) return;

            _signal = new ManualResetEventSlim();
        }

        /// <summary>
        /// 发送信号
        /// </summary>
        public override void Send()
        {
            if (_signal == null) return;

            ((ManualResetEventSlim)_signal).Set();
        }
    }
}
