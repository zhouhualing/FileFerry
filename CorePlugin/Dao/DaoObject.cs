using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace WD.CorePlugin
{
    public class ObjectList<T> : List<T>, ISerializable
    {
        public ObjectList()
        {
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ObjectList(SerializationInfo info, StreamingContext context)
        {
            SerializeDataObject(info, context);
        }

        protected void SerializeDataObject(SerializationInfo info, StreamingContext context)
        {
            foreach (var obj in info)
            {
                if (obj.Value is JToken)
                {
                    AddJsonToken((JToken)obj.Value);
                }
                Add((T)(object)obj);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Enumerator enu = base.GetEnumerator();
            while (enu.MoveNext())
            {
                object obj = enu.Current;
                Add((T)obj);
            }
        }

        public void AddJsonToken(JToken token)
        {
            if (token is JObject)
            {
                IDaoObject dataObject = new DaoObject();
                dataObject.AddJsonObject((JObject)token);
                Add((T)dataObject);
            }
            else if (token is JArray)
            {
                IList<object> childList = new ObjectList<object>();
                foreach (var obj in (JArray)token)
                {
                    ((ObjectList<object>)childList).AddJsonToken(obj);
                }
                Add((T)childList);
            }
            else if (token is JValue)
            {
                Add((T)((JValue)token).Value);
            }
        }

        public T Get(int index)
        {
            return base[index];
        }
    }

    public class ObjectMap<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// 添加或覆盖键值对。如果不存在该键值，添加；如果不存在该键值，覆盖原有键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put(TKey key, TValue value)
        {
            base[key] = value;
        }

        public TValue Get(TKey key, TValue defaultValue = default(TValue))
        {
            return base.ContainsKey(key) ? base[key] : defaultValue;
        }

        public void AddRange(IDictionary<TKey, TValue> dictionary)
        {
            var enu = dictionary.GetEnumerator();
            while (enu.MoveNext())
            {
                var pair = enu.Current;
                Put(pair.Key, pair.Value);
            }
        }

        public List<TValue> GetValueList()
        {
            var list = new ObjectList<TValue>();
            list.AddRange(Values);
            return list;
        }

        public List<TKey> GetKeyList()
        {
            var list = new ObjectList<TKey>();
            list.AddRange(Keys);
            return list;
        }

    }

    public class DaoObject : IDaoObject
    {
        public const string PROPERTY_KEY_UID = "Id";

        private ObjectMap<string, object> _inputMap = new ObjectMap<string, object>();

        public DaoObject()
        {
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DaoObject(SerializationInfo info, StreamingContext context)
        {
            SerializeDataObject(info, context);
        }

        protected void SerializeDataObject(SerializationInfo info, StreamingContext context)
        {
            foreach (var obj in info)
            {
                SetPropertyValue(obj.Name, obj.Value);
            }
        }

        public IDaoObject GetPropertyDataObject(string fieldName)
        {
            var value = _inputMap.Get(fieldName.ToLower());
            if (value is IDaoObject)
                return value as IDaoObject;
            return null;
        }

        public object GetPropertyValue(string fieldName)
        {
            if (!_inputMap.ContainsKey(fieldName.ToLower()))
            {
                return null;
            }
            return _inputMap.Get(fieldName.ToLower());
        }

        public void SetPropertyValue(string fieldName, object value)
        {
            fieldName = fieldName.ToLower();
            if (value is JObject)
                SetJsonValue(fieldName, (JObject)value);
            else if (value is JArray)
                SetJsonValue(fieldName, (JArray)value);
            else if (value is JValue)
                SetJsonValue(fieldName, (JValue)value);
            else
                _inputMap.Put(fieldName, value);
        }

        public void AddJsonObject(JObject jObject)
        {
            foreach (var obj in jObject)
            {
                SetPropertyValue(obj.Key, obj.Value);
            }
        }

        private void SetJsonValue(string fieldName, JValue value)
        {
            SetPropertyValue(fieldName.ToLower(), value.Value);
        }
        private void SetJsonValue(string fieldName, JObject value)
        {
            fieldName = fieldName.ToLower();
            var dataObject = GetPropertyDataObject(fieldName) as DaoObject;
            if (dataObject == null)
            {
                dataObject = new DaoObject();
                SetPropertyValue(fieldName, dataObject);
            }
            dataObject.AddJsonObject(value);
        }

        private void SetJsonValue(string fieldName, JArray value)
        {
            fieldName = fieldName.ToLower();
            var list = GetPropertyValue(fieldName) as ObjectList<object>;
            if (list == null)
            {
                list = new ObjectList<object>();
                SetPropertyValue(fieldName, list);
            }
            foreach (var obj in value)
            {
                list.AddJsonToken(obj);
            }
        }

        public void RemoveProperty(string fieldName)
        {
            fieldName = fieldName.ToLower();
            if (_inputMap.ContainsKey(fieldName))
            {
                _inputMap.Remove(fieldName);
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetProperties()
        {
            return ((ObjectMap<string, object>)GetValue()).GetEnumerator();
        }

        public object GetValue()
        {
            return _inputMap;
        }

        public void SetValue(object value)
        {
            if (value is ObjectMap<string, object>)
            {
                _inputMap = value as ObjectMap<string, object>;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void SetDictionary(IDictionary dictionary)
        {
            var enu = dictionary.GetEnumerator();
            while (enu.MoveNext())
            {
                var pair = (DictionaryEntry)enu.Current;
                SetPropertyValue((string)pair.Key, pair.Value);
            }
        }

        public string GetKeyPropertyName()
        {
            return PROPERTY_KEY_UID.ToLower();
        }

        public int? GetKeyId()
        {
            return (int) GetPropertyValue(GetKeyPropertyName());
        }

        public void SetKeyId(int keyId)
        {
            SetPropertyValue(GetKeyPropertyName(), keyId);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var property in _inputMap)
            {
                info.AddValue(property.Key, property.Value);
            }
        }

        public int GetPropertiesCount()
        {
            return _inputMap.Count;
        }

        public object GetFirstProperty()
        {
            return _inputMap.Select(property => property.Value).FirstOrDefault();
        }
    }
}
