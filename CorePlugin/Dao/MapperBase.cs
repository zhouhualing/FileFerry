using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WD.Library.Core;

namespace WD.CorePlugin
{
    public class MapperBase :IDisposable
    {
        protected volatile ISqlMapper _mapper = null;

        private string fileNamePrefix = "bin/IBatisConfig/";
        private string fileNameAppendix = ".config";
        private string mapperFileName = "sqlmap.config";
        public virtual string MapFileName
        {
            get
            {
                return mapperFileName;
            }
            set
            {
                mapperFileName = value;
            }
        }


        public MapperBase() { }
        public MapperBase(string mapFile)
        {
            if (!string.IsNullOrWhiteSpace(mapFile))
            {
                this.MapFileName = mapFile;
            }
        }

        public void Configure(object obj)
        {
            _mapper = null;
        }

        public ISqlMapSession BeginTrans()
        {
            return Mapper.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }

        public void CommitTrans()
        {
            Mapper.CommitTransaction();
        }

        public virtual void InitMapper()
        {
            ConfigureHandler handler = new ConfigureHandler(Configure);
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            var normalFileName = MapFileName;
            if (!normalFileName.StartsWith(fileNamePrefix,StringComparison.CurrentCultureIgnoreCase))
                normalFileName = fileNamePrefix + normalFileName;
            if (!normalFileName.EndsWith(fileNameAppendix, StringComparison.CurrentCultureIgnoreCase))
                normalFileName += fileNameAppendix;

            _mapper = builder.ConfigureAndWatch(normalFileName, handler);
        }

        public void Dispose()
        {
            if (_mapper != null)
            {
                if (_mapper.IsSessionStarted)
                {
                    _mapper.CloseConnection();
                }
                _mapper = null;
            }
        }

        public void Close()
        {
            Dispose();
        }

        public virtual ISqlMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    InitMapper();
                }
                
                return _mapper;
            }
        }

        public ISqlMapper Get()
        {
            return Mapper;
        }
    }
}  
