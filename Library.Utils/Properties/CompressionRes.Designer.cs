﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WD.Library.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CompressionRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CompressionRes() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WD.Library.Properties.CompressionRes", typeof(CompressionRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 CRC校验失败 的本地化字符串。
        /// </summary>
        internal static string InvalidCRC {
            get {
                return ResourceManager.GetString("InvalidCRC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 非法的压缩文件目录区 的本地化字符串。
        /// </summary>
        internal static string InvalidFileDir {
            get {
                return ResourceManager.GetString("InvalidFileDir", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 非法的压缩文件目录结束标志 的本地化字符串。
        /// </summary>
        internal static string InvalidFileEnding {
            get {
                return ResourceManager.GetString("InvalidFileEnding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 非法的压缩文件头结构 的本地化字符串。
        /// </summary>
        internal static string InvalidFileHeader {
            get {
                return ResourceManager.GetString("InvalidFileHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 非法的压缩算法编号 的本地化字符串。
        /// </summary>
        internal static string InvalidMethod {
            get {
                return ResourceManager.GetString("InvalidMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 非法的SFX文件 的本地化字符串。
        /// </summary>
        internal static string InvalidSFXFile {
            get {
                return ResourceManager.GetString("InvalidSFXFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 存在非法的文件流 的本地化字符串。
        /// </summary>
        internal static string InvalidStream {
            get {
                return ResourceManager.GetString("InvalidStream", resourceCulture);
            }
        }
    }
}
