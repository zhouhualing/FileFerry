﻿/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：ceb33349-e711-4855-91b7-23e71ede2da4
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：CbGeneric
 * 创建时间：2017/2/20 16:01:53
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFF.Helper
{
    public delegate void CbGeneric<T>(T obj);

    public delegate void CbGeneric<T1, T2>(T1 obj1, T2 obj2);
}
