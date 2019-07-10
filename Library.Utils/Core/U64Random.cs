using System;

namespace WD.Library.Utils
{
	/// <summary>
	/// Summary description for RandomEx.
	/// </summary>
	public class U64Random :Random
	{
		public U64Random()
		{
		}

        public U64Random(int seed) : base (seed)
        {
        }

        public ulong NextULong()
		{
			ulong high = (ulong)Next();
			high <<= 32;
			ulong low = (ulong) Next();

			return ( high | low );
		}
	}
}
