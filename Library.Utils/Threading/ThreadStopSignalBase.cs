using System;
using WD.Library.Threading;

namespace WD.Library.Threading
{
    internal abstract class ThreadStopSignalBase : IThreadStopSignal
    {
        protected IDisposable _signal;
        private int _interval;

        public int Interval
        {
            get
            {
                return _interval;
            }

            set
            {
                if (_interval != value)
                {
                    _interval = value;

                    OnIntervalChanged();
                }
            }
        }

        protected virtual void OnIntervalChanged()
        {

        }

        public void Dispose()
        {
            if (_signal == null)
            {
                return;
            }

            _signal.Dispose();
            _signal = null;
        }
        
        public abstract void Build();

        /// <summary>
        /// 等待信号
        /// </summary>
        /// <returns></returns>
        public abstract bool Wait();

        /// <summary>
        /// 发送信号
        /// </summary>
        public abstract void Send();
    }
}
