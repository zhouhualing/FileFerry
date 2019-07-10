///////////////////////////////////////////////////////////////////////////////
// File Name : AppCommand.cs
// Author    : zhou hualing
// Create At : 
// Summary   : 应用程序命令。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Framework.WorkItem
{
    [Serializable]
    public class AppCommand : MarshalByRefObject
    {
        #region Private fields

        private object  _sender;             // 命令发起者
        private string    _cmdID;                // 命令ID
        private AppCommandArgs  _args;       // 命令参数
        
        private volatile int _isCompleted;  // 是否完成
        private Hashtable _results;         // 执行结果
        
        private ThreadMode _threadMode = ThreadMode.None;   // 处理类型；未设置；同步；异步；UI；
        private AppCommandCompleteDelegate _completeHandle; // 操作完成后的处理
        private Control _uiObject;

        private WaitCallback _asyncCallback;

        #endregion


        #region Public Properties

        /// <summary>
        /// 事件发送者
        /// 当需要在UI线程执行处理时，发送者必须继承于System.Windows.Form.Control。
        /// </summary>
        public object Sender
        {
            get { return _sender; }
        }

        /// <summary>
        /// 命令ID
        /// </summary>
        public string CommandID
        {
            get { return _cmdID; }
        }

        /// <summary>
        /// 命令参数
        /// </summary>
        public AppCommandArgs Arguments
        {
            get
            { return _args; }
            set
            { _args = value; }
        }

        /// <summary>
        /// 命令是否完成
        /// </summary>
        public bool IsCompleted
        {
            get { return _isCompleted != 0; }
        }

        #endregion


        #region Constructors

        public AppCommand(object sender, string commandID, AppCommandArgs args)
        {
            _sender = sender;
            _cmdID = commandID;
            _args = args;

            _asyncCallback = new WaitCallback(this.DoAsyncInvoke);
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 这个方法需要在ICommandHandler的实现类进行处理(Excute),返回之前调用。
        /// 需要写到GuideLine文档中。
        /// </summary>
        public void Complete()
        {
            _isCompleted = 1;
            if (ThreadMode.None == _threadMode || null == _completeHandle)
                return;

            try
            {
                switch (_threadMode)
                {
                    case ThreadMode.Sync:
                        _completeHandle(_results);
                        break;

                    case ThreadMode.Async:
                        ThreadPool.QueueUserWorkItem(_asyncCallback);
                        break;

                    case ThreadMode.UI:
                        if (!_uiObject.InvokeRequired)
                            _uiObject.Invoke(_completeHandle, _results);
                        else
                            _completeHandle(_results);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 设置完成时处理委托对象,将同步执行。
        /// </summary>
        /// <param name="handler">处理委托对象</param>
        public void SetCompleteSyncHandler(AppCommandCompleteDelegate handler)
        {
            if (ThreadMode.None != _threadMode)
                throw new CommandException("command complete handler has registered!");

            _threadMode = ThreadMode.Sync;
            _completeHandle = handler;
        }

        /// <summary>
        /// 设置完成时处理委托对象,将异步执行。
        /// </summary>
        /// <param name="handler">处理委托对象</param>
        public void SetCompleteAsyncHandler(AppCommandCompleteDelegate handler)
        {
            if (ThreadMode.None != _threadMode)
                throw new CommandException("command complete handler has registered!");

            _threadMode = ThreadMode.Async;
            _completeHandle = handler;
        }

        /// <summary>
        /// 设置完成时处理委托对象，在UI线程执行。
        /// 要求此前设定的Sender必须继承于System.Windows.Forms.Control，否则将会引发异常。
        /// </summary>
        /// <param name="handler">处理委托对象</param>
        public void SetCompleteUIHandler(AppCommandCompleteDelegate handler, Control uiObject = null)
        {
            if (ThreadMode.None != _threadMode)
                throw new CommandException("command complete handler has registered!");

            _threadMode = ThreadMode.UI;
            _completeHandle = handler;

            _uiObject = (uiObject == null ? WorkItemContainer.UIObject : uiObject);
        }

        /// <summary>
        /// 往Results添加一个参数数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public void AddData(string key, object val)
        {
            try
            {
                if( _results == null )
                    _results = new Hashtable();
            
                _results.Add(key, val);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 从Results获取参数数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object GetData(string key)
        {
            if (_results == null)
                return null;
            else
                return _results[key];
        }

        /// <summary>
        /// 判断是否存在某参数数据
        /// </summary>
        /// <param name="key">参数Key</param>
        /// <returns>存在与否</returns>
        public bool TestData(string key)
        {
            if (_results == null)
                return false;
            else
                return _results.Contains(key);
        }

        /// <summary>
        /// 判断是否存在某参数数据
        /// </summary>
        /// <param name="key">参数Key</param>
        /// <returns>存在与否</returns>
        public bool TestArgumentsData(string key)
        {
            if (_args == null)
                return false;
            else
                return _args.TestData(key);
        }

        #endregion


        #region private methods

        private void DoAsyncInvoke(object state)
        {
            try
            {
                _completeHandle(_results);
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion
    }
}
