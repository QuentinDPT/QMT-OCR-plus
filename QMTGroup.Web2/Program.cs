using QMTGroup.Camera;
using QMTGroup.Web;
using QMTGroup.Web.Service;
using QMTGroup.Web2.Service;

namespace QMTGroup.Web2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddSignalR();
            builder.Services.AddSingleton<VideoStreamService>();
            builder.Services.AddSingleton<ICamera, Camera.Halcon.Camera>();
            builder.Services.AddSingleton(x => new Camera.EmguCV.CameraParameters()
            {
                Slot = 0
            });
            builder.Services.AddSingleton<CodeStorageService>();

            builder.Logging.ClearProviders(); // Désactive tous les loggers
            builder.Logging.AddConsole();
            builder.Logging.AddDebug(); // Active seulement les logs en mode debug

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<VideoHub>("/videoHub");
                endpoints.MapControllers();
            });

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
