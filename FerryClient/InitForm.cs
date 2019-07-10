using CCWin;
using CCWin.SkinControl;
using Framework.WorkItem;
using GFF.Component.Config;
using System;
using WD.Library.Core;

namespace GFFClient
{
    public class MainFormController:IEventHandler,ICommandHandler
    {

        public MainFormController()
        {
            WorkItemContainer.Instance.RegisterEventSyncHandler(new string[] { "ShowMainWindow" }, this);
        }

        public void Excute(AppCommand appCmd)
        {
            throw new NotImplementedException();
        }

        public void Handle(AppEvent appEvent)
        {
            if (appEvent.EventID.Equals("ShowMainWindow"))
            {
                DoLogin();
            }
        }

        private void DoLogin()
        {
            new MainForm(new ChatListSubItem
            {
                NicName = GlobalService.Resolve<IPropertyService>().Get<UserModel>("User", new UserModel()).UserName,
                DisplayName = GlobalService.Resolve<IPropertyService>().Get<UserModel>("User", new UserModel()).UserName,
                PersonalMsg = GlobalService.Resolve<IPropertyService>().Get<UserModel>("User", new UserModel()).UserName
            }).Show();
        }
    }
}