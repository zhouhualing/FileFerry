using System.Threading;
using WD.Library.Threading;

namespace WD.Library.Threading
{
    internal class AutoResetEventSignal : ThreadStopSignalBase
    {
        public override bool Wait()
        {
            if (_signal == null) return false;

            bool bStop = ((AutoResetEvent)_signal).WaitOne(this.Interval);

            return bStop;
        }

        /// <summary>
        /// 创建停止信号
        /// </summary>
        public override void Build()
        {
            if (_signal != null) return;

            _signal = new AutoResetEvent(false);
        }

        /// <summary>
        /// 发送停止信号
        /// </summary>
        public override void Send()
        {
            if (_signal == null) return;

            ((AutoResetEvent)_signal).Set();
        }
    }
}
