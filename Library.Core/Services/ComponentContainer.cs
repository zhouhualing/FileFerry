//using System;
//using System.IO;
//using System.Xml;

//namespace WD.Library.Core
//{
//    internal sealed class ComponentContainer
//    {
//        #region 属性

//        private ContainerBuilder _containerBuilder = null;
//        private IContainer _container = null;
//        private static ComponentContainer _instance = null;
//        private const string SECTION_NAME = "autofac";
//        private const string FILE_NAME = "components.config";
//        private ObjectMap<string, IEntryDataObject> _componentList = new ObjectMap<string, IEntryDataObject>();//保存配置在SectionName=protocol的组件对象
//        #endregion

//        #region 方法

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        private ComponentContainer(string filePath)
//        {
//            // 创建ContainerBuilder对象
//            _containerBuilder = new ContainerBuilder();

//            // 组件类型注册
//            Register(filePath);

//            // 容器：必须在注册后初始化
//            _container = _containerBuilder.Build();
//        }

//        private static object _lockObject = new object(); 

//        /// <summary>
//        /// 获取ComponentContainer对象
//        /// </summary>
//        /// <returns></returns>
//        internal static ComponentContainer GetInstance(string filePath = FILE_NAME)
//        {
//            if (_instance == null)
//            {
//                lock (_lockObject)
//                {
//                    if (_instance == null)
//                    {
//                        _instance = new ComponentContainer(filePath);
//                    }
//                }
//            }

//            return _instance;
//        }

//        internal static ComponentContainer Instance { get { return _instance; } }

//        /// <summary>
//        /// 根据组件类型获取组件对象
//        /// </summary>
//        /// <param name="type">组件接口类型</param>
//        /// <returns></returns>
//        public object Resolve(Type type, string key = null)
//        {
//            try
//            {
//                return ResolveEx(type, key);
//            }
//            catch (Autofac.Core.DependencyResolutionException)
//            {
//                // 重试
//                return ResolveEx(type, key);
//            }
//        }
//        #endregion

//        #region 私有方法
//        /// <summary>
//        /// 组件类型注册
//        /// </summary>
//        private void Register(string filePath)
//        {
//            //注册配置文件autofac节点下的所有组件
//            RegisterAll(filePath);

//            //解析配置文件扩展节点protocol，用于处理接口多继承问题
//            XmlHelper(filePath);
//        }

//        private void XmlHelper(string fileName)
//        {
//            string filePath = fileName;
//            if (!File.Exists(filePath))
//                throw new FileNotFoundException("ConfigurationFileNotFound", filePath);

//            //加载xml配置文件
//            XmlDocument doc = new XmlDocument();
//            doc.Load(filePath);

//            XmlNode root = doc.DocumentElement;
//            if (root.Name.ToLower() == "configuration")
//            {
//                XmlNodeList nodeList = root.ChildNodes;
//                foreach (var node in nodeList)
//                {
//                    if (node is XmlElement)
//                    {
//                        var _node = node as XmlElement;
//                        //找到protocol节点
//                        if (_node.Name.ToLower() == "protocol")
//                        {
//                            //找到component元素（对象化component）
//                            int n = 0;
//                            Type[] types = new Type[_node.ChildNodes.Count];
//                            foreach (XmlElement subElement in _node.SelectNodes("component"))
//                            {
//                                //构造component对象
//                                var entry = new EntryDataObject();
//                                Type type = Type.GetType(subElement.GetAttribute("type"));
//                                entry.SetPropertyValue("type", type);

//                                //以小写字母方式存储
//                                string key = subElement.GetAttribute("key").ToLower();
//                                if (!string.IsNullOrEmpty(key))
//                                {
//                                    entry.SetPropertyValue("key", key);
//                                }

//                                //将component对象保存到_componentList容器,以key-value形式存储
//                                _componentList.Add((string)entry.GetPropertyValue("key"), entry);

//                                //保存type
//                                types[n] = type;
//                                n++;
//                            }
//                            //注册组件类型（以class类型方式重新注册）
//                            _containerBuilder.RegisterTypes(types);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 注册所有组件(autofac节点下配置的组件)
//        /// </summary>
//        private void RegisterAll(string filePath)
//        {
//            if (!File.Exists(filePath))
//            {
//                throw new FileNotFoundException("ConfigurationFileNotFound");
//            }
//            _containerBuilder.RegisterModule(new ConfigurationSettingsReader(SECTION_NAME, filePath));
//        }

//        private object ResolveEx(Type type, string key = null)
//        {
//            using (var lifetimeScope = _container.BeginLifetimeScope())
//            {
//                //key为空，属于接口单继承
//                if (string.IsNullOrEmpty(key))
//                {
//                    //如果没有key，则以接口类型进行解析
//                    if (_container.IsRegistered(type))
//                    {
//                        return lifetimeScope.Resolve(type);
//                    }
//                }
//                else //key不为空，属于接口多继承
//                {
//                    //以小写字母方式获取
//                    key = key.ToLower();
//                    if (_componentList.ContainsKey(key))
//                    {
//                        //如果有key，则以class类型进行解析
//                        Type implemenType = (Type)_componentList.Get(key).GetPropertyValue("type");
//                        if (_container.IsRegistered(implemenType))
//                        {
//                            return lifetimeScope.Resolve(implemenType);
//                        }
//                    }
//                }
//            }
//            return null;
//        }
//        #endregion
//    }
//}
