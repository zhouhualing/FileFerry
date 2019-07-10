using System;

namespace WD.Library.Core
{
	public interface IStringTagProvider
	{
		string ProvideString(string tag, StringTagPair[] customTags);
	}
}
