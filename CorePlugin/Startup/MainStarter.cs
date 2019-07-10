
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using WD.Library.Core;

namespace WD.CorePlugin
{
    internal sealed class MainStarter : MarshalByRefObject
    {
        private List<object> plugins = null;
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region Initialize Core
        public void InitApplicationCore(StartupSettings properties)
        {

            var container = CoreServiceContainer.Default;
            ServiceSingleton.ServiceProvider = container;
            container.AddFallbackProvider(ServiceSingleton.FallbackServiceProvider);
            container.AddService(typeof(IMessageService), new MessageService());
            ServiceSingleton.ServiceProvider = container;
            CoreStartup startup = new CoreStartup("applicationTest");
            string configDirectory = properties.ConfigDirectory;
            string dataDirectory = properties.DataDirectory;
            string propertiesName;
            if (properties.PropertiesName != null)
            {
                propertiesName = properties.PropertiesName;
            }
            else
            {
                propertiesName = properties.ApplicationName + "Properties";
            }


            if (properties.ApplicationRootPath != null)
            {
                FileUtility.ApplicationRootPath = properties.ApplicationRootPath;
            }

            if (configDirectory == null)
            {
                configDirectory = DirectoryName.Create(dataDirectory ?? Path.Combine(FileUtility.ApplicationRootPath, "config"));
            }

            if(!File.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);
            var propertyService = new PropertyService(
                DirectoryName.Create(configDirectory),
                DirectoryName.Create(dataDirectory ?? Path.Combine(FileUtility.ApplicationRootPath, "data")),
                propertiesName);

            startup.StartCoreServices(propertyService);

            foreach (string file in properties.addInFiles)
            {
                startup.AddAddInFile(file);
            }
            foreach (string dir in properties.addInDirectories)
            {
                startup.AddAddInsFromDirectory(dir);
            }

            if (properties.AllowAddInConfigurationAndExternalAddIns)
            {
                startup.ConfigureExternalAddIns(Path.Combine(configDirectory, "AddIns.xml"));
            }
            if (properties.AllowUserAddIns)
            {
                startup.ConfigureUserAddIns(Path.Combine(configDirectory, "AddInInstallTemp"),
                    Path.Combine(configDirectory, "AddIns"));
            }

            //LoggingService.Info("Loading AddInTree...");
            startup.RunInitialization();


            //LoggingService.Info("Init finished");         

            plugins = AddInTree.BuildItems<object>("/Application/AddIn/Init", null, true);
            foreach (dynamic m in plugins)
            {
                m.Initialize();
            }
        }
        #endregion

    }

    internal class MainLogger
    {

    }
}
