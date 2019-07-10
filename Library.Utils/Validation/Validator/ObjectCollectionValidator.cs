using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WD.Library.Validation
{
    /// <summary>
    /// ���󼯺ϵ�У�����������ε��ö��󼯺��еĶ������У��
    /// </summary>
    internal class ObjectCollectionValidator : Validator
    {
        private Type targetType;
        private string targetRuleset;
        private Validator targetTypeValidator;

        /// <summary>
        /// ObjectCollectionValidator�Ĺ��캯��
        /// </summary>
        /// <param name="targetType">Ŀ������</param>
        /// <param name="targetRuleset">��У������������У�����</param>
        public ObjectCollectionValidator(Type targetType, string targetRuleset)
        {
            this.targetType = targetType;
            this.targetRuleset = targetRuleset;
            this.targetTypeValidator = ValidationFactory.CreateValidator(targetType, targetRuleset);
        }

        protected internal override void DoValidate(
            object objectToValidate,
            object currentObject,
            string key,
            ValidationResults validateResults)
        {
            if (objectToValidate != null)
            {
                IEnumerable enumerableObjects = (IEnumerable)objectToValidate;

                if (enumerableObjects != null)
                {
                    foreach (object enumerableObject in enumerableObjects)
                    {
                        if (this.targetType.IsAssignableFrom(enumerableObject.GetType()))
                        {
                            this.targetTypeValidator.DoValidate(enumerableObject, enumerableObject,
                                key, validateResults);
                        }
                    }
                }
            }
        }
    }
}
