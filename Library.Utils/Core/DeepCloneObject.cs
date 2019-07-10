using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WD.Library.Utils
{
    public class DeepCloneObject : ICloneable
    {
        public override bool Equals(object other)
        {
            bool result = false;
            if (other.GetType().Equals(base.GetType()))
            {
                result = true;
                Type type = base.GetType();
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldInfo[] array = fields;
                for (int i = 0; i < array.Length; i++)
                {
                    FieldInfo fieldInfo = array[i];
                    object value = fieldInfo.GetValue(this);
                    object value2 = fieldInfo.GetValue(other);
                    if (value is Array && value2 is Array)
                    {
                        if (!this.equal((Array)value, (Array)value2))
                        {
                            result = false;
                            break;
                        }
                    }
                    else if (!object.Equals(value, value2))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private bool equal(Array a, Array b)
        {
            if (a.Length == b.Length)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    object value = a.GetValue(i);
                    object value2 = b.GetValue(i);
                    if (!object.Equals(value, value2))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            DeepCloneObject dco = (DeepCloneObject)Activator.CreateInstance(base.GetType());
            Type type = base.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                object value = fieldInfo.GetValue(this);
                if (value is ICloneable)
                {
                    object value2 = ((ICloneable)value).Clone();
                    fieldInfo.SetValue(dco, value2);
                }
                else
                {
                    fieldInfo.SetValue(dco, value);
                }
            }
            return dco;
        }
    }

    public class DeepCloneObjectList<T> : List<T>, ICloneable
    {
        public DeepCloneObjectList()
        {
        }

        public DeepCloneObjectList(IEnumerable<T> collection) : base(collection)
        {
        }

        public object Clone()
        {
            DeepCloneObjectList<T> dcoList = (DeepCloneObjectList<T>)Activator.CreateInstance(base.GetType());
            foreach (T current in this)
            {
                if (current is ICloneable)
                {
                    dcoList.Add((T)((object)((ICloneable)((object)current)).Clone()));
                }
                else
                {
                    dcoList.Add(current);
                }
            }
            return dcoList;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object i_other)
        {
            DeepCloneObjectList<T> dcoList = i_other as DeepCloneObjectList<T>;
            bool result = false;
            if (dcoList != null && dcoList.Count == base.Count)
            {
                result = true;
                for (int i = 0; i < base.Count; i++)
                {
                    if (!object.Equals(dcoList[i], base[i]))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
