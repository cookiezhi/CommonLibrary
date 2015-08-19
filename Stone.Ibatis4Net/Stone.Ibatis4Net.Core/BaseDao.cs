using IBatisNet.DataMapper;
using System;

namespace Stone.Ibatis4Net.Core
{
    public abstract class BaseDao<T> where T : class
    {
        public ISqlMapper Reader { get; set; }

        public ISqlMapper Writer { get; set; }

        public BaseDao()
        {
        }

        public BaseDao(string readName, string writerName)
        {
            this.Reader = MapperManager.GetMapper(readName);
            this.Writer = MapperManager.GetMapper(writerName);
        }

        /// <summary>
        /// 当你要使用Ibatis的NameSpace的时候使用这个方法来调用.
        /// </summary>
        /// <param name="name">xml中的名称.</param>
        /// <returns></returns>
        public virtual string GetStatementName(string name)
        {
            return string.Format("{0}.{1}", typeof(T).Namespace, name);
        }

        /// <summary>
        /// 得到运行时ibatis.net动态生成的SQL
        /// </summary>
        /// <param name="sqlMapper"></param>
        /// <param name="statementName"></param>
        /// <param name="paramObject"></param>
        /// <returns></returns>
        public virtual string GetRuntimeSql(ISqlMapper sqlMapper, string statementName, object paramObject)
        {
            string result;
            try
            {
                var statement = sqlMapper.GetMappedStatement(statementName);
                if (!sqlMapper.IsSessionStarted)
                {
                    sqlMapper.OpenConnection();
                }
                var scope = statement.Statement.Sql.GetRequestScope(statement, paramObject, sqlMapper.LocalSession);
                result = scope.PreparedStatement.PreparedSql;
            }
            catch (Exception ex)
            {
                result = "获取SQL语句出现异常:" + ex.Message;
            }
            return result;
        }
    }
}