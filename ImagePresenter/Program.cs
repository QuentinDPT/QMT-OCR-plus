
using System.Diagnostics;

namespace ImagePresenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int numThreads = Environment.ProcessorCount;
            Console.WriteLine($"Nombre de cœurs logiques : {numThreads}");

            var p = Process.GetCurrentProcess();
            Console.WriteLine($"Nombre de thread utilisés : {p.Threads.Count}");


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<MemoryStreamService>();
            builder.Services.AddSingleton<ImageStreamService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
