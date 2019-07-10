using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WD.Library.Core
{
    /// <summary>
    /// Instance mode
    /// </summary>
    public enum InstanceMode
    {
        /// <summary>
        /// WindowsӦ��
        /// </summary>
        Windows,

        /// <summary>
        /// WebӦ��
        /// </summary>
        Web
    }

    /// <summary>
    /// ����Ӧ�û����������
    /// </summary>
    /// <remarks>����Ӧ�û����������
    /// </remarks>
    public static class EnvironmentHelper
    {
        #region Private field

        private static string shortDomainName;
        private static string domainDnsName;
        #endregion

        #region Constructor
        /// <summary>
        /// EnvironmentHelper�Ĺ��캯��
        /// </summary>
        /// <remarks>EnvironmentHelper�Ĺ��캯��,�ù��캯�������κβ�����
        /// </remarks>
        static EnvironmentHelper()
        {
            EnvironmentHelper.shortDomainName = GetShortDomainName();
            EnvironmentHelper.domainDnsName = GetDomainDnsName();
        }
        #endregion

        #region InterOP
        private enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
            ComputerNameMax,
        }

        private enum EXTENDED_NAME_FORMAT
        {
            NameUnknown = 0,
            NameFullyQualifiedDN = 1,
            NameSamCompatible = 2,
            NameDisplay = 3,
            NameUniqueId = 6,
            NameCanonical = 7,
            NameUserPrincipal = 8,
            NameCanonicalEx = 9,
            NameServicePrincipal = 10,
            NameDnsDomain = 12,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, StringBuilder nameBuffer, ref int bufferSize);

        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int GetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat, StringBuilder nameBuffer, ref int bufferSize);

        #endregion InterOP

        #region Private helper method
        private static bool CheckIsWebApplication()
        {
            bool isWebApp = false;
         
            AppDomain domain = AppDomain.CurrentDomain;
            try
            {
                if (domain.ShadowCopyFiles)
                    isWebApp = (HttpContext.Current != null);
            }
            catch(System.Exception)
            {
            }

            return isWebApp;
        }

        private static string GetShortDomainName()
        {
            string machineName = InnerGetComputerObjectName(EXTENDED_NAME_FORMAT.NameSamCompatible);

            string[] nameParts = machineName.Split('\\', '/');

            return nameParts[0];
        }

        private static string GetDomainDnsName()
        {
            return InnerGetComputerName(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsDomain);
        }

        private static string InnerGetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat)
        {
            StringBuilder strB = new StringBuilder(1024);

            int nSize = strB.Capacity;

            GetComputerObjectName(nameFormat, strB, ref nSize);

            return strB.ToString();
        }

        private static string InnerGetComputerName(COMPUTER_NAME_FORMAT nameType)
        {
            StringBuilder strB = new StringBuilder(1024);

            int nSize = strB.Capacity;

            GetComputerNameEx(nameType, strB, ref nSize);

            return strB.ToString();
        }
        #endregion


        /// <summary>
        /// ��ǰӦ���Ƿ�ΪwebӦ�õ����� ( Windows / Web)
        /// </summary>
        /// <remarks>��������ֻ���ġ�
        /// <seealso cref="WD.Library.Configuration.ConfigurationBroker"/>
        /// <seealso cref="WD.Library.Configuration.MetaConfigurationSourceInstanceElement"/>
        /// <seealso cref="WD.Library.Configuration.MetaConfigurationSourceMappingElement"/>
        /// </remarks>
        public static InstanceMode Mode
        {
            get 
            {
                if (EnvironmentHelper.CheckIsWebApplication())
                    return InstanceMode.Web;
                else
                    return InstanceMode.Windows;
            }
        }

        /// <summary>
        /// �������������ע�ᣬ���ض�����
        /// </summary>
        /// <remarks>������ֻ����������������ϵĶ�������oa\hb2004-db����ôShortDomainName����oa�����û�м������򷵻ؿմ�</remarks>
        public static string ShortDomainName
        {
            get
            {
                return EnvironmentHelper.shortDomainName;
            }
        }

        /// <summary>
        /// �������������ע�ᣬ���س�����
        /// </summary>
        /// <remarks>������ֻ������ĳ�������oa.hgzs.ain.cn�����û�м������򷵻ؿմ�</remarks>
        public static string DomainDnsName
        {
            get
            {
                return EnvironmentHelper.domainDnsName;
            }
        }
        public static bool Is64Bit
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }
    }
}
