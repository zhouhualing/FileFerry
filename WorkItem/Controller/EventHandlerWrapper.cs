///////////////////////////////////////////////////////////////////////////////
// File Name : EventHandlerWrapper.cs
// Author    : zhou hualing
// Create At :
// Summary   : 用于远程事件处理对象的封装类型。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Framework.WorkItem
{
    internal delegate void InvokeEventDelegate(AppEvent obj);

    public class EventHandlerWrapper : MarshalByRefObject, IEventHandler
    {
        #region Private fields

        private IEventHandler _handler = null;
        private ThreadMode _mode = ThreadMode.None;
        private Control _uiObject = null;

        private InvokeEventDelegate _handlerDelegate;

        #endregion


        #region Constructors

        public EventHandlerWrapper(IEventHandler handler, ThreadMode mode)
        {
            Debug.Assert(handler != null);
            Debug.Assert(mode == ThreadMode.Async || mode == ThreadMode.Sync);

            _handler = handler;
            _mode = mode;
        }

        public EventHandlerWrapper(IEventHandler handler, Control uiObject)
        {
            Debug.Assert(handler != null && uiObject != null);

            _handler = handler;
            _mode = ThreadMode.UI;
            _uiObject = uiObject;

            _handlerDelegate = new InvokeEventDelegate(handler.Handle);
        }

        #endregion


        #region IEventHandler Members

        public void Handle(AppEvent appEvent)
        {
            try
            {
                switch (_mode)
                {
                    case ThreadMode.Sync:
                        _handler.Handle(appEvent);
                        break;

                    case ThreadMode.Async:
                        _handler.Handle(appEvent);
                        break;

                    case ThreadMode.UI:
                        if (_uiObject == null)
                        {
                            _uiObject = WorkItemContainer.UIObject;
                        }
                        _uiObject.Invoke(_handlerDelegate, appEvent);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
