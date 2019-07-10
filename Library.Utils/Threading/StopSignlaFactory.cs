using WD.Library.Threading;

namespace WD.Library.Threading
{
    internal sealed class StopSignlaFactory
    {
        public static IThreadStopSignal CreateNew()
        {
            return new ManualResetEventSignal();
        }
    }
}
