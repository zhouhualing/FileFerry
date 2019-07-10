using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace WD.Library.Utils
{
    public static partial class JsonHelper
    {
        /// <summary>
        /// The json serializer
        /// </summary>
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();


        /// <summary>
        /// 将一个对象序列化JSON字符串
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="strTimeFormat">时间格式化转换标准</param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss fff";
            return JsonConvert.SerializeObject(obj, timeFormat);
        }

        /// <summary>
        /// 将一个对象集合序列化JSON字符串集合
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static IList<string> SerializeObjects(IList<object> objects )
        {
            if (objects == null || objects.Count == 0)
            {
                return null;
            }

            IList<string> list = new List<string>();
            foreach (var obj in objects)
            {
                list.Add(Serialize(obj));
            }

            return list;
        }

        /// <summary>
        /// 将JSON字符串反序列化为一个Object对象
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>Object对象</returns>
        public static object Deserialize(string json)
        {
            object obj;
            using (var sr = new StringReader(json))
            {
                obj = JsonSerializer.Deserialize(new JsonTextReader(sr));
            }
            return obj;
        }

        /// <summary>
        /// 将一个JSON字符串集合反序列化为对象集合
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static IList<object> DeserializeJsons(IList<string> jsonList)
        {
            if (jsonList == null || jsonList.Count == 0)
            {
                return null;
            }

            IList<object> list = new List<object>();
            foreach (var json in jsonList)
            {
                list.Add(Deserialize(json));
            }

            return list;
        }

        /// <summary>
        /// 将JSON字符串反序列化为一个指定类型对象
        /// </summary>
        /// <typeparam name="TObj">对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>指定类型对象</returns>
        public static TObj Deserialize<TObj>(string json) where TObj : class
        {
            TObj obj;
            using (var sr = new StringReader(json))
            {
                obj = JsonSerializer.Deserialize(new JsonTextReader(sr), typeof(TObj)) as TObj;
            }
            return obj;
        }

        /// <summary>
        /// 将JSON字符串反序列化为一个指定类型对象
        /// </summary>
        /// <typeparam name="TObj">对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>指定类型对象</returns>
        public static TObj DeserializeOnly<TObj>(string json)
        {
            TObj obj;
            using (var sr = new StringReader(json))
            {
                obj = (TObj)JsonSerializer.Deserialize(new JsonTextReader(sr), typeof(TObj));
            }
            return obj;
        }

        /// <summary>
        /// 将JSON字符串反序列化为一个JObject对象
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>JObject对象</returns>
        public static JObject DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json) as JObject;
        }

        /// <summary>
        /// 将JSON字符串反序列化为一个JArray数组
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>JArray对象</returns>
        public static JArray DeserializeArray(string json)
        {
            return JsonConvert.DeserializeObject(json) as JArray;
        }
    }
}
