///////////////////////////////////////////////////////////////////////////////
// File Name : AppCommandCompleteDelegate.cs
// Author    : zhou hualing
// Create At :
// Summary   : 委托，用于命令即将完成时的收尾处理
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    /// <summary>
    /// 委托，用于命令即将完成时的收尾处理
    /// </summary>
    /// <param name="results">操作结果</param>
    public delegate void AppCommandCompleteDelegate(Hashtable results); // TODO - to be deleted
    public delegate void AppCommandCompletedEventHandler(AppCommand sender, EventArgs e);
}
