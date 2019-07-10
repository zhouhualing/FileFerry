///////////////////////////////////////////////////////////////////////////////
// File Name : IEventHandler.cs
// Author    : zhou hualing
// Create At :
// Summary   : Interface Type
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    public interface IEventHandler
    {
        /// <summary>
        /// 处理过程
        /// </summary>
        void Handle(AppEvent appEvent);
    }
}
