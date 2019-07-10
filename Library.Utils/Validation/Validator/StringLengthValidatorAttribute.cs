using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
    /// <summary>
    /// �ַ������ȹ����������࣬У���ַ��������Ƿ���ָ���ķ�Χ��
    /// </summary>
    /// <remarks>
    /// �ַ������ȹ����������࣬У���ַ��������Ƿ���ָ���ķ�Χ��
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class StringLengthValidatorAttribute : ValidatorAttribute 
    {
        private int lowerBound;
        private int upperBound;

        /// <summary>
        /// �������Сֵ
        /// </summary>
        public int LowerBound
        {
            get { return lowerBound; }
        }

        /// <summary>
        /// �������ֵ
        /// </summary>
        public int UpperBound
        {
            get { return upperBound; }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="upperBound">�ַ�����������</param>
        /// <remarks>
        public StringLengthValidatorAttribute(int upperBound)
            : this(0, upperBound)
        { 
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="lowerBound">�ַ�����������</param>
        /// <param name="upperBound">�ַ�����������</param>
        /// <remarks>
        public StringLengthValidatorAttribute(int lowerBound, int upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        /// <summary>
        /// ���ػ���ķ���������StringLengthValidator���췽������StringLengthValidator
        /// </summary>
        /// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
        /// <returns>�ַ�������У����ʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new StringLengthValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag); 
        }

    }
}
