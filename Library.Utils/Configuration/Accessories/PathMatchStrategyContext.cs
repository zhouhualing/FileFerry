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
    /// ·��ƥ���㷨������̶��� Context
    /// </summary>
    internal class PathMatchStrategyContext : StrategyContextBase<BestPathMatchStrategyBase, string>
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        public PathMatchStrategyContext() { }


        /// <summary>
        /// ���ݳ����㷨������֯�������
        /// </summary>
        /// <returns></returns>
        public override string DoAction()
        {
			IList<KeyValuePair<string, string>> data = innerStrategy.Candidates;
			return innerStrategy.Calculate(data);
        }
    }
}
