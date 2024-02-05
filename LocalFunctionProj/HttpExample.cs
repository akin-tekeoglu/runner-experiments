using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace LocalFunctionProj
{
    public class User{
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class HttpExample
    {
        private readonly ILogger _logger;

        public HttpExample(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpExample>();
        }

        [Function("HttpExample")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request. some backend changes");

            string connectionString = "Host=db;Username=root;Password=1234;Database=test";
            var users=new List<User>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("SELECT id,username,email,created_at FROM users", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User{
                                Id=reader.GetInt32(reader.GetOrdinal("id")),
                                Username=reader.GetString(reader.GetOrdinal("username")),
                                Email=reader.GetString(reader.GetOrdinal("email")),
                                CreatedAt=reader.GetDateTime(reader.GetOrdinal("created_at"))
                            });
                        }
                    }
                }

                connection.Close();
            }

           
            return new OkObjectResult(users);
        }
    }
}
