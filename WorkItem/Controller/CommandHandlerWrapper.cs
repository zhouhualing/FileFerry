///////////////////////////////////////////////////////////////////////////////
// File Name : CommandHandlerWrapper.cs
// Author    : zhou hualing & lizheng
// Create At :
// Summary   : 用于远程命令处理对象的封装类型。
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;


namespace Framework.WorkItem
{
    internal delegate void InvokeCmdDelegate(AppCommand obj);

    public class CommandHandlerWrapper : MarshalByRefObject, ICommandHandler
    {
        #region Private fields

        private ICommandHandler _handler = null;
        private ThreadMode _mode = ThreadMode.None;
        private Control _uiObject = null;

        private InvokeCmdDelegate _cmdHandlerDelegate;

        #endregion


        #region Constructors

        public CommandHandlerWrapper(ICommandHandler handler, ThreadMode mode)
        {
            Debug.Assert(handler != null);
            Debug.Assert(mode == ThreadMode.Async || mode == ThreadMode.Sync);

            _handler = handler;
            _mode = mode;
        }

        public CommandHandlerWrapper(ICommandHandler handler, Control uiObject = null)
        {
            Debug.Assert(handler != null && uiObject != null);

            _handler = handler;
            _mode = ThreadMode.UI;
            _uiObject = (uiObject == null ? WorkItemContainer.UIObject : uiObject);

            _cmdHandlerDelegate = new InvokeCmdDelegate(handler.Excute);
        }

        #endregion


        #region Public methods

        public void Excute(AppCommand appCmd)
        {
            try
            {
                switch (_mode)
                {
                    case ThreadMode.Sync:
                        _handler.Excute(appCmd);
                        break;

                    case ThreadMode.Async:
                        _handler.Excute(appCmd);
                        break;

                    case ThreadMode.UI:

                        if (_uiObject == null)
                        {
                            _uiObject = WorkItemContainer.UIObject;
                        }
                        _uiObject.Invoke(_cmdHandlerDelegate, appCmd);
                        break;

                    default:
                        break;
                }
                appCmd.Complete();
            }
            catch (Exception ex)
            {                
            }
        }

        #endregion
    }
}
