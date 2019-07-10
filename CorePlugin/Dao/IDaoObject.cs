using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace WD.CorePlugin
{
    public interface IDaoObject
    {
        IDaoObject GetPropertyDataObject(string fieldName);

        void SetPropertyValue(string fieldName, object value);

        object GetPropertyValue(string fieldName);

        IEnumerator<KeyValuePair<string, object>> GetProperties();

        void SetDictionary(IDictionary dictionary);

        string GetKeyPropertyName();

        int? GetKeyId();

        void SetKeyId(int keyId);

        int GetPropertiesCount();

        object GetFirstProperty();

        void RemoveProperty(string fieldName);

        object GetValue();

        void SetValue(object value);
        void AddJsonObject(JObject jObject);
    }
}
