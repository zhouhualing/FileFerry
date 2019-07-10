using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD.Library.Core
{
    public class ClientConfig
    {
        public string ChatServer { get; set; } = "127.0.0.1:9999";
        public string FtpServer { get; set; } = "127.0.0.1:9998";
        public string HttpServer { get; set; } = "127.0.0.1:9997";
        public int Port { get; set; } = 39654; 
        public string Url { get; set; } = "http://127.0.0.1:39655/File/"; //文件服务器
        public int InitBufferSize { get; set; } = 1024 * 10; //解析命令初始缓存大小 
    }
}
