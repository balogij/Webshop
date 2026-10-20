using ComputerShopApi.Models; // <-- Fontos a modellek és a DbContext miatt
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ComputerShopApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //var conn = "server=localhost;database=computer;user=root;password=";

            //optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));

            // 1. ITT KELL REGISZTRÁLNI A DB CONTEXT-ET (Ez hiányzott!)
            //var conn = "Server=db74563.databaseasp.net; Database=db74563; Uid=db74563; Pwd=M+j9wS2-Z!p5;";

            //builder.Services.AddDbContext<ComputerShopDbContext>(options =>
            //    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 30)); // vagy amilyen verzió van a szerveren
            builder.Services.AddDbContext<ComputerShopDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                if (builder.Environment.IsDevelopment())
                {
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
                    {
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                }
                else
                {
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));
                    options.UseMySql(connectionString, serverVersion, mysqlOptions =>
                    {
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                }
            });
            builder.Services.AddSwaggerGen();

            // 2. CORS házirend regisztrálása
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowReact", policy => {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // 3. A CORS-nak a többi middleware ELŐTT kell lennie!
            app.UseCors("AllowReact");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}