
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using WD.Library.Caching;
using WD.Library.Core;

namespace WD.CorePlugin
{
    public static class BootStrapper
    {
        public static void RunApplication()
        {
            //POMMetaManager.Load();
            //var pomVersion = POMEntry.Instance.GetString("Version");
            //ExceptionHelper.AssertNotNull(pomVersion);
            new CachePerformanceCounterInstaller().Install();
            Debug.WriteLine("Starting ...");
            try
            {
                StartupSettings startup = new StartupSettings();

                Assembly exe = typeof(BootStrapper).Assembly;

                startup.ApplicationRootPath = CommonMethods.GetApplicationDir();
                startup.AllowUserAddIns = true;

                string configDirectory = ConfigurationManager.AppSettings["settingsPath"];

                var product = CommonMethods.GetAssemblyProduct(Assembly.GetCallingAssembly());
                var version = CommonMethods.GetProductVersion();
                if (String.IsNullOrEmpty(configDirectory))
                {
                    startup.ConfigDirectory = Path.Combine(FileUtility.ApplicationRootPath, "config");
                }
                else
                {
                    startup.ConfigDirectory = Path.Combine(startup.ApplicationRootPath, configDirectory);
                }
                startup.ApplicationName = CommonMethods.GetAssemblyProduct();


                startup.AddAddInsFromDirectory(Path.Combine(startup.ApplicationRootPath, "AddIns"));


                ApplicationHost host = new ApplicationHost(AppDomain.CurrentDomain, startup);
              
                System.Globalization.CultureInfo info = System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            finally
            {
                
            }
        }
    }
}
