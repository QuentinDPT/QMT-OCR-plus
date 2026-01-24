using LuaOrchestrator.Error;
using Neo.IronLua;
using System.Diagnostics;

namespace LuaOrchestrator;

public class LuaScript
{
    public LuaScript(string scriptName, string[] scriptLines)
    {
        _scriptName = scriptName;
        _scriptLines = new LuaScriptLine[scriptLines.Length];
        _luaEngine = new Lua();
        _global = _luaEngine.CreateEnvironment<LuaGlobal>();

        for (int i = 0; i < scriptLines.Length; i++)
        {
            if (scriptLines[i].StartsWith("require "))
            {
                var require = scriptLines[i].Replace("require", "").Replace("\'", "").Trim();

                LuaTable? lib = null;

                switch(require)
                {
                    case "EmguCv":
                        break;
                    case "Math":
                        lib = new LuaLibraryUtility(typeof(Math)).ToLuaLibrary();
                        break;
                    case "QMT":
                        lib = new LuaLibraryUtility(typeof(QMT)).ToLuaLibrary();
                        break;
                }

                if(lib == null)
                {
                    _scriptLines[i] = new LuaScriptLine(i, "-- `" + scriptLines[i] + "` library not found")
                    {
                        Warning = "`" + scriptLines[i] + "` library not found"
                    };

                }
                else
                {
                    _global[require] = lib;
                    _scriptLines[i] = new LuaScriptLine(i, "-- Match found with local library \"" + scriptLines[i] + "\"");
                }
            }
            else
            {
                _scriptLines[i] = new LuaScriptLine(i, scriptLines[i]);
            }
        }
    }

    public LuaScript(string scriptName, string scriptLines)
        : this(scriptName, scriptLines.Replace("\r\n","\n").Split("\n"))
    { }

    public ErrorModel? Execute()
    {
        List<LuaScriptLine> lines = _executableScriptLines.ToList();
        int executableLineIndex = 0;
        int executableLineCount = lines.Count();
        try
        {
            for(executableLineIndex = 0; executableLineIndex < executableLineCount; executableLineIndex++)
            {
                _global.dochunk(lines[executableLineIndex].LineMinified, ScriptName);
            }
        }catch(Exception ex)
        {
            Console.WriteLine($"Script error in \"{ScriptName}\" on line {lines[executableLineIndex].LineIndex +1} : {ex.Message}");
            return new ErrorModel()
            {
                Errors = new List<ErrorElement>()
                {
                    new ErrorElement()
                    {
                        Title = "Error",
                        Line = lines[executableLineIndex].LineIndex + 1,
                        Description = ex.Message,
                        Severity = "Fatal"
                    }
                }
            };
        }

        return null;
    }

    public ErrorModel? ExecuteDebug(LuaScriptExecutionOptions scriptExecutionOptions)
    {
        Stopwatch stopwatch = new Stopwatch();
        List<LuaScriptLine> lines = _executableScriptLines.ToList();
        int executableLineIndex = 0;
        int executableLineCount = lines.Count();

        for (executableLineIndex = 0; executableLineIndex < executableLineCount; executableLineIndex++)
        {
            try
            {
                if(scriptExecutionOptions.TimedScriptLine)
                {
                    stopwatch.Restart();
                    _global.dochunk(lines[executableLineIndex].LineMinified, ScriptName);
                    stopwatch.Stop();
                    lines[executableLineIndex].ExecutionDuration = stopwatch.ElapsedTicks;
                }
                else
                {
                    _global.dochunk(lines[executableLineIndex].LineMinified, ScriptName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Script error in \"{ScriptName}\" on line {lines[executableLineIndex].LineIndex} : {ex.Message}");

                lines[executableLineIndex].Error = "FATAL " + ex.Message;

                return new ErrorModel()
                {
                    Errors = new List<ErrorElement>()
                {
                    new ErrorElement()
                    {
                        Title = "Error",
                        Line = lines[executableLineIndex].LineIndex + 1,
                        Description = ex.Message,
                        Severity = "Fatal"
                    }
                }
                };
            }
        }

        return null;
    }


    public string ScriptName => _scriptName;
    private string _scriptName;

    private IEnumerable<LuaScriptLine> _executableScriptLines => _scriptLines.Where(x => x.Executable);

    public IEnumerable<LuaScriptLine> ScriptLines => _scriptLines.AsEnumerable();

    private LuaScriptLine[] _scriptLines;

    private Lua _luaEngine;

    private dynamic _global;
}
