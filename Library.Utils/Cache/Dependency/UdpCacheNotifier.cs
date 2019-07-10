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
    /// ͨ��Udp��ʽ������Ϣ����Classʹ���˵���ģʽ��ֱ��ʹ��UdpCacheNotifier.Instance
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
        /// �ڲ�������ʵ����󣬵���ģʽ�ı��ַ�ʽ
        /// </summary>
        public static readonly UdpCacheNotifier Instance = new UdpCacheNotifier();

        private UdpCacheNotifier()
        {
        }

        /// <summary>
        /// ����Cache֪ͨ
        /// </summary>
		/// <param name="notifyDataArray">Cache֪ͨ����</param>
        public void SendNotify(params CacheNotifyData[] notifyDataArray)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(notifyDataArray != null, "notifyDataArray");

            UdpCacheNotifierSettings settings = UdpCacheNotifierSettings.GetConfig();

			List<byte[]> dataList = GetSerializedCacheData(notifyDataArray);

			SendSerializedData(dataList, settings.EndPoints);
        }

		/// <summary>
		/// �첽����Cache֪ͨ
		/// </summary>
		/// <param name="notifyDataArray">Cache֪ͨ����</param>
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
					"Cache Key{0}��֪ͨ���ݰ���СΪ{1}�ֽ�, ����С�ڵ���{2}�ֽڣ����Ե���udpCacheNotifierSettings���ýڵ�packageSize���������������",
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
