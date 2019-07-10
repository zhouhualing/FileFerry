
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace WD.Library.Reflection
{
	public static class EnumUtility
	{
        public static dynamic CreateEnumValueList(Type enumType)
        {
            var genericType = typeof(List<>).MakeGenericType(enumType);
            var result = Activator.CreateInstance(genericType);
            MethodInfo addMethod = genericType.GetMethod("Add");
            foreach (FieldInfo fieldInfo in enumType.GetFields())
            {
                if (enumType.Equals(fieldInfo.FieldType))
                {
                    addMethod.Invoke(result,new object[] { Enum.Parse(enumType, fieldInfo.Name, true) });
                }
            }
            return result;

        }
        public static List<TEnum> CreateEnumValueList<TEnum>()
		{
			Type enumType = typeof(TEnum);

			List<TEnum> result = new List<TEnum>();
			foreach (FieldInfo fieldInfo in enumType.GetFields())
			{
				if (enumType.Equals(fieldInfo.FieldType))
				{
					TEnum item = (TEnum)Enum.Parse(enumType, fieldInfo.Name, true);
					result.Add(item);
				}
			}
			return result;
		}

        public static List<string> CreateEnumValueStringList<TEnum>()
        {
            List<TEnum> list = CreateEnumValueList<TEnum>();
            List<string> list2 = new List<string>();
            EnumConverter converter = new EnumConverter(typeof(TEnum));
            foreach (var v in list)
            {
                list2.Add(converter.ConvertToString(v));
            }
            return list2;
        }

        public static List<string> CreateEnumValueStringList(Type enumType)
        {
            var list = CreateEnumValueList(enumType);
            List<string> list2 = new List<string>();
            EnumConverter converter = new EnumConverter(enumType);
            foreach (var v in list)
            {
                list2.Add(converter.ConvertToString(v));
            }
            return list2;
        }

        //Hualing: if FlagsAttribute exist,return value like A,B,C
        public static string GetEnumValueString(Type type , object value)
        {
            EnumConverter converter = new EnumConverter(type);

            if (value is string)
            {
                string tmpStr = value.ToString().Trim();
                List<string> strList = CreateEnumValueStringList(type);

                //Multi-value like A|B
                if (tmpStr.Contains("|"))
                {
                    return ConvertEnumValueString(tmpStr, strList, type);
                }
                //Single-value like A
                else
                {
                    if (strList.Contains(tmpStr))
                        return tmpStr;
                }
            }

            Enum[] values = (Enum[])converter.ConvertTo(value, typeof(Enum[]));

            List<string> list = new List<string>();
            foreach (Enum enumValue in values)
            {
                list.Add(converter.ConvertToString(enumValue) );
            }
            return string.Join(",", list);            
        }

        //Hualing: if FlagsAttribute exist,return value like A,B,C
        public static string GetEnumValueString<TEnum>(object value)
        {
            EnumConverter converter = new EnumConverter(typeof(TEnum));

            if (value is string)
            {
                string tmpStr = value.ToString().Trim();
                List<string> strList = CreateEnumValueStringList<TEnum>();

                //Multi-value like A|B
                if (tmpStr.Contains("|"))
                {
                    return ConvertEnumValueString(tmpStr, strList, typeof(TEnum));
                }
                //Single-value like A
                else
                {
                    if (strList.Contains(tmpStr))
                        return tmpStr;
                }
            }

            Enum[] values = (Enum[])converter.ConvertTo(value, typeof(Enum[]));            

            List<string> list = new List<string>();
            foreach (Enum enumValue in values)
            {
                list.Add(converter.ConvertToString(enumValue));
            }
            return string.Join(",", list);
        }

        public static IEnumerable<TEnum> CreateEnumValueList<TEnum>(this Enum enumValue)
		{
			return CreateEnumValueList<TEnum>();
		}

        public static string ConvertEnumValueString(string enumStr,List<string> srcList,Type type)
        {
            string[] tokens = enumStr.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens != null
                && tokens.Length > 0)
            {
                foreach (var token in tokens)
                {
                    if (!srcList.Contains(token))
                    {
                        throw new InvalidEnumArgumentException($"{token} is not valid {type}.");
                    }
                }
                return enumStr.Replace("|", ",");
            }
            return string.Empty;
        }

        public static string GetEnumDescription<TEnum>(TEnum enumValue)
        {
            Type enumType = typeof(TEnum);

            List<TEnum> result = new List<TEnum>();
            foreach (FieldInfo fieldInfo in enumType.GetFields())
            {
                if (enumType.Equals(fieldInfo.FieldType))
                {
                    TEnum item = (TEnum)Enum.Parse(enumType, fieldInfo.Name, true);
                    if (item.Equals(enumValue))
                    {
                        var description = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                        if (description != null && !string.IsNullOrWhiteSpace(description.Description))
                        {
                            return description.Description;
                        }

                    }
                }
            }
            return string.Empty;
        }
    }
}
