using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD.CorePlugin
{
    public class SqlServerMapper:MapperBase
    {   
        public override string MapFileName
        {
            get; set;
        } = "sqlservermap.config";

        public SqlServerMapper() { }
        public SqlServerMapper(string mapFile):base(mapFile)
        {

        }
    }
}
