#region using

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;

using WD.Library.Core;
using WD.Library.Caching;
using WD.Library.Properties;
using System.Text.RegularExpressions;

#endregion

namespace WD.Library.Configuration
{
	/// <summary>
	/// <remarks>
	/// Broker��������б��������ļ���ӳ�������ļ���
	/// Զ�������ļ�ӳ����ConfigurationFileMap��ConfigurationManager.OpenMappedMachineConfiguration����.
	/// 
	/// Լ��:
	///     <list type="bullet">
	///         <item>
	///         ӳ���ļ�������ConfigurationSectionGroup��ConfigurationSection��ʼ.
	///         </item>
	///         <item>
	///         </item>
	///     </list>
	/// </remarks>
	/// </summary>
	public static class ConfigurationBroker
	{
		#region private const and field
		/// <summary>
		/// Private const
		/// </summary>
		private const string LocalItem = "local";
		private const string MetaConfigurationItem = "WD.MetaConfiguration";
		private const string MetaConfigurationSectionGroupItem = "wd.library.metaConfig";


		/// <summary>
		/// Private static field
		/// </summary>
		private static readonly string MachineConfigurationFile = ConfigurationManager.OpenMachineConfiguration().FilePath;
		private static readonly string LocalConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		private static readonly string GlobalConfigurationFile = ConfigurationBroker.MachineConfigurationFile;
		// ����ʧЧʱ��
		private static readonly TimeSpan SlidingTime = TimeSpan.FromSeconds(15.0); //TimeSpan.FromMinutes(5.0);

		#endregion private const and field

		/// <summary>
		/// ���캯��
		/// </summary>
		static ConfigurationBroker()
		{

		}

		/// <summary>
		/// meta�����ļ�λ��ö��
		/// </summary>
		private enum MetaConfigurationPosition
		{
			LocalFile,
			MetaFile
		}

		/// <summary>
		/// �ڲ��࣬���ڴ�š�����machine��local��meta��global�����ļ��ĵ�ַ��
		/// meta�ļ�λ�ã�ö�٣� 
		/// </summary>
		private class ConfigFilesSetting
		{
			private string machineConfigurationFile = string.Empty;
			private string localConfigurationFile = string.Empty;
			private string metaConfigurationFile = string.Empty;
			private string globalConfigurationFile = string.Empty;
			private MetaConfigurationPosition metaFilePosition = MetaConfigurationPosition.LocalFile;

			public MetaConfigurationPosition MetaFilePosition
			{
				get { return this.metaFilePosition; }
				set { this.metaFilePosition = value; }
			}

			public string GlobalConfigurationFile
			{
				get { return this.globalConfigurationFile; }
				set { this.globalConfigurationFile = value; }
			}

			public string MetaConfigurationFile
			{
				get { return this.metaConfigurationFile; }
				set { this.metaConfigurationFile = value; }
			}

			public string LocalConfigurationFile
			{
				get { return this.localConfigurationFile; }
				set { this.localConfigurationFile = value; }
			}

			public string MachineConfigurationFile
			{
				get { return this.machineConfigurationFile; }
				set { this.machineConfigurationFile = value; }
			}
		}



		#region private static method

		/// <summary>
		/// ����configuration����Ļ���keyֵ
		/// </summary>
		/// <param name="fileNames">�ļ��б�</param>
		/// <returns>cache key</returns>
		private static string CreateConfigurationCacheKey(params string[] fileNames)
		{
			StringBuilder key = new StringBuilder(256);

			for (int i = 0; i < fileNames.Length; i++)
			{
				// ֻȡ�ļ�����ȥ������·��
				key.Append(Path.GetFileName(fileNames[i]).ToLower());
			}

			return key.ToString();
		}

