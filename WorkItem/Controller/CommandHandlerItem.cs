///////////////////////////////////////////////////////////////////////////////
// File Name : CommandHandlerItem.cs
// Author    : zhou hualing
// Create At :
// Summary   : 命令处理对象和执行模式的映射。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    internal delegate void CommandHandlerDelegate(AppCommand cmd);

    internal class CommandHandlerItem
    {
        private ICommandHandler _handler = null;
        private ThreadMode _mode = ThreadMode.None;
        private Control _uiObject = null;

        CommandHandlerDelegate _handlerDelegate = null;


        public ThreadMode ThreadMode
        {
            get { return _mode; }
        }

        public ICommandHandler Handler
        {
            get { return _handler; }
        }


        public CommandHandlerItem(ThreadMode mode, ICommandHandler handler, Control uiObject = null)
        {
            Debug.Assert(ThreadMode.None != mode && handler != null);

            _handler = handler;
            _mode = mode;
            _uiObject = (uiObject == null ? WorkItemContainer.UIObject : uiObject);

            _handlerDelegate = new CommandHandlerDelegate(handler.Excute);
        }

        public void Excute(AppCommand appCmd)
        {
            switch (_mode)
            {
                case ThreadMode.Sync :
                    _handler.Excute(appCmd);
                    break;

                case ThreadMode.Async:
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DoAsyncInvoke), appCmd);
                    break;

                case ThreadMode.UI:
                    if (_uiObject == null)
                    {
                        _uiObject = WorkItemContainer.UIObject;
                    }
                    if (!_uiObject.InvokeRequired)
                        _uiObject.Invoke(_handlerDelegate, appCmd);
                    else
                        _handler.Excute(appCmd);
                    break;

                default:
                    break;
            }
        }

        private void DoAsyncInvoke(object appCmd)
        {
            try
            {
                _handler.Excute((AppCommand)appCmd);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
