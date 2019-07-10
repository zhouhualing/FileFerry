using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
    /// <summary>
    /// У����У����
    /// </summary>
    /// <remarks>
    /// ����У����У���������ݽṹ
    /// </remarks>
    [Serializable]
    public class ValidationResult
    {
        private string message;
        private string key;
        private string tag;
        [NonSerialized]
        private object target;
        [NonSerialized]
        private Validator validator;
        private IEnumerable<ValidationResult> nestedValidationResults;
        private static readonly IEnumerable<ValidationResult> NoNestedValidationResults = new ValidationResult[0];

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="message">У����ʾ��Ϣ</param>
        /// <param name="target">У�����</param>
        /// <param name="key">У������ʶ</param>
        /// <param name="tag">У�������</param>
        /// <param name="validator">У����</param>
        public ValidationResult(string message, object target, string key, string tag, Validator validator)
			: this(message, target, key, tag, validator, ValidationResult.NoNestedValidationResults)
        { }
      
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="message">У����Ϣ</param>
        /// <param name="target">У�����</param>
        /// <param name="key">У������ʶ</param>
        /// <param name="tag">У�������</param>
        /// <param name="validator">����У���У����</param>
        /// <param name="nestedValidationResults">Ƕ�׵�У������Ӧ�ó��������͵�У������ͨ��������ṹ�ڼ�¼���У��ʧ�ܵ���Ϣ��</param>
        public ValidationResult(string message, object target, string key, string tag, Validator validator,
            IEnumerable<ValidationResult> nestedValidationResults)
        {
            this.message = message;
            this.key = key;
            this.target = target;
            this.tag = tag;
            this.validator = validator;
            this.nestedValidationResults = nestedValidationResults;
        }

        /// <summary>
        /// ��ʶ
        /// </summary>
        public string Key
        {
            get 
            { 
                return this.key; 
            }
        }

        /// <summary>
        /// У����Ϣ
        /// </summary>
        public string Message
        {
            get 
            { 
                return this.message; 
            }
        }

        /// <summary>
        /// ���
        /// </summary>
        public string Tag
        {
            get 
            { 
                return tag; 
            }
        }

        /// <summary>
        /// ��У�����
        /// </summary>
        public object Target
        {
            get 
            { 
                return this.target; 
            }
        }
       
        /// <summary>
        /// У����
        /// </summary>
        public Validator Validator
        {
            get 
            { 
                return this.validator; 
            }
        }

        /// <summary>
        /// Ƕ�׵�У����
        /// </summary>
        public IEnumerable<ValidationResult> NestedValidationResults
        {
            get 
            { 
                return this.nestedValidationResults; 
            }
        }
    }

    /// <summary>
    /// У����ö�ټ���
    /// </summary>
    public class ValidationResults : IEnumerable<ValidationResult>
    {
        private List<ValidationResult> validationResults;

        /// <summary>
        /// ���췽��
        /// </summary>
        public ValidationResults()
        { 
            this.validationResults = new List<ValidationResult>(); 
        }

        /// <summary>
        /// ��У���������д�ŵ�У������Ŀ
        /// </summary>
        /// <returns></returns>
        public int ResultCount
        {
            get
            {
                return this.validationResults.Count;
            }
        }

        /// <summary>
        /// ��У������ö�ټ��������У��������
        /// </summary>
        /// <param name="result">У��������</param>
        public void AddResult(ValidationResult result)
        {
            this.validationResults.Add(result);
        }

        /// <summary>
        /// �ж�У���Ƿ�ͨ��
        /// </summary>
        /// <returns>У����</returns>
        public bool IsValid()
        {
            return this.validationResults.Count == 0;
        }

        IEnumerator<ValidationResult> IEnumerable<ValidationResult>.GetEnumerator()
        {
            return this.validationResults.GetEnumerator();
           
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.validationResults.GetEnumerator();
        }
    }
}
