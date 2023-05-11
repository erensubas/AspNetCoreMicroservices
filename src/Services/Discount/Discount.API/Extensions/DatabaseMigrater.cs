using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Extensions
{
    public class DatabaseMigrater : IHostedService
    {
        private readonly IConfiguration _configuration;

        public DatabaseMigrater(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            bool isNotComplete = true;
            int counter = 0;

            while(isNotComplete)
            {
                try
                {
                    counter++;
                    using var connection = new NpgsqlConnection
                    (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

                    await connection.ExecuteAsync("DROP TABLE IF EXISTS Coupon");

                    await connection.ExecuteAsync(@"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)");

                    await connection.ExecuteAsync("INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);");


                    await connection.ExecuteAsync("INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);");

                    isNotComplete = false;

                }
                catch (Exception ex)
                {
                    counter++;
                    if(counter > 50)
                    {
                        isNotComplete = false;
                        throw ex;
                    }
                }
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
