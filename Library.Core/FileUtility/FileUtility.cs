
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;

namespace WD.Library.Core
{
    public class FileNameEventArgs : System.EventArgs
    {
        FileName fileName;

        public FileName FileName
        {
            get
            {
                return fileName;
            }
        }

        public FileNameEventArgs(FileName fileName)
        {
            this.fileName = fileName;
        }

        public FileNameEventArgs(string fileName)
        {
            this.fileName = FileName.Create(fileName);
        }
    }
    public static class FileUtility
	{
        public static event EventHandler<FileNameEventArgs> FileLoaded;
        public static event EventHandler<FileNameEventArgs> FileSaved;

        readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		static string applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;
		const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";
		
		public static string ApplicationRootPath {
			get {
				return applicationRootPath;
			}
			set {
				applicationRootPath = value;
			}
		}
		
		public static bool IsUrl(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			return path.IndexOf("://", StringComparison.Ordinal) > 0;
		}
		
		public static bool IsEqualFileName(FileName fileName1, FileName fileName2)
		{
			return fileName1 == fileName2;
		}
		
		public static string GetCommonBaseDirectory(string dir1, string dir2)
		{
			if (dir1 == null || dir2 == null) return null;
			if (IsUrl(dir1) || IsUrl(dir2)) return null;
			
			dir1 = NormalizePath(dir1);
			dir2 = NormalizePath(dir2);
			
			string[] aPath = dir1.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string[] bPath = dir2.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			StringBuilder result = new StringBuilder();
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx) {
				if (bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase)) {
					if (result.Length > 0) result.Append(Path.DirectorySeparatorChar);
					result.Append(aPath[indx]);
				} else {
					break;
				}
			}
			if (indx == 0)
				return null;
			else
				return result.ToString();
		}		
		
		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			if (string.IsNullOrEmpty(baseDirectoryPath)) {
				return absPath;
			}
			if (IsUrl(absPath) || IsUrl(baseDirectoryPath)){
				return absPath;
			}
			
			baseDirectoryPath = NormalizePath(baseDirectoryPath);
			absPath           = NormalizePath(absPath);
			
			string[] bPath = baseDirectoryPath != "." ? baseDirectoryPath.Split(separators) : new string[0];
			string[] aPath = absPath != "." ? absPath.Split(separators) : new string[0];
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx){
				if(!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
					break;
			}
			
			if (indx == 0 && (Path.IsPathRooted(baseDirectoryPath) || Path.IsPathRooted(absPath))) {
				return absPath;
			}
			
			if(indx == bPath.Length && indx == aPath.Length) {
				return ".";
			}
			StringBuilder erg = new StringBuilder();
			for (int i = indx; i < bPath.Length; ++i) {
				erg.Append("..");
				erg.Append(Path.DirectorySeparatorChar);
			}
			erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length-indx));
			if (erg[erg.Length - 1] == Path.DirectorySeparatorChar)
				erg.Length -= 1;
			return erg.ToString();
		}
		
		/// <summary>
		/// Combines baseDirectoryPath with relPath and normalizes the resulting path.
		/// </summary>
		public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
		{
			return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
		}
		
		public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
		{
			fileName     = NormalizePath(fileName);
			oldDirectory = NormalizePath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			newDirectory = NormalizePath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			if (IsBaseDirectory(oldDirectory, fileName)) {
				if (fileName.Length == oldDirectory.Length) {
					return newDirectory;
				}
				return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
			}
			return fileName;
		}

        public static void CopyDir(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            foreach (string fileName in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
            }
            foreach (string directoryName in Directory.GetDirectories(sourceDirectory))
            {
                CopyDir(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
            }
        }

        static bool IsNotHidden(string dir)
        {
            try
            {
                return (File.GetAttributes(dir) & FileAttributes.Hidden) != FileAttributes.Hidden;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
        
        public static readonly int MaxPathLength = 260;
		
		public static bool IsValidPath(string fileName)
		{

			if (fileName == null || fileName.Length == 0 || fileName.Length >= MaxPathLength) {
				return false;
			}
			
			if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
				return false;
			}
			if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0) {
				return false;
			}
			
			if (!Regex.IsMatch(fileName, fileNameRegEx)) {
				return false;
			}
			
			if(fileName[fileName.Length-1] == ' ') {
				return false;
			}
			
			if(fileName[fileName.Length-1] == '.') {
				return false;
			}
			
			
			string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (nameWithoutExtension != null) {
				nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
			}
			
			if (nameWithoutExtension == "CON" ||
			    nameWithoutExtension == "PRN" ||
			    nameWithoutExtension == "AUX" ||
			    nameWithoutExtension == "NUL") {
				return false;
			}
			
			char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';
			
			return !((nameWithoutExtension.StartsWith("COM", StringComparison.Ordinal) ||
			          nameWithoutExtension.StartsWith("LPT", StringComparison.Ordinal)) &&
			         Char.IsDigit(ch));
		}
		
		public static bool IsValidDirectoryEntryName(string name)
		{
			if (!IsValidPath(name)) {
				return false;
			}
			if (name.IndexOfAny(new char[]{Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar,Path.VolumeSeparatorChar}) >= 0) {
				return false;
			}
			if (name.Trim(' ').Length == 0) {
				return false;
			}
			return true;
		}
		
		public static bool IsDirectory(string filename)
		{
			if (!Directory.Exists(filename)) {
				return false;
			}
			FileAttributes attr = File.GetAttributes(filename);
			return (attr & FileAttributes.Directory) != 0;
		}
		static bool MatchN (string src, int srcidx, string pattern, int patidx)
		{
			int patlen = pattern.Length;
			int srclen = src.Length;
			char next_char;

			for (;;) {
				if (patidx == patlen)
					return (srcidx == srclen);
				next_char = pattern[patidx++];
				if (next_char == '?') {
					if (srcidx == src.Length)
						return false;
					srcidx++;
				}
				else if (next_char != '*') {
					if ((srcidx == src.Length) || (src[srcidx] != next_char))
						return false;
					srcidx++;
				}
				else {
					if (patidx == pattern.Length)
						return true;
					while (srcidx < srclen) {
						if (MatchN(src, srcidx, pattern, patidx))
							return true;
						srcidx++;
					}
					return false;
				}
			}
		}

		static bool Match(string src, string pattern)
		{
			if (pattern[0] == '*') {
				// common case optimization
				int i = pattern.Length;
				int j = src.Length;
				while (--i > 0) {
					if (pattern[i] == '*')
						return MatchN(src, 0, pattern, 0);
					if (j-- == 0)
						return false;
					if ((pattern[i] != src[j]) && (pattern[i] != '?'))
						return false;
				}
				return true;
			}
			return MatchN(src, 0, pattern, 0);
		}

		public static bool MatchesPattern(string filename, string pattern)
		{
			filename = filename.ToUpperInvariant();
			pattern = pattern.ToUpperInvariant();
			string[] patterns = pattern.Split(';');
			foreach (string p in patterns) {
				if (Match(filename, p)) {
					return true;
				}
			}
			return false;
		}        
		
		static void OnFileLoaded(FileNameEventArgs e)
		{
			if (FileLoaded != null) {
				FileLoaded(null, e);
			}
		}
		
		public static void RaiseFileSaved(FileNameEventArgs e)
		{
			if (FileSaved != null) {
				FileSaved(null, e);
			}
		}
        public static string NormalizePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return fileName;

            int i;

            bool isWeb = false;
            for (i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '/' || fileName[i] == '\\')
                    break;
                if (fileName[i] == ':')
                {
                    if (i > 1)
                        isWeb = true;
                    break;
                }
            }

            char outputSeparator = isWeb ? '/' : System.IO.Path.DirectorySeparatorChar;
            bool isRelative;

            StringBuilder result = new StringBuilder();
            if (isWeb == false && fileName.StartsWith(@"\\", StringComparison.Ordinal) || fileName.StartsWith("//", StringComparison.Ordinal))
            {
                i = 2;
                result.Append(outputSeparator);
                isRelative = false;
            }
            else
            {
                i = 0;
                isRelative = !isWeb && (fileName.Length < 2 || fileName[1] != ':');
            }
            int levelsBack = 0;
            int segmentStartPos = i;
            for (; i <= fileName.Length; i++)
            {
                if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
                {
                    int segmentLength = i - segmentStartPos;
                    switch (segmentLength)
                    {
                        case 0:
                            if (isWeb)
                            {
                                result.Append(outputSeparator);
                            }
                            break;
                        case 1:
                            if (fileName[segmentStartPos] != '.')
                            {
                                if (result.Length > 0) result.Append(outputSeparator);
                                result.Append(fileName[segmentStartPos]);
                            }
                            break;
                        case 2:
                            if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
                            {
                                int j;
                                for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--) ;
                                if (j > 0)
                                {
                                    result.Length = j;
                                }
                                else if (isRelative)
                                {
                                    if (result.Length == 0)
                                        levelsBack++;
                                    else
                                        result.Length = 0;
                                }
                                break;
                            }
                            else
                            {
                                // append normal segment
                                goto default;
                            }
                        default:
                            if (result.Length > 0) result.Append(outputSeparator);
                            result.Append(fileName, segmentStartPos, segmentLength);
                            break;
                    }
                    segmentStartPos = i + 1; // remember start position for next segment
                }
            }
            if (isWeb == false)
            {
                if (isRelative)
                {
                    for (int j = 0; j < levelsBack; j++)
                    {
                        result.Insert(0, ".." + outputSeparator);
                    }
                }
                if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
                {
                    result.Length -= 1;
                }
                if (result.Length == 2 && result[1] == ':')
                {
                    result.Append(outputSeparator);
                }
                if (result.Length == 0)
                    return ".";
            }
            return result.ToString();
        }

        public static bool IsEqualFileName(string fileName1, string fileName2)
        {
            return string.Equals(NormalizePath(fileName1),
                                 NormalizePath(fileName2),
                                 StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
        {
            if (baseDirectory == null || testDirectory == null)
                return false;
            baseDirectory = NormalizePath(baseDirectory);
            if (baseDirectory == ".")
                return !Path.IsPathRooted(testDirectory);
            baseDirectory = AddTrailingSeparator(baseDirectory);
            testDirectory = AddTrailingSeparator(NormalizePath(testDirectory));

            return testDirectory.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase);
        }

        static string AddTrailingSeparator(string input)
        {
            if (input[input.Length - 1] == Path.DirectorySeparatorChar)
                return input;
            else
                return input + Path.DirectorySeparatorChar.ToString();
        }
	}
}
