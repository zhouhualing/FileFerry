///////////////////////////////////////////////////////////////////////////////
// File Name : CommandDispatcher.cs
// Author    : zhou hualing
// Create At :
// Summary   : 命令调度功能类。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Framework.WorkItem
{
    public class CommandDispatcher
    {
        #region Private fields

        private Dictionary<string, CommandHandlerItem> _handlers;

        #endregion


        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdCount">命令的数量</param>
        public CommandDispatcher()
        {
            _handlers = new Dictionary<string, CommandHandlerItem>();
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 发送命令，
        /// 忽略未注册的命令。
        /// </summary>
        /// <param name="appCmd"></param>
        public bool Send(AppCommand appCmd)
        {
            CommandHandlerItem item = null;
            lock (_handlers)
            {
                if (!_handlers.TryGetValue(appCmd.CommandID, out item)) 
                    return false;
            }

            if (null == item)
                return false;

            item.Excute(appCmd);
            return true;
        }

        /// <summary>
        /// 注册同步处理对象
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="handler">处理对象</param>
        /// <param name="force">是否强制注册</param>
        public void RegisterSyncHandler(string[] cmdID, ICommandHandler handler, bool force)
        {
            RegisterHandler(cmdID, handler, force, ThreadMode.Sync, null);
        }

        /// <summary>
        /// 强制注册同步处理对象
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="handler">处理对象</param>
        public void RegisterSyncHandler(string[] cmdID, ICommandHandler handler)
        {
            RegisterHandler(cmdID, handler, true, ThreadMode.Sync, null);
        }

        /// <summary>
        /// 强制注册异步处理对象
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="handler">处理对象</param>
        /// <param name="force">是否强制注册</param>
        public void RegisterAsyncHandler(string[] cmdID, ICommandHandler handler, bool force)
        {
            RegisterHandler(cmdID, handler, force, ThreadMode.Async, null);
        }

        /// <summary>
        /// 注册异步处理对象
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="handler">处理对象</param>
        public void RegisterAsyncHandler(string[] cmdID, ICommandHandler handler)
        {
            RegisterHandler(cmdID, handler, true, ThreadMode.Async, null);
        }

        /// <summary>
        /// 注册UI处理对象
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="handler">处理对象</param>
        /// <param name="force">是否强制注册</param>
        public void RegisterUIHandler(string[] cmdID, ICommandHandler handler, bool force, Control uiObject)
        {
            RegisterHandler(cmdID, handler, force, ThreadMode.UI, uiObject);
        }

        /// <summary>
        /// 注册UI处理对象
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="handler">处理对象</param>
        public void RegisterUIHandler(string[] cmdID, ICommandHandler handler, Control uiObject)
        {
            RegisterHandler(cmdID, handler, true, ThreadMode.UI, uiObject);
        }

        /// <summary>
        /// 注销命令处理对象
        /// </summary>
        /// <param name="cmdID"></param>
        public void UnregisterHandler(ICommandHandler handler)
        {
            lock (_handlers)
            {
                List<string> keys = new List<string>();
                foreach (string key in _handlers.Keys)
                {
                    if (object.ReferenceEquals(_handlers[key].Handler, handler))
                        keys.Add(key);
                }

                foreach (string key in keys)
                    _handlers.Remove(key);
            }
        }

        public void Dispose()
        {
            lock (_handlers)
            {
                _handlers.Clear();
            }
        }

        #endregion


        #region Private methods

        private void RegisterHandler(string[] cmdID, ICommandHandler handler, bool force, 
            ThreadMode mode, Control uiObject)
        {
            if (null == handler)
                throw new CommandException("Handler is invalid.");

            for (int i = 0; i < cmdID.Length; i++)
            {
                if (force)
                {
                    // 强制注册
                    lock (_handlers)
                    {
                        _handlers[cmdID[i]] = new CommandHandlerItem(mode, handler, uiObject);
                    }
                }
                else if (null != _handlers[cmdID[i]])
                {
                    throw new CommandException(string.Format("Duplicated Handler : {0}.", cmdID[i]));
                }
                else
                {
                    // 注册
                    lock (_handlers)
                    {
                        _handlers[cmdID[i]] = new CommandHandlerItem(mode, handler, uiObject);
                    }
                }
            }
        }

        #endregion
    }
}
