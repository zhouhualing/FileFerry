using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WD.CorePlugin
{
    public class MySqlMapper:MapperBase
    {   
        public override string MapFileName
        {
            get; set;
        } = "mysqlmap.config";

        public MySqlMapper() { }
        public MySqlMapper(string mapFile):base(mapFile)
        {

        }
    }
}
