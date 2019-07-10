using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace WD.Library.Core
{
    public partial class CommonMethods
    {
        public static string GetApplicationDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory; 
        }

        public static string GetProductVersion()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            return GetAssemblyVersion(ass);
        }

        public static string GetAssemblyVersion(Assembly ass)
        {
            if (ass == null)
                ass = Assembly.GetExecutingAssembly();

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(ass.Location);
            string version = info.ProductVersion.Trim();

            return string.IsNullOrWhiteSpace(version) ? "None" : version;
        }

        public static string Copyright
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return GetAssemblyCopyright(ass);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return GetAssemblyVersion(ass);
            }
        }
        public static string GetAssemblyCopyright(Assembly assembly)
        {
            foreach (Attribute a in assembly.GetCustomAttributes(true))
            {
                if (a is AssemblyCopyrightAttribute)
                {
                    string copyright = (a as AssemblyCopyrightAttribute).Copyright;
                    return copyright;
                }
            }

            return "Copyright 2019.  All rights reserved.";
        }

        public static string GetAssemblyProduct(Assembly assembly)
        {

            foreach (Attribute a in assembly.GetCustomAttributes(true))
            {
                if (a is AssemblyProductAttribute)
                {
                    string product = (a as AssemblyProductAttribute).Product;
                    return product;
                }
            }

            return "FerryApplication";
        }

        public static string GetAssemblyProduct()
        {
            return GetAssemblyProduct(Assembly.GetExecutingAssembly());
        }

        public static string GetFileUrl(string filePath)
        {
            Uri uri = new UriBuilder
            {
                Host = string.Empty,
                Scheme = Uri.UriSchemeFile,
                Path = System.IO.Path.GetFullPath(filePath)
            }.Uri;
            return uri.ToString();
        }


        public static string TimeSpanToString(TimeSpan duration)
        {
            DateTime dateTime = new DateTime(duration.Ticks);
            return dateTime.ToString("HH:mm:ss.ff");
        }

        public static IList MergeList(IList l1, IList l2)
        {
            if (l1 == null)
                throw new ArgumentException("Destination list must not be null.");
            if (l2 == null || l2.Count == 0)
                return l1;
            foreach (object o in l2)
            {
                if (!l1.Contains(o))
                    l1.Add(o);
            }
            return l1;
        }

        public static int SearchDirectory(string entireDirPath, string filemask, ref List<string> filelist)
        {
            string[] dirs;
            try
            {
                dirs = Directory.GetFiles(entireDirPath, filemask);
                foreach (string dir in dirs)
                {
                    filelist.Add(Path.GetFileName(dir));
                }
                return (dirs.Length);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
