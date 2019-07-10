
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace WD.Library.Core
{
    [TypeConverter(typeof(FileNameConverter))]
	public sealed class FileName : PathName, IEquatable<FileName>
	{
		public FileName(string path)
			: base(path)
		{
		}
		
		public static FileName Create(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			else
				return new FileName(fileName);
		}

		public string GetFileName()
		{
			return Path.GetFileName(normalizedPath);
		}
		
		public string GetExtension()
		{
			return Path.GetExtension(normalizedPath);
		}
		
		public bool HasExtension(string extension)
		{
			if (extension == null)
				throw new ArgumentNullException("extension");
			if (extension.Length == 0 || extension[0] != '.')
				throw new ArgumentException("extension must start with '.'");
			return normalizedPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
		}
		
		public string GetFileNameWithoutExtension()
		{
			return Path.GetFileNameWithoutExtension(normalizedPath);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as FileName);
		}
		
		public bool Equals(FileName other)
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
		
		public static bool operator ==(FileName left, FileName right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}
		
		public static bool operator !=(FileName left, FileName right)
		{
			return !(left == right);
		}
		
		#endregion
	}
	
	public class FileNameConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(FileName) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string) {
				return FileName.Create((string)value);
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