		/// <summary>
		/// ����machine��local�����ļ���meta�����ļ���meta�е����ýڣ����仺�沢��������ʧЧ������
		/// ���Ҳ��� ConfigFilesSetting ��ʵ���м�¼machine��local��meta��global�����ļ��ĵ�ַ��
		/// meta�����ļ�λ�ã�ö�٣�
		/// </summary>
		/// <returns>ConfigFilesSetting ��ʵ��</returns>
		private static ConfigFilesSetting LoadFilesSetting()
		{
			ConfigFilesSetting settings = new ConfigFilesSetting();
			settings.MachineConfigurationFile = ConfigurationBroker.MachineConfigurationFile;
			settings.LocalConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
			settings.GlobalConfigurationFile = ConfigurationBroker.GlobalConfigurationFile;

			MetaConfigurationSourceInstanceSection metaSection = ConfigurationBroker.GetMetaSourceInstanceSection(settings);

			if (metaSection != null)
			{
				string currPath;

				if (EnvironmentHelper.Mode == InstanceMode.Web)
					currPath = HttpContext.Current.Request.Url.AbsoluteUri;
				else
					currPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

				settings.GlobalConfigurationFile = ConfigurationBroker.ReplaceEnvironmentVariablesInFilePath(metaSection.Instances.GetMatchedPath(currPath));

				if (string.IsNullOrEmpty(settings.GlobalConfigurationFile))
					settings.GlobalConfigurationFile = ConfigurationBroker.GlobalConfigurationFile;
				else
				{
					if (false == Path.IsPathRooted(settings.GlobalConfigurationFile))
						settings.GlobalConfigurationFile =
							AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + settings.GlobalConfigurationFile;

					ExceptionHelper.FalseThrow(File.Exists(settings.GlobalConfigurationFile), Resource.GlobalFileNotFound, settings.GlobalConfigurationFile);
				}
			}

			return settings;
		}

		/// <summary>
		/// ��ȡmeta�����е� sourceMappings �ڵ�
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		/// <returns>meta�����е� sourceMappings �ڵ�</returns>
		private static MetaConfigurationSourceInstanceSection GetMetaSourceInstanceSection(ConfigFilesSetting fileSettings)
		{
			ConfigurationSection section;

			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(fileSettings.MachineConfigurationFile,
				MetaConfigurationSourceInstanceSection.Name);

			if (ConfigurationSectionCache.Instance.TryGetValue(cacheKey, out section) == false)
			{
				ConfigurationBroker.GetMetaFileSettings(fileSettings);

				if (fileSettings.MetaFilePosition == MetaConfigurationPosition.LocalFile)
					section = ConfigurationBroker.LoadMetaSourceInstanceSectionFromLocal(fileSettings);
				else
					section = ConfigurationBroker.LoadMetaSourceInstanceSectionFromMetaFile(fileSettings);

				FileCacheDependency fileDependency = new FileCacheDependency(
					true,
					fileSettings.MachineConfigurationFile,
					fileSettings.LocalConfigurationFile,
					fileSettings.MetaConfigurationFile);

				SlidingTimeDependency timeDependency = new SlidingTimeDependency(ConfigurationBroker.SlidingTime);

				ConfigurationSectionCache.Instance.Add(cacheKey, section, new MixedDependency(fileDependency, timeDependency));
			}

			return (MetaConfigurationSourceInstanceSection)section;

		}

		/// <summary>
		/// �ӱ���config�ļ��ж�ȡmeta����
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		/// <returns>MetaConfigurationSourceInstanceSection ʵ��</returns>
		private static MetaConfigurationSourceInstanceSection LoadMetaSourceInstanceSectionFromLocal(ConfigFilesSetting fileSettings)
		{
			System.Configuration.Configuration config;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
				config = ConfigurationBroker.GetStandardWebConfiguration(fileSettings.MachineConfigurationFile, true);
			else
				config = ConfigurationBroker.GetStandardExeConfiguration(fileSettings.MachineConfigurationFile, fileSettings.LocalConfigurationFile, true);

			MetaConfigurationSectionGroup group =
				(MetaConfigurationSectionGroup)config.GetSectionGroup(ConfigurationBroker.MetaConfigurationSectionGroupItem);
			MetaConfigurationSourceInstanceSection section = null;

			if (group != null)
				section = group.SourceConfigurationMapping;

			return section;
		}

