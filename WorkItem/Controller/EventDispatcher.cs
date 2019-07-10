///////////////////////////////////////////////////////////////////////////////
// File Name : EventDispatcher.cs
// Author    : zhou hualing
// Create At :
// Summary   : 事件调度功能类。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;


namespace Framework.WorkItem
{
    public class EventDispatcher
    {
        #region Private fields

        private Dictionary<string, EventHandlerMapItem> _handlers;

        #endregion


        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="count">需要接收的事件的数量</param>
        public EventDispatcher()
        {
            _handlers = new Dictionary<string, EventHandlerMapItem>(32);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 发起事件
        /// </summary>
        /// <param name="appEvent">事件对象</param>
        public bool Raise(AppEvent appEvent)
        {
            EventHandlerMapItem item = null;
            lock (_handlers)
            {
                if (!_handlers.TryGetValue(appEvent.EventID, out item))
                    return false;
            }

            if (null == item)
                return false;

            item.Raise(appEvent);
            return true;
        }

        /// <summary>
        /// 注册事件的同步处理对象
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterSyncHandler(string[] events, IEventHandler handler)
        {
            RegisterHandler(events, handler, ThreadMode.Sync, null);
        }

        /// <summary>
        /// 注册事件的异步处理对象
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterAsyncHandler(string[] events, IEventHandler handler)
        {
            RegisterHandler(events, handler, ThreadMode.Async, null);
        }
        
        /// <summary>
        /// 注册事件的UI处理对象
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterUIHandler(string[] events, IEventHandler handler, Control uiObject = null)
        {
            RegisterHandler(events, handler, ThreadMode.UI, uiObject);
        }

        public void UnregisterHandler(IEventHandler handler)
        {
            if (null == handler)
                throw new EventException("handler is required.");

            lock (_handlers)
            {
                foreach (string key in _handlers.Keys)
                {
                    try
                    {
                        
                        _handlers[key].RemoveHandler(handler);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void Dispose()
        {
            _handlers.Clear();
        }

        #endregion


        #region Private methods

        private void RegisterHandler(string[] events, IEventHandler handler,
            ThreadMode mode, Control uiObject = null)
        {
            if (null == events || events.Length == 0)
            {
                throw new EventException("events is required.");
            }
            if (null == handler)
            {
                throw new EventException("handler is required.");
            }

            lock (_handlers)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    EventHandlerMapItem item = null;
                    if (!_handlers.TryGetValue(events[i], out item)
                        || item == null)
                    {
                        item = new EventHandlerMapItem();
                        _handlers[events[i]] = item;
                    }

                    item.AddHandler(handler, mode, uiObject);
                }
            }
        }

        #endregion
    }
}
