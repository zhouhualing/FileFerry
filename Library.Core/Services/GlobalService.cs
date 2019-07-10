using System;
using System.Threading.Tasks;

namespace WD.Library.Core
{
    public static class GlobalService
	{
        public static ICoreServiceContainer ServiceContainer
        {
            get
            {
                return CoreServiceContainer.Default;
            }
        }

		public static T GetService<T>() where T : class
		{
            return ServiceContainer.Resolve<T>();
		}

        public static T GetService<T>(string serviceName) where T : class
        {
            return ServiceContainer.Resolve<T>(serviceName);
        }


        public static T Resolve<T>() where T : class
        {
            return ServiceContainer.Resolve<T>();
        }

        public static T Resolve<T>(string serviceName) where T : class
        {
            return ServiceContainer.Resolve<T>(serviceName);
        }
    }
}
