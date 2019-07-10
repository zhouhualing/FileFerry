
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace WD.Library.Core
{
    [TypeConverter(typeof(DirectoryNameConverter))]
	public sealed class DirectoryName : PathName
	{
		public DirectoryName(string path)
			: base(path)
		{
		}
		public static DirectoryName Create(string DirectoryName)
		{
			if (string.IsNullOrEmpty(DirectoryName))
				return null;
			else
				return new DirectoryName(DirectoryName);
		}

		public DirectoryName Combine(DirectoryName relativePath)
		{
			if (relativePath == null)
				return null;
			return DirectoryName.Create(Path.Combine(normalizedPath, relativePath));
		}
		
		public FileName Combine(FileName relativePath)
		{
			if (relativePath == null)
				return null;
			return FileName.Create(Path.Combine(normalizedPath, relativePath));
		}
		
		public FileName CombineFile(string relativeFileName)
		{
			if (relativeFileName == null)
				return null;
			return FileName.Create(Path.Combine(normalizedPath, relativeFileName));
		}
		
		public DirectoryName CombineDirectory(string relativeDirectoryName)
		{
			if (relativeDirectoryName == null)
				return null;
			return DirectoryName.Create(Path.Combine(normalizedPath, relativeDirectoryName));
		}
		
		public DirectoryName GetRelativePath(DirectoryName path)
		{
			if (path == null)
				return null;
			return DirectoryName.Create(FileUtility.GetRelativePath(normalizedPath, path));
		}

		public FileName GetRelativePath(FileName path)
		{
			if (path == null)
				return null;
			return FileName.Create(FileUtility.GetRelativePath(normalizedPath, path));
		}
		
		public string ToStringWithTrailingBackslash()
		{
			if (normalizedPath.EndsWith("\\", StringComparison.Ordinal))
				return normalizedPath; 
			else
				return normalizedPath + "\\";
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as DirectoryName);
		}
		
		public bool Equals(DirectoryName other)
		{
			if (other != null)
				return string.Equals(normalizedPath, other.normalizedPath, StringComparison.OrdinalIgnoreCase);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(normalizedPath);
		}
		
		public static bool operator ==(DirectoryName left, DirectoryName right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}
		
		public static bool operator !=(DirectoryName left, DirectoryName right)
		{
			return !(left == right);
		}
		
		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator ==(DirectoryName left, string right)
		{
			return (string)left == right;
		}
		
		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator !=(DirectoryName left, string right)
		{
			return (string)left != right;
		}
		
		#endregion
	}
	
	public class DirectoryNameConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(DirectoryName) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string) {
				return DirectoryName.Create((string)value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
		                                 object value, Type destinationType)
		{
			if (destinationType == typeof(string)) {
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
