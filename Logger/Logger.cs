using log4net;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WD.Library.Utils
{
    public class Logger
    {
        private static ILog _logdebug = LogManager.GetLogger("LogDebug");
        private static ILog _loginfo = LogManager.GetLogger("LogInfo");
        private static ILog _logerror = LogManager.GetLogger("LogError");

        public static bool doDebug = true;
        public static bool doInfo = true;
        public static bool doWarn = false;
        public static bool doError = true;
        public static bool doFatal = false;


        public static void Debug(string message)
        {
            LogMessageModel model = new LogMessageModel();
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "DEBUG";
            model.msg = message;
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _logdebug.Debug(JsonHelper.ToJson(model));
        }


        public static void Info(string message)
        {
            LogMessageModel model = new LogMessageModel();
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "INFO";
            model.msg = message;
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _loginfo.Info(JsonHelper.ToJson(model));
        }

        public static void Error(string message)
        {
            LogMessageModel model = new LogMessageModel();
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "ERROR";
            model.msg = message;
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _logerror.Error(JsonHelper.ToJson(model));
        }

        public static void Error(string message, Exception e)
        {
            LogMessageModel model = new LogMessageModel();
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "ERROR";
            model.msg = message + "\n" + e.ToString();
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _logerror.Error(JsonHelper.ToJson(model));
        }


        public static void Debug(LogMessageModel model)
        {
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "DEBUG";
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _logdebug.Debug(JsonHelper.ToJson(model));
        }

        public static void Info(LogMessageModel model)
        {
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "INFO";
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _loginfo.Info(JsonHelper.ToJson(model));
        }


        public static void Error(LogMessageModel model)
        {
            model.traceid = MDC.Get("traceId");
            model.logid = GetLogId();
            model.time = DateTime.Now;
            model.host = GetLocalIP();
            model.level = "ERROR";
            //StackFrame sf = new StackFrame(1);
            //model.stack = string.Format("{0}{1}.{2}", model.version, sf.GetMethod().ReflectedType.FullName, sf.GetMethod().Name);
            _logerror.Error(JsonHelper.ToJson(model));
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


        //TODO hualing
        private static string GetLogId(string prefix = "")
        {
            string logid = prefix;
            if (string.IsNullOrWhiteSpace(logid))
                logid = "WD_";


            try
            {
                DateTime now = DateTime.Now;
                logid += now.Ticks.ToString();
            }
            catch (Exception)
            {
                logid = Guid.NewGuid().ToString();
            }
            return logid;
        }

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
