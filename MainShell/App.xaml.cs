using Framework.WorkItem;
using GFFClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WD.CorePlugin;
using WD.Library.Core;

namespace LoginSample
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var wi = WorkItemContainer.Instance;
            GlobalService.ServiceContainer.RegisterInstance<MainFormController>(new MainFormController());
            BootStrapper.RunApplication();
        }

    }
}
