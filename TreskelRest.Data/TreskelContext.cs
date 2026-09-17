using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace TreskelRest.Data
{
    public class TreskelContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TreskelContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
