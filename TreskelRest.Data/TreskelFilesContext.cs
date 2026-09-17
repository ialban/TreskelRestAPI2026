using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TreskelRest.Data
{
    public class TreskelFilesContext
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase _mongoDatabase;

        public TreskelFilesContext(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("MongoConnection");
            var dbName = _configuration.GetSection("ConnectionStrings:MongoDatabase").Value;

            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase(dbName);
        }

        public IMongoDatabase GetMongoDatabase() => _mongoDatabase;
    }
}
