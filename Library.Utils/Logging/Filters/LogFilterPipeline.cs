using System;
using System.Collections.Generic;
using System.Text;

using WD.Library.Core;
using WD.Library.Accessories;

namespace WD.Library.Logging
{
    /// <summary>
    /// ʵ��ILogFilter�Ĺܵ���Pipeline��
    /// </summary>
    /// <remarks>
    /// ����LogFilter���϶���
    /// </remarks>
#if DELUXEWORKSTEST
    public class LogFilterPipeline : FilterPipelineBase<ILogFilter, LogEntity>
#else
    internal class LogFilterPipeline : FilterPipelineBase<ILogFilter, LogEntity>
#endif
    {
        internal LogFilterPipeline(List<ILogFilter> filters)
        {
            this.pipeline = filters;
        }

        internal LogFilterPipeline()
        {
            this.pipeline = new List<ILogFilter>();
        }

        /// <summary>
        /// ��Pipeline������ILogFilter����
        /// </summary>
        /// <param name="filter">ILogFilter����</param>
        public override void Add(ILogFilter filter)
        {
            lock (pipeline.GetType())
            {
                pipeline.Add(filter);
            }
        }

        /// <summary>
        /// ��Pipeline���Ƴ�ILogFilterʵ��
        /// </summary>
        /// <param name="filter">ILogFilterʵ��</param>
        /// <remarks>
        public override void Remove(ILogFilter filter)
        {
            lock (pipeline.GetType())
            {
                pipeline.Remove(filter);
            }
        }

        #region �����������
        /// <summary>
        /// �����������ص�����ILogFilterʵ��
        /// </summary>
        /// <param name="index">����</param>
        /// <returns>������ILogFilterʵ��</returns>
        public ILogFilter this[int index]
        {
            get
            {
                return pipeline[index];
            }
        }

        /// <summary>
        /// Pipeline��ILogFilterʵ���ĸ���
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int Length
        {
            get
            {
                return pipeline.Count;
            }
        }
        #endregion

        /// <summary>
        /// ʵ����־���ˣ��ж��Ƿ�ͨ��LogFilterPipeline
        /// </summary>
        /// <param name="log">��־����</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        /// <remarks>
        /// Pipeline��Filter֮���ǡ��롱�Ĺ�ϵ��ֻ�����е�Filter��ͨ��������ͨ��
        /// </remarks>
        public override bool IsMatch(LogEntity log)
        {
            bool passFilters = true;

            if (this.pipeline != null)
            {
                foreach(ILogFilter filter in pipeline)
                {
                    try
                    {
                        bool passed = filter.IsMatch(log);
                        passFilters &= passed;

                        if (false == passFilters)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw new LogException(string.Format("LogFilter:{0}������־ʱʧ�ܣ����������û�ʵ���Ƿ���ȷ", filter.Name), ex);
                    }
                }
            }
            return passFilters;
        }
    }
}