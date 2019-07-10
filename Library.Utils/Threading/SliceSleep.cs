using System;
using System.Threading;

namespace WD.Library.Threading
{
    public class SliceSleep
    {
        private const int MIN_SLEEP_SLICE = 50; // 最小切片（CPU上下文切换的最小时间切片是30毫秒）
        private int _sleepSliceCount;           // 切片数量
        private int _sleepSliceLast;            // 切片余数

        /// <summary>
        /// 时间片计算
        /// </summary>
        /// <param name="interval">单位：毫秒</param>
        public void Calculate(int interval)
        {
            // 切片数量
            _sleepSliceCount = interval / MIN_SLEEP_SLICE;
            // 切片余数
            _sleepSliceLast = interval % MIN_SLEEP_SLICE;
        }

        public void SleepSlice(CancellationTokenSource _signal)
        {
            for (int i = 1; i <= _sleepSliceCount; i++)
            {
                // 停止信号来了吗？
                if (_signal != null && _signal.IsCancellationRequested)
                {
                    return;
                }

                // 睡上一个时间片
                Thread.Sleep(MIN_SLEEP_SLICE);
            }

            // 停止信号来了吗？
            if (_signal != null && _signal.IsCancellationRequested)
            {
                return;
            }

            // 睡眠补齐
            if (_sleepSliceLast > 0)
            {
                Thread.Sleep(_sleepSliceLast);
            }
        }
    }
}