		/// <summary>
		/// �ӵ�����meta.config�ļ��ж�ȡmeta����
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ʵ��</param>
		/// <returns>MetaConfigurationSourceInstanceSection ʵ��</returns>
		private static MetaConfigurationSourceInstanceSection LoadMetaSourceInstanceSectionFromMetaFile(ConfigFilesSetting fileSettings)
		{
			System.Configuration.Configuration config = ConfigurationBroker.GetSingleFileConfiguration(
					fileSettings.MetaConfigurationFile,
					true,
					fileSettings.MachineConfigurationFile,
					fileSettings.LocalConfigurationFile);

			MetaConfigurationSectionGroup group =
				config.GetSectionGroup(ConfigurationBroker.MetaConfigurationSectionGroupItem) as MetaConfigurationSectionGroup;

			MetaConfigurationSourceInstanceSection section = null;

			if (group != null)
				section = group.SourceConfigurationMapping;

			return section;
		}

		/// <summary>
		/// ��ȡmeta�ļ��ĵ�ַ��λ��
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		private static void GetMetaFileSettings(ConfigFilesSetting fileSettings)
		{
			AppSettingsSection section = ConfigurationBroker.GetLocalAppSettingsSection();

			if (section != null)
			{
				if (section.Settings[ConfigurationBroker.MetaConfigurationItem] == null)
					fileSettings.MetaConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
				else
					fileSettings.MetaConfigurationFile =
						ConfigurationBroker.ReplaceEnvironmentVariablesInFilePath(section.Settings[ConfigurationBroker.MetaConfigurationItem].Value);
			}

			if (string.IsNullOrEmpty(fileSettings.MetaConfigurationFile) == true)
			{
				fileSettings.MetaFilePosition = MetaConfigurationPosition.LocalFile;
				fileSettings.MetaConfigurationFile = ConfigurationBroker.LocalConfigurationFile;
			}
			else
			{
				fileSettings.MetaFilePosition = MetaConfigurationPosition.MetaFile;

				if (false == Path.IsPathRooted(fileSettings.MetaConfigurationFile))
					fileSettings.MetaConfigurationFile =
						AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + fileSettings.MetaConfigurationFile;

				ExceptionHelper.FalseThrow(File.Exists(fileSettings.MetaConfigurationFile), Resource.MetaFileNotFound, fileSettings.MetaConfigurationFile);
			}
		}

		/// <summary>
		/// ��ȡ���� local �� global �ϲ���� Configuration
		/// </summary>
		/// <param name="fileSettings">ConfigFilesSetting ��ʵ��</param>
		/// <returns>local �� global �ϲ���� Configuration</returns>
		private static System.Configuration.Configuration GetFinalConfiguration(ConfigFilesSetting fileSettings)
		{
			System.Configuration.Configuration config;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
				config = ConfigurationBroker.GetStandardWebConfiguration(
							fileSettings.GlobalConfigurationFile,
							true,
							fileSettings.LocalConfigurationFile,
							fileSettings.MachineConfigurationFile,
							fileSettings.MetaConfigurationFile);
			else
				config = ConfigurationBroker.GetStandardExeConfiguration(fileSettings.GlobalConfigurationFile,
							fileSettings.LocalConfigurationFile,
							true,
							fileSettings.MachineConfigurationFile,
							fileSettings.MetaConfigurationFile);

			return config;
		}

		/// <summary>
		/// ��ȡ����config��AppSettings�ڵ�
		/// </summary>
		/// <returns>AppSettingsSection</returns>
		private static AppSettingsSection GetLocalAppSettingsSection()
		{
			System.Configuration.Configuration config = null;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
				config = ConfigurationBroker.GetStandardWebConfiguration(ConfigurationBroker.MachineConfigurationFile, true);
			else
				config = ConfigurationBroker.GetStandardExeConfiguration(ConfigurationBroker.MachineConfigurationFile, ConfigurationBroker.LocalConfigurationFile, true);

			return config.AppSettings;
		}

