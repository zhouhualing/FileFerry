using System.Threading;
using WD.Library.Threading;

namespace WD.Library.Threading
{
    internal class SliceSleepSignalModel : ThreadStopSignalBase
    {
        // 时间片
        private SliceSleep _SleepSlice = new SliceSleep();
        public override bool Wait()
        {
            if (_signal == null) return false;

            bool bStop = ((CancellationTokenSource)_signal).IsCancellationRequested;

            if (!bStop)
            {
                // wait till flower wilt, if no stop ,continue job
                _SleepSlice.SleepSlice((CancellationTokenSource)_signal);
            }

            return bStop;
        }

        public override void Build()
        {
            if (_signal != null) return;

            _signal = new CancellationTokenSource();
        }

        public override void Send()
        {
            if (_signal == null) return;

            ((CancellationTokenSource)_signal).Cancel();
        }

        protected override void OnIntervalChanged()
        {
            _SleepSlice.Calculate(this.Interval);
        }
    }
}
