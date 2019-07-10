using System;

namespace WD.Library.Threading
{
    public class ThreadContext:ICloneable
    {
        public ThreadContext()
        {
            Interval = 0;
            ExecuteSequence = EnumThreadAction.ActionFirst;
            LoopType = EnumThreadSchedule.Once;
            IsBreakLoop = false;
            IsUI = false;
        }

        public object Clone()
        {
            return new ThreadContext
            {
                Interval = this.Interval,
                ExecuteSequence = this.ExecuteSequence,
                LoopType = this.LoopType,
                IsUI = this.IsUI,
                IsBreakLoop = this.IsBreakLoop
            };
        }

        /// <summary>
        /// ms
        /// </summary>
        public int Interval;

        /// <summary>
        /// sequence
        /// </summary>
        public EnumThreadAction ExecuteSequence { get; set; }

        /// <summary>
        /// 循环类型
        /// </summary>
        public EnumThreadSchedule LoopType { get; set; }

        public bool IsUI { get; set; }

        /// <summary>
        /// 是否中断循环
        /// </summary>
        public bool IsBreakLoop { get; set; }
    }
}
