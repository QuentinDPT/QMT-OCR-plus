using PluginBidon;
using QMTGroup.Core;
using QMTGroup.DSL.Hub;
using QMTGroup.DSL.Library.EmguCV;
using QMTGroup.DSL.Library.Math;
using QMTGroup.DSL.Library.Standard;
using QMTGroup.DSL.Lua;
using QMTGroup.Image.Interface;
using QMTGroup.Web.Factory;
using QMTGroup.Web.Service;
using System.Reflection;
using System.Runtime;

namespace QMTGroup.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;


            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls(["https://localhost:7083", "http://localhost:5062", "https://0.0.0.0:7083", "http://0.0.0.0:5062"]);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddSignalR();

            builder.Services.AddSingleton<AssemblyTypes>(_ => new(AppDomain.CurrentDomain.GetAssemblies()));

            builder.Services.AddSingleton<MathLib>();
            builder.Services.AddSingleton<FeatureBidonPlugin>();
            builder.Services.AddSingleton<StdLib>();
            builder.Services.AddSingleton<LogLib>();
            builder.Services.AddSingleton<EmguCVLib>();

            builder.Services.AddSingleton<IResourceHub, ResourceHub>();
            builder.Services.AddSingleton<DSLLuaEngine>();
            builder.Services.AddSingleton<DSLLuaLibraryFactory>();

            builder.Services.AddSingleton<VideoStreamService>();
            builder.Services.AddSingleton<ICameraFactory, CameraFactory>();
            builder.Services.AddSingleton<IJpegConverter, Image.EmguCV.CodecConverter>();
            builder.Services.AddSingleton<OverlayService>();
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
            app.UseAuthorization();
            app.MapControllers();

            app.MapRazorPages();

            var cameraFactory = app.Services.GetService<ICameraFactory>();
            if (cameraFactory is null)
                throw new NullReferenceException(nameof(cameraFactory));

            cameraFactory.Create<Camera.EmguCV.Camera>(new Camera.EmguCV.CameraParameters()
            {
                Slot = 0,
                FlipHorizontal = true,
                UserParamters = new Dictionary<Urn.Urn, double>()
                {
                    //{ new Urn.Urn("urn:Fps"), 10 },
                    /*
                    // 720p
                    { new Urn.Urn("urn:FrameWidth"), 1280 },
                    { new Urn.Urn("urn:FrameHeight"), 720 },
                    //*/
                    /*
                    // 1080p
                    { new Urn.Urn("urn:FrameWidth"), 1920 },
                    { new Urn.Urn("urn:FrameHeight"), 1080 },
                    //*/
                }
            });
            cameraFactory.Create<Camera.Halcon.Camera>(null);
            cameraFactory.Create<Camera.File.Camera>(new Camera.File.CameraParameters()
            {
                Path = @"..\rgb_calibration.png",
                AcquisitionLoopSleep = 10,
                //Path = @"..\heavy.jpg"
            });

            app.Run();
        }
    }
}
