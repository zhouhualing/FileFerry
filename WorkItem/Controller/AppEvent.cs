///////////////////////////////////////////////////////////////////////////////
// File Name : AppEvent.cs
// Author    : zhou hualing
// Create At :
// Summary   : 应用程序事件。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    [Serializable]
    public class AppEvent:MarshalByRefObject
    {
        #region Private fields

        [NonSerialized]
        private object _sender;
        private string _eventID;
        private IAppEventArgs _eventArgs;

        #endregion


        #region Public Properties

        /// <summary>
        /// Sender
        /// </summary>
        public object Sender
        {
            get
            {
                return _sender;
            }
        }

        /// <summary>
        /// App EventID
        /// </summary>
        public string EventID
        {
            get
            {
                return _eventID;
            }
        }

        /// <summary>
        /// 事件参数
        /// 因定制事件参数而重载时,必须具有[Serializable]属性
        /// </summary>
        public IAppEventArgs EventArgs
        {
            get
            {
                return _eventArgs;
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="eventid">event id</param>
        /// <param name="eventargs">event argument</param>
        public AppEvent(object sender, string eventID, IAppEventArgs eventArgs)
        {
            _sender = sender;
            _eventID = eventID;
            _eventArgs = eventArgs;
        }

        #endregion
    }
}
