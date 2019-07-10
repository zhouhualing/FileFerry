namespace WD.Library.Core
{
    public interface IStartupSettings
    {
        string ResourceAssemblyName
        {
            get; set;
        }

        string ApplicationName
        {
            get; set;
        }

        /// <summary>
        /// Gets/Sets the application root path to use.
        /// Use null (default) to use the base directory of the Renix AppDomain.
        /// </summary>
        string ApplicationRootPath
        {
            get; set;
        }

        bool AllowUserAddIns
        {
            get;
            set;
        }
        /// <summary>
        /// Use the file <see cref="ConfigDirectory"/>\AddIns.xml to maintain
        /// a list of deactivated AddIns and list of AddIns to load from
        /// external locations.
        /// The default value is true.
        /// </summary>
        bool AllowAddInConfigurationAndExternalAddIns
        {
            get;
            set;
        }
        /// <summary>
        /// Gets/Sets the directory used to store Renix properties,
        /// settings and user AddIns.
        /// Use null (default) to use "ApplicationData\ApplicationName"
        /// </summary>
        string ConfigDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the data directory used to load resources.
        /// Use null (default) to use the default path "ApplicationRootPath\data".
        /// </summary>
        string DataDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the name used for the properties file (without path or extension).
        /// Use null (default) to use the default name.
        /// </summary>
        string PropertiesName
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the directory used to store the code completion cache.
        /// Use null (default) to disable the code completion cache.
        /// </summary>
        string DomPersistencePath
        {
            get;
            set;
        }
        void AddAddInsFromDirectory(string addInDir);

        /// <summary>
        /// Add the specified .addin file.
        /// </summary>
        void AddAddInFile(string addInFile);
    }
}