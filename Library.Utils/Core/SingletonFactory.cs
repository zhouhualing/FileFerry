
namespace WD.Library.Core
{
    public sealed class SingletonFactory<T> where T : new()
    {
        private static T instance = new T();

        private SingletonFactory()
        {
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
