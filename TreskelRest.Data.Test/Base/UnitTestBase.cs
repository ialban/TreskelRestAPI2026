using Microsoft.Extensions.Configuration;

namespace TreskelRest.Data.Test.Base
{
    public class UnitTestBase: IDisposable
    {
        protected TreskelContext _context;
        protected ITreskelRepository _repo;
        private string _connectionString;

        //----------------------------------------------------
        private bool disposedValue;

        [SetUp]
        public void Setup()
        {
            _connectionString = "Server=localhost;Database=ServNube;User Id=trsklusr;Password=Tr3sk3lS0ft;TrustServerCertificate=True;";

            // Se hace un mock de la conexión a la base de datos
            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:DefaultConnection", _connectionString}
            };

            IConfiguration _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _context = new TreskelContext(_config);
            _repo = new TreskelRepository(_context, true, true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminar el estado administrado (objetos administrados)
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
