namespace WD.Library.Threading
{  
    public class ContextFactory
    {
        public static ThreadContext GetLoopThreadContext(int interval = 0)
        {
            return new ThreadContext()
            {
                Interval = interval,
                LoopType = EnumThreadSchedule.Loop
            };
        }

        public static ThreadContext GetSingleThreadContext()
        {
            return new ThreadContext();
        }

        public static ThreadContext GetUIThreadContext()
        {
            return new ThreadContext() { IsUI = true };
        }
    }
}
