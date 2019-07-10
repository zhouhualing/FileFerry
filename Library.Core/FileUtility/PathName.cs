
using System;
using System.IO;

namespace WD.Library.Core
{
	public abstract class PathName
	{
		protected readonly string normalizedPath;
		
		protected PathName(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (path.Length == 0)
				throw new ArgumentException("The empty string is not a valid path");
			this.normalizedPath = FileUtility.NormalizePath(path);
		}
		
		protected PathName(PathName path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			this.normalizedPath = path.normalizedPath;
		}
		
		public static implicit operator string(PathName path)
		{
			if (path != null)
				return path.normalizedPath;
			else
				return null;
		}
		
		public override string ToString()
		{
			return normalizedPath;
		}
		
		public bool IsRelative {
			get { return !Path.IsPathRooted(normalizedPath); }
		}
		
		public DirectoryName GetParentDirectory()
		{
			if (normalizedPath.Length < 2 || normalizedPath[1] != ':')
				return DirectoryName.Create(Path.Combine(normalizedPath, ".."));
			else
				return DirectoryName.Create(Path.GetDirectoryName(normalizedPath));
		}
	}
}
