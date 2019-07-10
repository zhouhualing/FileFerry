///////////////////////////////////////////////////////////////////////////////
// File Name : IControllerFactory.cs
// Author    : zhou hualing
// Create At :
// Summary   : Container Factory
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    public interface IContainerFactory
    {
        IPrimaryController GetController();
    }
}
