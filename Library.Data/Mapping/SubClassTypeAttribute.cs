using System;
using System.Text;
using System.Collections.Generic;
using WD.Library.Core;

namespace WD.Library.Data.Mapping
{
    /// <summary>
    /// 描述子对象类型的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SubClassTypeAttribute : System.Attribute
    {
        private System.Type type = null;
        private string typeDescription = null;

        /// <summary>
        /// 构造方法，通过类型信息构造
        /// </summary>
        /// <param name="type">类型信息</param>
        public SubClassTypeAttribute(System.Type type)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

            this.type = type;
        }

        /// <summary>
        /// 构造方法，通过类型描述构造
        /// </summary>
        /// <param name="typeDesp">类型描述</param>
        public SubClassTypeAttribute(string typeDesp)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(typeDesp, "typeDesp");
            this.typeDescription = typeDesp;
        }

        /// <summary>
        /// 类型信息
        /// </summary>
        public System.Type Type
        {
            get
            {
                if (this.type == null && string.IsNullOrEmpty(this.typeDescription) == false)
                    this.type = TypeCreator.GetTypeInfo(this.typeDescription);

                return this.type;
            }
        }

        /// <summary>
        /// 类型描述
        /// </summary>
        public string TypeDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.typeDescription) == true && this.type != null)
                    this.typeDescription = this.type.FullName + ", " + this.type.Assembly.FullName;

                return this.typeDescription;
            }
        }

        /// <summary>
        /// 输出类型描述
        /// </summary>
        /// <returns>类型描述</returns>
        public override string ToString()
        {
            return TypeDescription;
        }
    }
}
