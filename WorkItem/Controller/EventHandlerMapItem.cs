///////////////////////////////////////////////////////////////////////////////
// File Name : EventHandlerMapItem.cs
// Author    : zhou hualing
// Create At :
// Summary   : EventHandler process map
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Framework.WorkItem
{
    internal delegate void EventHandlerDelegate(AppEvent evt);

    internal class EventHandlerUIObject
    {
        private IEventHandler _handler;
        private Control _uiObject;

        private EventHandlerDelegate _eventHandlerDelegate = null;


        public EventHandlerUIObject(IEventHandler handler, Control uiObject = null)
        {
            _handler = handler;
            _uiObject = (uiObject == null ? WorkItemContainer.UIObject : uiObject);

            _eventHandlerDelegate = new EventHandlerDelegate(this._handler.Handle);
        }


        public void Raise(AppEvent appEvent)
        {
            try
            {
                if (_uiObject == null)
                {
                    _uiObject = WorkItemContainer.UIObject;
                }
                if (!_uiObject.InvokeRequired)
                    _uiObject.Invoke(_eventHandlerDelegate, appEvent);
                else
                    this._handler.Handle(appEvent);
            }
            catch (Exception ex)
            {
                
            }
        }

        public bool Compare(IEventHandler handler)
        {
            return Object.ReferenceEquals(_handler, handler);
        }
    }

    internal class EventHandlerMapItem
    {
        #region private fields

        /// <summary>
        /// 这个事件的所有同步处理方法串
        /// </summary>
        private List<IEventHandler> _syncHandlers = new List<IEventHandler>();
        /// <summary>
        /// 这个事件的所有异步处理方法串
        /// </summary>
        private List<IEventHandler> _asyncHandlers = new List<IEventHandler>();
        /// <summary>
        /// 这个事件的所有UI处理方法串
        /// </summary>
        private List<EventHandlerUIObject> _uiHandlers = new List<EventHandlerUIObject>();

        private WaitCallback _asyncCallBack;


        private class WaitCallBackObj
        {
            public IEventHandler _handler;
            public AppEvent _event;

            public WaitCallBackObj(IEventHandler handler, AppEvent evt)
            {
                _handler = handler;
                _event = evt;
            }

            public void Handle()
            {
                _handler.Handle(_event);
            }
        }

        #endregion


        #region Constructors

        public EventHandlerMapItem()
        {
            _asyncCallBack = new WaitCallback(this.DoAsyncInvoke);
        }

        #endregion


        #region public methods

        public void AddHandler(IEventHandler handler, ThreadMode mode,Control uiObject = null)
        {
            switch (mode)
            {
                case ThreadMode.Sync:
                    lock(_syncHandlers)
                        _syncHandlers.Add(handler);
                    break;

                case ThreadMode.Async:
                    lock(_asyncHandlers)
                        _asyncHandlers.Add(handler);
                    break;

                case ThreadMode.UI:
                    lock(_uiHandlers)
                        _uiHandlers.Add(new EventHandlerUIObject(handler, uiObject));
                    break;

                default:
                    break;
            }
        }

        public void RemoveHandler(IEventHandler handler)
        {
            lock(_syncHandlers)
                _syncHandlers.Remove(handler);

            lock(_asyncHandlers)
                _asyncHandlers.Remove(handler);

            lock (_uiHandlers)
            {
                foreach (EventHandlerUIObject obj in _uiHandlers)
                {
                    if (obj.Compare(handler))
                    {
                        _uiHandlers.Remove(obj);
                        return;
                    }
                }
            }
        }

        public void Raise(AppEvent appEvent)
        {
            try
            {
                IEventHandler[] handlers;
                if (_asyncHandlers.Count > 0)
                {
                    lock (_asyncHandlers)
                        handlers = _asyncHandlers.ToArray();

                    foreach (IEventHandler eh in handlers)
                    {
                        ThreadPool.QueueUserWorkItem(_asyncCallBack,
                            new WaitCallBackObj(eh, appEvent));
                    }
                }

                EventHandlerUIObject[] uiHandlers;
                if (_uiHandlers.Count > 0)
                {
                    lock (_uiHandlers)
                        uiHandlers = _uiHandlers.ToArray();

                    foreach (EventHandlerUIObject ehui in uiHandlers)
                    {
                        try
                        {
                            ehui.Raise(appEvent);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }

                if (_syncHandlers.Count > 0)
                {
                    lock (_syncHandlers)
                        handlers = _syncHandlers.ToArray();

                    foreach (IEventHandler eh in handlers)
                    {
                        try
                        {
                            eh.Handle(appEvent);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion


        #region Private methods

        private void DoAsyncInvoke(object state)
        {
            try
            {
                WaitCallBackObj evt = (WaitCallBackObj)state;
                evt.Handle();
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion
    }
}
