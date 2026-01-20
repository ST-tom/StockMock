using MongoDB.Driver;
using System.Collections.Concurrent;
using TS.Shared.Extension;

namespace TS.Shared.Sql.MongoDB
{
    public class MongoClientManager : IDisposable
    {
        private static readonly Lazy<MongoClientManager> _lazyInstance = new(() => new MongoClientManager(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static MongoClientManager Instance => _lazyInstance.Value;

        private readonly ConcurrentDictionary<string, MongoClient> clientDic = new();

        private MongoClientManager()
        {
        }

        public MongoClient Get(string key)
        {
            this.clientDic.TryGetValue(key, out var client);

            if (client == null)
                throw new KeyNotFoundException($"未获取MongoDB数据库连接客户端：{key}");

            return client;
        }

        public IMongoDatabase GetDb(string key, string? dbName = null)
        {
            var client = Get(key);

            //目标库的key和database一致
            dbName ??= key;

            var db = client.GetDatabase(dbName);
            return db ?? throw new KeyNotFoundException($"未获取MongoDB数据库{dbName}");
        }

        /// <summary>
        /// 创建客户端
        /// </summary>
        /// <param name="strConnStr"></param>
        /// <param name="timeout">连接超时时间(秒)</param>
        /// <returns></returns>
        public MongoClient Set(string key, string strConnStr, int timeout = 30)
        {
            if (key.IsNullOrWhiteSpace())
                throw new ArgumentException("MongoDb数据库配置key(dbName)不能为空");

            if( strConnStr.IsNullOrWhiteSpace())
                throw new ArgumentException("MongoDb数据库配置连接字符串不能为空");

            var settings = MongoClientSettings.FromConnectionString(strConnStr);
            settings.ConnectTimeout = TimeSpan.FromSeconds(timeout);
            MongoClient client = new(strConnStr);

            this.clientDic.AddOrUpdate(key, client, (key, oldVulue) =>
            {
                oldVulue.Dispose();
                return client;
            });

            return client;
        }

        public void Dispose()
        {
            this.clientDic.ForEach(x => x.Value.Dispose());
            this.clientDic.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
