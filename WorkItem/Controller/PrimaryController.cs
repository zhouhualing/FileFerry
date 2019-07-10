///////////////////////////////////////////////////////////////////////////////
// File Name : PrimaryController.cs
// Author    : zhou hualing
// Create At : 
// Summary   : 事件调度功能
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Framework.WorkItem
{
    public abstract class PrimaryController : MarshalByRefObject, IPrimaryController
    {
        #region private fields

        private EventDispatcher _eventBus;
        private CommandDispatcher _commandBus;

        #endregion


        #region Constructors

        public PrimaryController()
        {
            _eventBus = new EventDispatcher();
            _commandBus = new CommandDispatcher();
        }

        #endregion


        #region public methods

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="appCmd"></param>
        public virtual void Send(AppCommand appCmd)
        {
            _commandBus.Send(appCmd);
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="appCmd"></param>
        public virtual void Send(string appCmd)
        {
            AppCommand cmd = new AppCommand(null,appCmd,AppCommandArgs.Empty);
            _commandBus.Send(cmd);
        }

        /// <summary>
        /// 注册命令的同步处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        public virtual void RegisterCmdSyncHandler(string[] cmdID, ICommandHandler handler)
        {
            _commandBus.RegisterSyncHandler(cmdID, handler);
        }

        /// <summary>
        /// 注册命令的异步处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        public virtual void RegisterCmdAsyncHandler(string[] cmdID, ICommandHandler handler)
        {
            _commandBus.RegisterAsyncHandler(cmdID , handler);
        }

        /// <summary>
        /// 注册命令的UI处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        public virtual void RegisterCmdUIHandler(string[] cmdID, ICommandHandler handler, Control uiObject = null)
        {
            _commandBus.RegisterUIHandler(cmdID, handler, uiObject);
        }

        /// <summary>
        /// 注销命令处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        public virtual void UnregisterCmdHandler(ICommandHandler handler)
        {
            _commandBus.UnregisterHandler(handler);
        }

        /// <summary>
        /// 发起事件
        /// </summary>
        /// <param name="appEvent">事件对象</param>
        public virtual void Raise(AppEvent appEvent)
        {
            _eventBus.Raise(appEvent);
        }

        /// <summary>
        /// 发起事件
        /// </summary>
        /// <param name="appEvent">事件对象</param>
        public virtual void Raise(string appEventId)
        {
            AppEvent appEvent = new AppEvent(null,appEventId,AppEventArgs.Empty);

            _eventBus.Raise(appEvent);
        }

        /// <summary>
        /// 注册事件的同步处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        public virtual void RegisterEventSyncHandler(string[] events, IEventHandler handler)
        {
            _eventBus.RegisterSyncHandler(events, handler);
        }

        /// <summary>
        /// 注册事件的异步处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        public virtual void RegisterEventAsyncHandler(string[] events, IEventHandler handler)
        {
            _eventBus.RegisterAsyncHandler(events, handler);
        }

        /// <summary>
        /// 注册事件的UI处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        public virtual void RegisterEventUIHandler(string[] events, IEventHandler handler, Control uiObject = null)
        {
            _eventBus.RegisterUIHandler(events, handler, uiObject);
        }

        /// <summary>
        /// 注消事件的UI处理对象
        /// </summary>
        /// <param name="events"></param>
        /// <param name="handler"></param>
        public virtual void UnregisterEventHandler(IEventHandler handler)
        {
            _eventBus.UnregisterHandler(handler);
        }


        public virtual void Dispose()
        {
            // 提供清除的方法
            _eventBus.Dispose();
            _commandBus.Dispose();
        }

        #endregion
    }
}
