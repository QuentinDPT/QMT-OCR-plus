using Microsoft.AspNetCore.Mvc;
using QMTGroup.Core;
using QMTGroup.DSL.Lua;
using QMTGroup.Web.Service;
using QMTGroup.WebLogger;
using System.Diagnostics;
using System.Linq.Expressions;

namespace QMTGroup.Web.Controller;

[Route("api/code")]
[ApiController]
public class Code2Controller : ControllerBase
{
    private CodeStorageService _codeStorageService;
    private readonly IWebLogger _webLogger;
    private readonly IServiceProvider _serviceProvider;
    private readonly DSLLuaEngine _engine;

    public Code2Controller(DSLLuaEngine engine, CodeStorageService codeStorageService, IWebLogger webLogger)
    {
        _codeStorageService = codeStorageService;
        _webLogger = webLogger;
        _engine = engine;
    }

    [HttpGet("{id}")]
    public IActionResult GetCodeFromId(string id)
    {
        return Ok(_codeStorageService.GetCode(id));
    }


    [HttpGet("execute/{id}")]
    public async Task ExecuteId(string id, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";

        Exception luaException = null;
        var luaThread = new Thread(() =>
        {
            try
            {
                _executeScript(id);
            }
            catch (ThreadInterruptedException)
            {
                _webLogger.LogWarning("The script was aborted due to a user command");
            }
            catch (Exception ex)
            {
                _webLogger.LogError($"Erreur Lua : {ex.Message}");
                luaException = ex;
            }
        });

        luaThread.Start();

        try
        {
            // Attendre l'annulation ou la fin du script Lua
            while (luaThread.IsAlive)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    luaThread.Interrupt(); // ou Abort() si vraiment nécessaire
                    break;
                }

                await Task.Delay(100); // Attente coopérative
            }

            luaThread.Join(); // attendre qu'il se termine

            if (luaException != null)
            {
                _webLogger.LogError($"Erreur Lua : {luaException.Message}");
            }
        }
        catch (Exception ex)
        {
            _webLogger.LogError($"Erreur : {ex.Message}");
        }
    }

    private void _executeScript(string id)
    {
        DSLLuaCompiled scriptCompiled;

        try
        {
            var script = new DSLLuaScript(id);
            script.ExecutionScript = _codeStorageService.GetCode(id);

            _engine.Clear();

            scriptCompiled = _engine.Compile(script, _webLogger);
        }
        catch (Exception ex)
        {
            _webLogger.LogError(ex, "Something went wrong during the compilation :\n" + ex.Message);
            return;
        }
        Stopwatch sw = new Stopwatch();
        try
        {
            sw.Start();

            scriptCompiled.Invoke();
        }
        catch (Exception ex)
        {
            _webLogger.LogError(ex, "Something went wrong during the execution :\n" + ex.Message);
        }
        finally
        {
            sw.Stop();

            _webLogger.LogTrace("`" + id + "` ran successfully in " + FormatTimeSpan(sw.Elapsed) + " !");
        }
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMilliseconds < 1500)
        {
            return $"{timeSpan.TotalMilliseconds:F3} ms";
        }
        else if (timeSpan.TotalSeconds < 90)
        {
            return $"{timeSpan.TotalSeconds:F2} s";
        }
        else
        {
            return $"{timeSpan.TotalMinutes:F0} min {timeSpan.TotalSeconds:F2} s";
        }
    }

    [HttpPost("{id}")]
    public void SaveCodeFromId(string id, [FromBody] string code)
    {
        _codeStorageService.SaveCode(id, code);
    }
}
