using Microsoft.Extensions.Configuration;
using Mongo2Go;

namespace TreskelRest.Data.Test.Base
{
    [TestFixture]
    public class MongoUTestBase
    {
        protected MongoDbRunner _runner;
        protected TreskelFilesContext _context;
        protected ITreskelFilesRepository _repo;
        private string _connectionString;

        [SetUp]
        public void Setup()
        {
            _runner = MongoDbRunner.Start();

            _connectionString = "mongodb://trsklusr:Tr3sk3lS0ft@localhost:27017/TreskelRestFiles?authSource=TreskelRestFiles&authMechanism=SCRAM-SHA-256";


            // Se hace un mock de la conexión a la base de datos
            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:MongoConnection", _connectionString},
                {"ConnectionStrings:MongoDatabase", "TreskelRestFiles" }
            };

            IConfiguration _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _context = new TreskelFilesContext(_config);
            _repo = new TreskelFilesRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            if (_runner != null)
            {
                _runner.Dispose();
                _runner = null;
            }
        }

    }
}
