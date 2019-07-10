using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace WD.CorePlugin
{
    /// <summary>
    /// This class can host an instance of inside another
    /// AppDomain.
    /// </summary>
    public sealed class ApplicationHost
    {
        #region CreateDomain
       
        public static AppDomain CreateDomain()
        {
            return AppDomain.CreateDomain("Application.Web.App", null, CreateDomainSetup());
        }

        public static AppDomainSetup CreateDomainSetup()
        {
            AppDomainSetup s = new AppDomainSetup();
            s.ApplicationBase = Path.GetDirectoryName(CurrentAssembly.Location);

            s.ConfigurationFile = CurrentAssembly.Location + ".config";
            s.ApplicationName = "ApplicationWebApp";
            return s;
        }
        #endregion

        #region Static helpers
        internal static Assembly CurrentAssembly
        {
            get
            {
                return typeof(ApplicationHost).Assembly;
            }
        }
        #endregion


        AppDomain appDomain;
        MainStarter helper;

        public ApplicationHost(StartupSettings startup)
        {
            if (startup == null)
            {
                throw new ArgumentNullException("startup");
            }
            this.appDomain = CreateDomain();
            helper = (MainStarter)appDomain.CreateInstanceAndUnwrap(CurrentAssembly.FullName, typeof(MainStarter).FullName);
            helper.InitApplicationCore(startup);
        }
        public ApplicationHost(AppDomain appDomain, StartupSettings startup)
        {
            if (appDomain == null)
            {
                throw new ArgumentNullException("appDomain");
            }
            if (startup == null)
            {
                throw new ArgumentNullException("startup");
            }
            this.appDomain = appDomain;
            helper = (MainStarter)appDomain.CreateInstanceAndUnwrap(CurrentAssembly.FullName, typeof(MainStarter).FullName);
            helper.InitApplicationCore(startup);
        }
    }
}
