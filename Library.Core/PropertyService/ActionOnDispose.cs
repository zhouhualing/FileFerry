
using System;
using System.Diagnostics;
using System.Threading;

namespace WD.Library.Core
{
	public class ActionOnDispose : IDisposable
	{
		Action callback;
		
		public ActionOnDispose(Action callback)
		{
			if (callback == null)
				throw new ArgumentNullException("DisposeCallback");
			this.callback = callback;
		}
		
		public void Dispose()
		{
			Action action = Interlocked.Exchange(ref callback, null);
			if (action != null) {
				action();
			}
		}
	}
}
