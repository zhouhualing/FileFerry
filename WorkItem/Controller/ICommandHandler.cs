///////////////////////////////////////////////////////////////////////////////
// File Name : ICommandHandler.cs
// Author    : zhou hualing
// Create At :
// Summary   : 命令对象处理的基础接口。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Framework.WorkItem
{
    public interface ICommandHandler
    {
        void Excute(AppCommand appCmd);
    }
}
