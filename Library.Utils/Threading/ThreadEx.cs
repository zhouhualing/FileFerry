
using System;

namespace WD.Library.Threading
{
    public class LoopThread : ThreadEx
    {
        public LoopThread() : base(EnumThreadAction.ActionFirst, EnumThreadSchedule.Loop, 0)
        {
        }
    }
    public class SingleThread : ThreadEx
    {
        public SingleThread() : base(EnumThreadAction.WaitFirst, EnumThreadSchedule.Once, 0)
        {
        }
    }

    public class UIThread
    {
        public event Action<ThreadContext> Executing;
        public event Action<Exception> ExceptionInvoked;
        public event Action<ThreadContext> UIInvoked;

        public void Start(ThreadContext context)
        {
            ThreadFactory.StartNew(context, NotifyExecute, NotifyUpdateUI, NotifyExeception);
        }

        public virtual void NotifyExecute(ThreadContext context)
        {
            if (Executing != null)
            {
                Executing(context);
            }
        }

        public virtual void NotifyUpdateUI(ThreadContext context)
        {
            if (UIInvoked != null)
            {
                UIInvoked(context);
            }
        }


        public virtual void NotifyExeception(Exception e)
        {
            if (ExceptionInvoked != null)
            {
                ExceptionInvoked(e);
            }
        }
    }

    public class ThreadEx : IThreadLifetime, IThread
    {
        private IThread _thread;

        private ThreadContext _context = new ThreadContext();

        public event Action<ThreadContext> OnExecuteEvent;
        public event Action<Exception> OnExceptionEvent;
        public event Action<ThreadContext> OnBeforeStartEvent;
        public event Action<ThreadContext> OnAfterStartEvent;
        public event Action<ThreadContext> OnBeforeStopEvent;
        public event Action<ThreadContext> OnAfterStopEvent;

        private ThreadEx() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sequence">action order</param>
        /// <param name="loopType">loop type</param>
        /// <param name="interval">ms）</param>
        public ThreadEx(EnumThreadAction sequence = EnumThreadAction.ActionFirst, EnumThreadSchedule loopType = EnumThreadSchedule.Once, int interval = 1000)
        {
            this._context.ExecuteSequence = sequence;
            this._context.LoopType = loopType;
            this._context.Interval = interval;

            OnInit(this._context);
            
            if (this._context.Interval < 0)
            {
                this._context.Interval = 0;
            }

            _thread = ThreadFactory.CreateNew(this._context);
            _thread.OnExecuteEvent += OnExecute;
            _thread.OnExceptionEvent += OnException;
            _thread.OnBeforeStartEvent += OnBeforeStart;
            _thread.OnAfterStartEvent += OnAfterStart;
            _thread.OnBeforeStopEvent += OnBeforeStop;
            _thread.OnAfterStopEvent += OnAfterStop;
        }

        public int Interval
        {
            get
            {
                return _context.Interval;
            }

            set
            {
                _context.Interval = value;
            }
        }

        public virtual void OnInit(ThreadContext context)
        {            

        }

        public virtual void OnExecute(ThreadContext context)
        {
            if (OnExecuteEvent != null)
            {
                OnExecuteEvent(this._context);
            }
        }

        public virtual void OnException(Exception e)
        {
            if (OnExceptionEvent != null)
            {
                OnExceptionEvent(e);
            }
        }

        public virtual void OnBeforeStop(ThreadContext context)
        {
            if (OnBeforeStopEvent != null)
            {
                OnBeforeStopEvent(context);
            }
        }

        public virtual void OnAfterStop(ThreadContext context)
        {
            if (OnAfterStopEvent != null)
            {
                OnAfterStopEvent(context);
            }
        }

        public virtual void OnBeforeStart(ThreadContext context)
        {
            if (OnBeforeStartEvent != null)
            {
                OnBeforeStartEvent(context);
            }
        }

        public virtual void OnAfterStart(ThreadContext context)
        {
            if (OnAfterStartEvent != null)
            {
                OnAfterStartEvent(context);
            }
        }
        public bool Start()
        {
            return Start(this._context);
        }

        public bool Start(ThreadContext context)
        {
            if (_thread == null)
            {
                return false;
            }

            return _thread.Start(context);
        }

        public bool Restart()
        {
            return Restart(this._context);
        }

        public bool Restart(ThreadContext context)
        {
            if (_thread == null)
            {
                return false;
            }

            return _thread.Restart(context);
        }

        public bool Stop()
        {
            if (_thread == null)
            {
                return false;
            }

            return _thread.Stop();
        }
    }
}
