using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD.Library.Utils
{
    public class LoggingService
    {
        private static ILog _logdebug = LogManager.GetLogger("LogDebug");
        private static ILog _loginfo = LogManager.GetLogger("LogInfo");
        private static ILog _logwarn = LogManager.GetLogger("LogWarn");
        private static ILog _logerror = LogManager.GetLogger("LogError");
        private static ILog _logfatal = LogManager.GetLogger("LogFatal");

        public static bool doDebug = true;
        public static bool doInfo = true;
        public static bool doWarn = true;
        public static bool doError = true;
        public static bool doFatal = true;

        public static void Debug(string msg)
        {
            if (!doDebug)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logdebug.Debug(msg);
        }

        public static void Debug(string msg, Exception e)
        {
            if (!doDebug)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logdebug.Debug(msg, e);
        }

        public static void Info(string msg)
        {
            if (!doInfo)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _loginfo.Info(msg);
        }
        public static void Info(string msg, Exception e)
        {
            if (!doInfo)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _loginfo.Info(msg, e);
        }


        public static void Warn(string msg)
        {
            if (!doWarn)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logwarn.Warn(msg);
        }
        public static void Warn(string msg, Exception e)
        {
            if (!doWarn)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logwarn.Warn(msg, e);
        }

        public static void Error(string msg)
        {
            if (!doError)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logerror.Error(msg);
        }

        public static void Error(string msg, Exception e)
        {
            if (!doError)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logerror.Error(msg, e);
        }

        public static void Fatal(string msg)
        {
            if (!doFatal)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logfatal.Fatal(msg);
        }

        public static void Fatal(string msg, Exception e)
        {
            if (!doFatal)
            {
                return;
            }
            StackFrame sf = new StackFrame(1);
            msg = string.Format("{0}.{1}:{2}", sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name, msg);
            _logfatal.Fatal(msg, e);
        }


        public static void DeleteLog(DirectoryInfo[] folderPaths, int days)
        {
            try
            {
                foreach (var folder in folderPaths)
                {
                    DirectoryInfo[] ds = folder.GetDirectories();
                    DateTime today = DateTime.Now.Date;
                    foreach (var item in ds)
                    {
                        DateTime logTime = DateTime.Parse(item.Name);
                        if (logTime <= today.AddDays(-days))
                        {
                            item.Delete(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error("Delete Log error：", ex);
            }
        }

        public static void DeleteLog(int days)
        {
            try
            {
                string[] folders = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);
                List<DirectoryInfo> diList = new List<DirectoryInfo>();
                foreach(string folder in folders)
                {
                    diList.Add(new DirectoryInfo(folder));
                }
                var folderPaths = diList.ToArray();
                DeleteLog(folderPaths, days);
            }
            catch (Exception ex)
            {
                Error("Delete Log error：", ex);
            }
        }
    }
}
