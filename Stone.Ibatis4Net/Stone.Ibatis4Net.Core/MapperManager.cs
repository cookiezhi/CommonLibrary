using IBatisNet.DataMapper;
using IBatisNet.DataMapper.SessionStore;
using Stone.Ibatis4Net.Core.Entity;
using System.Collections.Generic;
using System.Configuration;
using iBatis = IBatisNet;

namespace Stone.Ibatis4Net.Core
{
    internal class MapperManager
    {
        private static readonly object Locker = new object();
        private static readonly IDictionary<string, ISqlMapper> Mappers = new Dictionary<string, ISqlMapper>();

        public static ISqlMapper GetMapper(string name)
        {
            lock (Locker)
            {
                if (Mappers.ContainsKey(name))
                {
                    return Mappers[name];
                }
            }

            var resource = name;

            var section = ConfigurationManager.GetSection("StoneIBatisMappers") as StoneIBatisMappers;

            if (section != null && section.Count > 0)
            {
                var m = section.FindLast(map => map.Name == name);
                resource = m.File;
            }

            lock (Locker)
            {
                if (Mappers.ContainsKey(name)) { return Mappers[name]; }

                var mapper = new iBatis.DataMapper.Configuration.DomSqlMapBuilder().Configure(resource);
                mapper.SessionStore = new HybridWebThreadSessionStore(mapper.Id);
                Mappers.Add(name, mapper);
                return mapper;
            }
        }
    }
}