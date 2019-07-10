///////////////////////////////////////////////////////////////////////////////
// File Name : ThreadMode.cs
// Author    : zhou hualing
// Create At :
// Summary   : Thread Mode
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    /// <summary>
    /// 
    /// </summary>
    public enum ThreadMode : byte
    {
        None = 0,
        Sync = 1,
        Async = 2,
        UI= 3
    }
}
