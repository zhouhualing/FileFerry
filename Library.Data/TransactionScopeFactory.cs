using System;
using System.Collections.Generic;
using System.Transactions;
using System.Configuration;

using WD.Library.Configuration;
using WD.Library.Data.Configuration;
using WD.Library.Core;

namespace WD.Library.Data
{
    /// <summary>
    /// Ϊ�˶���TransactionScope�����ò�����Ƶ�ר�ù����ࡣ
    /// </summary>
    public static class TransactionScopeFactory
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.Parse("00:10:00");
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        private static TransactionConfigurationSection GetConfiguration()
        {
            TransactionConfigurationSection section = ConfigurationBroker.GetSection("transactions") as TransactionConfigurationSection;

            if (section == null)//ϵͳ����Ĭ������
                section = new TransactionConfigurationSection();

            return section;
        }

        /// <summary>
        /// ������� TransactionScope ���õĳ�ʱ����
        /// </summary>
        public static TimeSpan Timeout
        {
            get
            {
                TransactionConfigurationSection section = GetConfiguration();
                if (section == null)
                    return DefaultTimeout;
                else
                    return section.TimeOut;
            }
        }


        /// <summary>
        /// ������� TransactionScope ���õĽ��׸����
        /// </summary>
        public static IsolationLevel IsolationLevel
        {
            get
            {
                TransactionConfigurationSection section = GetConfiguration();
                if (section == null)
                    return DefaultIsolationLevel;
                else
                    return section.IsolationLevel;
            }
        }


        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����ѡ��</param>
        /// <param name="transactionOptions">���񸽼���Ϣ</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
        {
            return new TransactionScope(scopeOption, transactionOptions);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="scopeTimeout">TimeSpan��ʽ��ʾ�ĳ�ʱʱ��</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
        {
            ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(scopeTimeout <= TimeSpan.Zero, "scopeTimeout");

            TransactionOptions options = new TransactionOptions();
            options.Timeout = scopeTimeout;
            options.IsolationLevel = TransactionScopeFactory.IsolationLevel;

            return Create(scopeOption, options);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="scopeTimeout">��ʱʱ��</param>
        /// <param name="isolationLevel">���뼶��</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, IsolationLevel isolationLevel)
        {
            if (scopeTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("scopeTimeout");

            // ��װTS����
            TransactionOptions options = new TransactionOptions();
            options.Timeout = scopeTimeout;
            options.IsolationLevel = isolationLevel;

            return Create(scopeOption, options);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <param name="isolationLevel">���뼶��</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            return Create(scopeOption, TransactionScopeFactory.Timeout, isolationLevel);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <param name="scopeOption">����Χ��ѡ��</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(TransactionScopeOption scopeOption)
        {
            return Create(scopeOption, TransactionScopeFactory.Timeout, TransactionScopeFactory.IsolationLevel);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// </summary>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create()
        {
            return Create(TransactionScopeOption.Required);
        }

        /// <summary>
        /// ���� TransactionScope ����ʵ��
        /// ���� Transaction �Դ����׸����˵������˲��漰��������ȡ�ò����Ĳ���
        /// </summary>
        /// <param name="transactionToUse">����</param>
        /// <param name="scopeTimeout">��ʱ��Χ</param>
        /// <returns>�����Է�Χ����</returns>
        public static TransactionScope Create(Transaction transactionToUse, TimeSpan scopeTimeout)
        {
            if (scopeTimeout < TimeSpan.Zero) throw new ArgumentOutOfRangeException("scopeTimeout");

            return new TransactionScope(transactionToUse, scopeTimeout);
        }
    }
}
