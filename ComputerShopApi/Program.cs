using ComputerShopApi.Models; // <-- Fontos a modellek és a DbContext miatt
using Microsoft.EntityFrameworkCore;

namespace ComputerShopApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. ITT KELL REGISZTRÁLNI A DB CONTEXT-ET (Ez hiányzott!)
            var conn = "Server=db74563.databaseasp.net; Database=db74563; Uid=db74563; Pwd=M+j9wS2-Z!p5;";
            //builder.Services.AddDbContext<ComputerShopDbContext>(options =>
            //    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 30)); // vagy amilyen verzió van a szerveren
            builder.Services.AddDbContext<ComputerShopDbContext>(options =>
                options.UseMySql(conn, serverVersion));
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