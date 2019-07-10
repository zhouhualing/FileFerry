using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using WD.Library.Core;
using WD.Library.Properties;

namespace WD.Library.Configuration
{
	/// <summary>
	/// ��Config�쳣�йص��쳣��������
	/// </summary>
	public static class ConfigurationExceptionHelper
	{
		/// <summary>
		/// ���section�Ƿ�Ϊ�գ����Ϊ�գ����׳��쳣
		/// </summary>
		/// <param name="section">section����</param>
		/// <param name="sectionName">section�����ƣ������쳣��Ϣ�г���</param>
		public static void CheckSectionNotNull(ConfigurationSection section, string sectionName)
		{
			ExceptionHelper.FalseThrow<ConfigurationException>(section != null, 
				Resource.CanNotFoundConfigSection, sectionName);
		}

		/// <summary>
		/// ���section��source�Ƿ�Ϊ�գ����Ϊ�գ����׳��쳣�����������ִ��CheckSectionNotNull
		/// </summary>
		/// <param name="section">section����</param>
		/// <param name="sectionName">section�����ƣ������쳣��Ϣ�г���</param>
		public static void CheckSectionSource(ConfigurationSection section, string sectionName)
		{
			CheckSectionNotNull(section, sectionName);

			ExceptionHelper.FalseThrow<ConfigurationException>(section.ElementInformation.Source != null,
				Resource.CanNotFoundConfigSectionElement, sectionName);
		}
	}
}
