
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace WD.Library.Core
{
	public class PropertyService : PropertyServiceImpl
	{
		DirectoryName configDirectory;
        DirectoryName dataDirectory;
        FileName propertiesFileName;

		public PropertyService(DirectoryName configDirectory, DirectoryName dataDirectory, string propertiesName)
			: base(LoadPropertiesFromStream(configDirectory.CombineFile(propertiesName + ".xml")))
		{
			this.configDirectory = configDirectory;
            this.dataDirectory = dataDirectory;
            propertiesFileName = configDirectory.CombineFile(propertiesName + ".xml");
		}
		
		public override DirectoryName ConfigDirectory {
			get {
				return configDirectory;
			}
		}
        public override DirectoryName DataDirectory
        {
            get
            {
                return dataDirectory;
            }
        }
        static DataProperties LoadPropertiesFromStream(FileName fileName)
        {
            if (!File.Exists(fileName))
            {
                return new DataProperties();
            }
            try
            {
                using (LockPropertyFile())
                {
                    return DataProperties.Load(fileName);
                }
            }
            catch (XmlException ex)
            {
            }
            catch (IOException ex)
            {
            }
            return new DataProperties();
        }
		
		public override void Save()
		{
			using (LockPropertyFile()) {
				this.MainPropertiesContainer.Save(propertiesFileName);
			}
		}
		
		/// <summary>
		/// Acquires an exclusive lock on the properties file so that it can be opened safely.
		/// </summary>
		static IDisposable LockPropertyFile()
		{
			Mutex mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
			mutex.WaitOne();
			return new ActionOnDispose(
				delegate {
					mutex.ReleaseMutex();
					mutex.Close();
				});
		}
		
		FileName GetExtraFileName(string key)
		{
			return configDirectory.CombineFile("preferences/" + key.GetStableHashCode().ToString("x8") + ".xml");
		}
		
		public override DataProperties LoadExtraProperties(string key)
		{
			var fileName = GetExtraFileName(key);
			using (LockPropertyFile()) {
				if (File.Exists(fileName))
					return DataProperties.Load(fileName);
				else
					return new DataProperties();
			}
		}
		
		public override void SaveExtraProperties(string key, DataProperties p)
		{
			var fileName = GetExtraFileName(key);
			using (LockPropertyFile()) {
				Directory.CreateDirectory(fileName.GetParentDirectory());
				p.Save(fileName);
			}
		}

    }

    public static class CustomExtension
    {

        public static int GetStableHashCode(this string text)
        {
            unchecked
            {
                int h = 0;
                foreach (char c in text)
                {
                    h = (h << 5) - h + c;
                }
                return h;
            }
        }
    }
    }
