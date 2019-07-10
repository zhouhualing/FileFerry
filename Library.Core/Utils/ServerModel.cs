using System;
using System.IO;

namespace WD.Library.Core
{
    public class ServerConfig
    {
        /// <summary>
        ///     服务器监听IP
        /// </summary>
        public string IP { get; set; } = "127.0.0.1"; //服务器监听IP

        /// <summary>
        ///     服务器监听端口
        /// </summary>
        public int Port { get; set; } = 39654; //服务器监听端口  

        
        public int FilePort { get; set; } = 39655;

        /// <summary>
        ///     解析命令初始缓存大小
        /// </summary>
        public int InitBufferSize { get; set; } = 1024 * 10; //解析命令初始缓存大小  

        /// <summary>
        ///     最大连接数
        /// </summary>
        public int MaxClientSize { get; set; } = 10000; //最大连接数
    }
}
