using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    public interface IPrimaryController
    {
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="appCmd"></param>
        void Send(AppCommand appCmd);

        /// <summary>
        /// 注册命令的同步处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        void RegisterCmdSyncHandler(string[] cmdID, ICommandHandler handler);

        /// <summary>
        /// 注册命令的异步处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        void RegisterCmdAsyncHandler(string[] cmdID, ICommandHandler handler);

        /// <summary>
        /// 注册命令的UI处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        void RegisterCmdUIHandler(string[] cmdID, ICommandHandler handler, Control uiObject);

        /// <summary>
        /// 注销命令处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        void UnregisterCmdHandler(ICommandHandler handler);

        /// <summary>
        /// 发起事件
        /// </summary>
        /// <param name="appEvent">事件对象</param>
        void Raise(AppEvent appEvent);

        /// <summary>
        /// 注册事件的同步处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        void RegisterEventSyncHandler(string[] events, IEventHandler handler);

        /// <summary>
        /// 注册事件的异步处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        void RegisterEventAsyncHandler(string[] events, IEventHandler handler);

        /// <summary>
        /// 注册事件的UI处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        void RegisterEventUIHandler(string[] events, IEventHandler handler, Control uiObject);

        /// <summary>
        /// 注消事件的UI处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        void UnregisterEventHandler(IEventHandler handler);
    }
}
