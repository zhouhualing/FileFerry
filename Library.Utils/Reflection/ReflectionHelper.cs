using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WD.Library.Reflection
{
    public class ReflectionHelper
    {
        public static Type GetNullableType(Type propertyType)
        {
            if (propertyType.IsGenericType &&
                propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return propertyType.GetGenericArguments()[0];
            }

            return propertyType;
        }

        public static PropertyInfo GetProperty(Type type, string propertyName, bool throwIfInvalid)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (!IsValidProperty(propertyInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            propertyName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return propertyInfo;
        }

        public static bool IsValidProperty(PropertyInfo propertyInfo)
        {
            return null != propertyInfo                // exists
                    && propertyInfo.CanRead            // and it's readable
                    && propertyInfo.GetIndexParameters().Length == 0;    // and it's not an indexer
        }

        public static FieldInfo GetField(Type type, string fieldName, bool throwIfInvalid = false)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance| BindingFlags.Static);

            if (!IsValidField(fieldInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            fieldName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return fieldInfo;
        }

        internal static bool IsValidField(FieldInfo fieldInfo)
        {
            return null != fieldInfo;
        }

        public static MethodInfo GetMethod(Type type, string methodName, bool throwIfInvalid)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullException("methodName");
            }

            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            if (!IsValidMethod(methodInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            methodName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return methodInfo;
        }

        internal static bool IsValidMethod(MethodInfo methodInfo)
        {
            return null != methodInfo
                && typeof(void) != methodInfo.ReturnType
                && methodInfo.GetParameters().Length == 0;
        }

        internal static T ExtractValidationAttribute<T>(MemberInfo attributeProvider)
            where T : Attribute
        {
            if (attributeProvider != null)
            {
                foreach (T attribute in GetCustomAttributes(attributeProvider, typeof(T), false))
                {
                    return attribute;
                }
            }

            return null;
        }

        public static T ExtractValidationAttribute<T>(ParameterInfo attributeProvider)
            where T : Attribute
        {
            if (attributeProvider != null)
            {
                foreach (T attribute in attributeProvider.GetCustomAttributes(typeof(T), false))
                {
                    return attribute;
                }
            }

            return null;
        }

        public static IEnumerable<MethodInfo> GetSelfValidationMethods<T>(Type type) where T : Attribute
        {
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var attr = methodInfo.GetCustomAttribute<T>();
                if (attr != null)
                {
                    bool hasReturnType = methodInfo.ReturnType == typeof(bool);
                    ParameterInfo[] parameters = methodInfo.GetParameters();

                    if (hasReturnType && parameters.Length == 2)
                    {
                        foreach (T attribute
                            in GetCustomAttributes(methodInfo, typeof(T), false))
                        {
                            yield return methodInfo;
                            continue;
                        }
                    }
                }
            }
        }

        public static Attribute[] GetClassCustomAttributes(Type type, Type attributeType, bool inherit)
        {
            List<Attribute> attrList = new List<Attribute>();
            foreach (var element in type.GetCustomAttributes(attributeType,inherit))
            {
                attrList.Add(element as Attribute);
            }
            return attrList.ToArray();
        }

        public static Attribute[] GetCustomAttributes(Type type, Type attributeType, bool inherit)
        {
            List<Attribute> attrList = new List<Attribute>();
            foreach (var element in type.GetProperties())
            {
                MemberInfo matchingElement = GetMatchingElement(element);

                attrList.AddRange(Attribute.GetCustomAttributes(matchingElement, attributeType, inherit));
            }
            return attrList.ToArray();
        }

        public static Attribute[] GetCustomAttributes(MemberInfo element, Type attributeType, bool inherit)
        {
            MemberInfo matchingElement = GetMatchingElement(element);

            return Attribute.GetCustomAttributes(matchingElement, attributeType, inherit);
        }

        private static MemberInfo GetMatchingElement(MemberInfo element)
        {
            Type sourceType = element as Type;
            bool elementIsType = sourceType != null;
            if (sourceType == null)
            {
                sourceType = element.DeclaringType;
            }

            MetadataTypeAttribute metadataTypeAttribute = (MetadataTypeAttribute)
                Attribute.GetCustomAttribute(sourceType, typeof(MetadataTypeAttribute), false);

            if (metadataTypeAttribute == null)
            {
                return element;
            }

            sourceType = metadataTypeAttribute.MetadataClassType;

            if (elementIsType)
            {
                return sourceType;
            }

            MemberInfo[] matchingMembers =
                sourceType.GetMember(
                    element.Name,
                    element.MemberType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (matchingMembers.Length > 0)
            {
                MethodBase methodBase = element as MethodBase;
                if (methodBase == null)
                {
                    return matchingMembers[0];
                }

                Type[] parameterTypes = methodBase.GetParameters().Select(pi => pi.ParameterType).ToArray();
                return matchingMembers.Cast<MethodBase>().FirstOrDefault(mb => MatchMethodBase(mb, parameterTypes)) ?? element;
            }

            return element;
        }

        private static bool MatchMethodBase(MethodBase mb, Type[] parameterTypes)
        {
            ParameterInfo[] parameters = mb.GetParameters();

            if (parameters.Length != parameterTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != parameterTypes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
