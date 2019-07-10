using System;
using System.Text;
using System.Collections.Generic;
using WD.Library.Core;

namespace WD.Library.Expression
{
    /// <summary>
    /// �������
    /// </summary>
    /// <remarks>
    /// ����ʽ������һЩ�����װ
    /// </remarks>
    public enum ParseError 
    {
        /// <summary>
        /// ���쳣
        /// </summary>
        peNone = 0,

        /// <summary>
        /// �Ƿ��ַ�
        /// </summary>
        peInvalidChar,

        /// <summary>
        /// �Ƿ����ַ���
        /// </summary>
        peInvalidString,

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        peInvalidOperator,

        /// <summary>
        /// ���Ͳ�ƥ��
        /// </summary>
        peTypeMismatch,

        /// <summary>
        /// �Ƿ��Ĳ���
        /// </summary>
        peInvalidParam,

        /// <summary>
        /// �Ƿ����û��Զ��庯������ֵ
        /// </summary>
        peInvalidUFValue,

        /// <summary>
        /// �﷨����
        /// </summary>
        peSyntaxError,

        /// <summary>
        /// �����������
        /// </summary>
        peFloatOverflow,

        /// <summary>
        /// ��Ҫĳ���ַ�
        /// </summary>
        peCharExpected,

        /// <summary>
        /// ��������
        /// </summary>
        peFuncError,

        /// <summary>
        /// ��Ҫ������
        /// </summary>
        peNeedOperand,

        /// <summary>
        /// ��ʽ����
        /// </summary>
        peFormatError,

        /// <summary>
        /// �������ʹ���
        /// </summary>
        InvalidParameterType,


        ///// <summary>
        ///// ������������
        ///// </summary>
        //InvalidParameterNum, 
    }

    /// <summary>
    /// �������ͣ���Ϊ�﷨�����������ڵ��е��������ʶ
    /// </summary>
    //public enum ParseOperation
	public enum Operation_IDs
    {
        /// <summary>
        /// �޲�����
        /// </summary>
        [EnumItemDescription("�޲�����", ShortName="")]
        OI_NONE = 0,

        /// <summary>
        /// ��
        /// </summary>
        [EnumItemDescription("�ǲ�����", ShortName = "!")]
        OI_NOT = 120,
        
        /// <summary>
        /// ��
        /// </summary>
        [EnumItemDescription("�Ӳ�����", ShortName = "+")]
        OI_ADD,
        
        /// <summary>
        /// ��
        /// </summary>
        [EnumItemDescription("��������", ShortName = "-")]
        OI_MINUS,
        
        /// <summary>
        /// ��
        /// </summary>
        [EnumItemDescription("�˲�����", ShortName = "*")]
        OI_MUL,
        
        /// <summary>
        /// ��
        /// </summary>
        [EnumItemDescription("��������", ShortName = "/")]
        OI_DIV,
        
        /// <summary>
        /// ����
        /// </summary>
        [EnumItemDescription("��������", ShortName = "-")]
        OI_NEG,
        
        /// <summary>
        /// ����
        /// </summary>
        [EnumItemDescription("���ڲ�����", ShortName = "==")]
        OI_EQUAL,
        
        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("�����ڲ�����", ShortName = "<>")]
        OI_NOT_EQUAL,
        
        /// <summary>
        /// ����
        /// </summary>
        [EnumItemDescription("���ڲ�����", ShortName = "<")]
        OI_GREAT,

        /// <summary>
        /// ���ڵ���
        /// </summary>
        [EnumItemDescription("���ڵ��ڲ�����", ShortName = ">=")]
        OI_GREATEQUAL,

        /// <summary>
        /// С��
        /// </summary>
        [EnumItemDescription("С�ڲ�����", ShortName = "<")]
        OI_LESS,

        /// <summary>
        /// С�ڵ���
        /// </summary>
        [EnumItemDescription("С�ڵ��ڲ�����", ShortName = "<=")]
        OI_LESSEQUAL,

        /// <summary>
        ///�߼��� 
        /// </summary>
        [EnumItemDescription("�߼��������", ShortName = "&")]
        OI_LOGICAL_AND,

        /// <summary>
        /// �߼���
        /// </summary>
        [EnumItemDescription("�߼��������", ShortName = "|")]
        OI_LOGICAL_OR,
        
        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("�����Ų�����", ShortName = "(")]
        OI_LBRACKET,

        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("�����Ų�����", ShortName = ")")]
        OI_RBRACKET,

        /// <summary>
        /// ����
        /// </summary>
        [EnumItemDescription("���Ų�����", ShortName = ",")]
        OI_COMMA,
        
        /// <summary>
        /// �Զ��庯��
        /// </summary>
        [EnumItemDescription("�Զ��庯��", ShortName = "�Զ��庯��")]
        OI_USERDEFINE,

        /// <summary>
        /// �ַ���
        /// </summary>
        [EnumItemDescription("�ַ���", ShortName = "�ַ���")]
        OI_STRING,

        /// <summary>
        /// ����
        /// </summary>
        [EnumItemDescription("����", ShortName = "����")]
        OI_NUMBER,

        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("������", ShortName = "������")]
        OI_BOOLEAN,

        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("������", ShortName = "������")]
        OI_DATETIME,
    }
}