		/// <summary>
		/// ȡ�õ���config�ļ��е� Configuration
		/// </summary>
		/// <param name="fileName">�ļ���ַ</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <returns>Configuration����</returns>
		private static System.Configuration.Configuration GetSingleFileConfiguration(string fileName, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(fileName);

			System.Configuration.Configuration config;

			if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
			{
				config = ConfigurationManager.OpenMappedMachineConfiguration(new ConfigurationFileMap(fileName));

				Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 1);
				fileDependencies[fileDependencies.Length - 1] = fileName;

				ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
			}

			return config;
		}

		/// <summary>
		/// ��ȡ��׼WebӦ�ó����������Ϣ���ϲ�Web.config��global�����ļ�
		/// </summary>
		/// <param name="machineConfigPath">global�����ļ���ַ</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <returns>Web.config��global�����ļ��ϲ����Configuration����</returns>
		private static System.Configuration.Configuration GetStandardWebConfiguration(string machineConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(machineConfigPath);

			System.Configuration.Configuration config;

			if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
			{
				WebConfigurationFileMap fileMap = new WebConfigurationFileMap();

				fileMap.MachineConfigFilename = machineConfigPath;
				VirtualDirectoryMapping vDirMap = new VirtualDirectoryMapping(
						HttpContext.Current.Request.PhysicalApplicationPath,
						true);

				fileMap.VirtualDirectories.Add("/", vDirMap);

				config = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/",
					HttpContext.Current.Request.ServerVariables["INSTANCE_ID"]);

				Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 1);
				fileDependencies[fileDependencies.Length - 1] = machineConfigPath;

				ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
#if DELUXEWORKSTEST
				// ����ʹ��
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromFile;
#endif
			}
			else
			{
#if DELUXEWORKSTEST
				// ����ʹ��
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromCache;
#endif
			}

			return config;

		}

		/// <summary>
		/// ��ȡ��׼WinFormӦ�ó����������Ϣ���ϲ�App.config��global�����ļ�
		/// </summary>
		/// <param name="machineConfigPath">global�����ļ���ַ</param>
		/// <param name="localConfigPath">����Ӧ�ó��������ļ���ַ</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="fileDependencies">���������ļ�</param>
		/// <returns>App.config��global�����ļ��ϲ����Configuration����</returns>
		private static System.Configuration.Configuration GetStandardExeConfiguration(string machineConfigPath, string localConfigPath, bool ignoreFileNotExist, params string[] fileDependencies)
		{
			string cacheKey = ConfigurationBroker.CreateConfigurationCacheKey(machineConfigPath, localConfigPath);

			System.Configuration.Configuration config;

			if (ConfigurationCache.Instance.TryGetValue(cacheKey, out config) == false)
			{
				ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
				fileMap.MachineConfigFilename = machineConfigPath;
				fileMap.ExeConfigFilename = localConfigPath;

				config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

				Array.Resize<string>(ref fileDependencies, fileDependencies.Length + 2);
				fileDependencies[fileDependencies.Length - 2] = machineConfigPath;
				fileDependencies[fileDependencies.Length - 1] = localConfigPath;

				ConfigurationBroker.AddConfigurationToCache(cacheKey, config, ignoreFileNotExist, fileDependencies);
#if DELUXEWORKSTEST
				// ����ʹ��
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromFile;
#endif
			}
			else
			{
#if DELUXEWORKSTEST
				// ����ʹ��
				ConfigurationBroker.configurationReadFrom = ReadFrom.ReadFromCache;
#endif
			}

			return config;

		}

		/// <summary>
		/// ��Configuration������뻺�棬����ʱ����ļ��Ļ������
		/// </summary>
		/// <param name="cacheKey">cache key</param>
		/// <param name="config">�������Configuration����</param>
		/// <param name="ignoreFileNotExist">�Ƿ���Բ����ڵ��ļ�</param>
		/// <param name="files">�����������ļ�</param>
		private static void AddConfigurationToCache(string cacheKey, System.Configuration.Configuration config, bool ignoreFileNotExist, params string[] files)
		{
			MixedDependency dependency = new MixedDependency(
				new FileCacheDependency(ignoreFileNotExist, files),
				new SlidingTimeDependency(ConfigurationBroker.SlidingTime));

			ConfigurationCache.Instance.Add(cacheKey, config, dependency);
		}

		/// <summary>
		/// ��SectionGroup�ж�ȡSection����Sectionд��Group��ʱʹ��
		/// </summary>
		/// <param name="sectionName">section name</param>
		/// <param name="groups">SectionGroup</param>
		/// <returns>ConfigurationSection</returns>
		private static ConfigurationSection GetSectionFromGroups(string sectionName, ConfigurationSectionGroupCollection groups)
		{
			ConfigurationSection section = null;

			for (int i = 0; i < groups.Count; i++)
			{
				try
				{
					ConfigurationSectionGroup group = groups[i];

					if (group.SectionGroups.Count > 0)
						section = ConfigurationBroker.GetSectionFromGroups(sectionName, group.SectionGroups);
					else
						section = group.Sections[sectionName];

					if (section != null)
						break;
				}
				catch (System.IO.FileNotFoundException)
				{
				}

			}

			return section;
		}

		private static string ReplaceEnvironmentVariablesInFilePath(string filePath)
		{
			string result = filePath;

			Regex r = new Regex(@"%\S+?%");

			foreach (Match m in r.Matches(filePath))
			{
				string variableName = m.Value.Trim('%');
				string variableValue = Environment.GetEnvironmentVariable(variableName);

				if (variableValue != null)
					result = result.Replace(m.Value, variableValue);
			}

			return result;
		}

		#endregion private static method

		#region Public static method

		/// <summary>
		/// ���ڵ����ƴ�������Ϣ��ȡ�ýڵ㣬�����ڵ���Ϣ���棬�����ļ�����
		/// </summary>
		/// <param name="sectionName">�ڵ�����</param>
		/// <returns>���ýڵ�</returns>
		/// <remarks>
		/// �����ƻ�ȡ���ýڵ���Ϣ������ConfigurationSection��������ʵ�壬ʵ���������û��Զ��塣
		
		/// </remarks>
		public static ConfigurationSection GetSection(string sectionName)
		{

			ConfigurationSection section;

			if(false == ConfigurationSectionCache.Instance.TryGetValue(sectionName, out section))
			{
				ConfigFilesSetting settings = LoadFilesSetting();

				System.Configuration.Configuration config = GetFinalConfiguration(settings);
				section = config.GetSection(sectionName);

				// ��Configuration�����в���ֱ���õ�Section����ʱ����������Group����Section
				if (section == null || section is DefaultSection)
					section = GetSectionFromGroups(sectionName, config.SectionGroups);

				FileCacheDependency dependency = new FileCacheDependency(
													true,
													settings.MachineConfigurationFile,
													settings.LocalConfigurationFile,
													settings.MetaConfigurationFile,
													settings.GlobalConfigurationFile);

				ConfigurationSectionCache.Instance.Add(sectionName, section, dependency);
#if DELUXEWORKSTEST
				// ����ʹ��
				ConfigurationBroker.sectionReadFrom = ReadFrom.ReadFromFile;
#endif
			}
			else
			{
#if DELUXEWORKSTEST
				// ����ʹ��
				ConfigurationBroker.sectionReadFrom = ReadFrom.ReadFromCache;
#endif
			}

			return section;

		}

		#endregion

#if DELUXEWORKSTEST
		#region static property and method for test

		public enum ReadFrom
		{
			ReadFromFile, ReadFromCache
		}

		private static ReadFrom configurationReadFrom;
		private static ReadFrom sectionReadFrom;

		public static ReadFrom SectionReadFrom
		{
			get { return ConfigurationBroker.sectionReadFrom; }
		}

		public static ReadFrom ConfigurationReadFrom
		{
			get { return ConfigurationBroker.configurationReadFrom; }
		}

		public static ConfigurationSection GetSectionForTimeDependencyTest(string sectionName)
		{
			ConfigurationSection section;

			ConfigFilesSetting settings = LoadFilesSetting();

			System.Configuration.Configuration config = GetFinalConfiguration(settings);
			section = config.GetSection(sectionName);

			if (section == null)
				section = GetSectionFromGroups(sectionName, config.SectionGroups);

			return section;
		}
		#endregion static property and method for test
#endif
	} // class end
} // namespace end