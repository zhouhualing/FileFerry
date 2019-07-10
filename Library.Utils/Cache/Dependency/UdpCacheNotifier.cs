using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using WD.Library.Core;

namespace WD.Library.Caching
{
    /// <summary>
    /// 通过Udp方式发送消息，该Class使用了单件模式，直接使用UdpCacheNotifier.Instance
    /// </summary>
    public sealed class UdpCacheNotifier
    {
		private class SendDataWrapper
		{
			public List<byte[]> DataList;
			public UdpCacheNotifierTargetCollection Targets;

			public SendDataWrapper(List<byte[]> dl, UdpCacheNotifierTargetCollection t)
			{
				DataList = dl;
				Targets = t;
			}
		}

        /// <summary>
        /// 内部构建的实体对象，单件模式的表现方式
        /// </summary>
        public static readonly UdpCacheNotifier Instance = new UdpCacheNotifier();

        private UdpCacheNotifier()
        {
        }

        /// <summary>
        /// 发送Cache通知
        /// </summary>
		/// <param name="notifyDataArray">Cache通知数据</param>
        public void SendNotify(params CacheNotifyData[] notifyDataArray)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(notifyDataArray != null, "notifyDataArray");

            UdpCacheNotifierSettings settings = UdpCacheNotifierSettings.GetConfig();

			List<byte[]> dataList = GetSerializedCacheData(notifyDataArray);

			SendSerializedData(dataList, settings.EndPoints);
        }

		/// <summary>
		/// 异步发送Cache通知
		/// </summary>
		/// <param name="notifyDataArray">Cache通知数据</param>
		public void SendNotifyAsync(params CacheNotifyData[] notifyDataArray)
		{
			SendDataWrapper sdWrapper = new SendDataWrapper(GetSerializedCacheData(notifyDataArray), 
				UdpCacheNotifierSettings.GetConfig().EndPoints);

			ThreadPool.QueueUserWorkItem(new WaitCallback(SendNotifyThreadCallBack), sdWrapper);
		}

		private static void SendNotifyThreadCallBack(object context)
		{
			SendDataWrapper sdWrapper = (SendDataWrapper)context;

			SendSerializedData(sdWrapper.DataList, sdWrapper.Targets);
		}

		private static void SendSerializedData(List<byte[]> serializedData, UdpCacheNotifierTargetCollection targets)
		{
			foreach (byte[] data in serializedData)
			{
				foreach (UdpCacheNotifierTarget endPoint in targets)
				{
					foreach (int port in endPoint.GetPorts())
					{
						using (UdpClient udp = new UdpClient())
						{
							IPEndPoint remoteEndPoint = new IPEndPoint(endPoint.Address, port);

							udp.Connect(remoteEndPoint);

							udp.Send(data, data.Length);
						}
					}
				}
			}
		}

        private static List<byte[]> GetSerializedCacheData(CacheNotifyData[] notifyDataArray)
        {
			ExceptionHelper.FalseThrow<ArgumentNullException>(notifyDataArray != null, "notifyDataArray");

			UdpCacheNotifierSettings settings = UdpCacheNotifierSettings.GetConfig();

			List<byte[]> dataList = new List<byte[]>();

			foreach (CacheNotifyData notifyData in notifyDataArray)
			{
				byte[] data = GetDataBuffer(notifyData);

				ExceptionHelper.FalseThrow(data.Length <= settings.PackageSize,
					"Cache Key{0}的通知数据包大小为{1}字节, 必须小于等于{2}字节，可以调整udpCacheNotifierSettings配置节的packageSize属性来解决此问题",
					notifyData.CacheKey, data.Length, settings.PackageSize);

				dataList.Add(data);
			}

			return dataList;
        }

        private static byte[] GetDataBuffer(CacheNotifyData notifyData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(ms, notifyData);

                return ms.ToArray();
            }
        }
    }
}
