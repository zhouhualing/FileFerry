#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using WD.Library.Core;
using WD.Library.Accessories;
#endregion
namespace WD.Library.Configuration.Accessories
{
    /// <summary>
    /// 路径匹配算法计算过程定义 Context
    /// </summary>
    internal class PathMatchStrategyContext : StrategyContextBase<BestPathMatchStrategyBase, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PathMatchStrategyContext() { }


        /// <summary>
        /// 根据抽象算法类型组织计算过程
        /// </summary>
        /// <returns></returns>
        public override string DoAction()
        {
			IList<KeyValuePair<string, string>> data = innerStrategy.Candidates;
			return innerStrategy.Calculate(data);
        }
    }
}
