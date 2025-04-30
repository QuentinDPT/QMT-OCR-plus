using Microsoft.Extensions.Logging;
using QMTGroup.DSL.Core;
using QMTGroup.DSL.Library;
using System.Text.RegularExpressions;

namespace QMTGroup.DSL.Lua
{
    public class DSLLuaEngine : IDSLEngine<DSLLuaScript>
    {
        public IEnumerable<IDSLLibrary> Libraries => _libraries;
        private readonly List<IDSLLibrary> _libraries = new();

        private readonly List<DSLLuaCompiled> _compiledScripts = new();

        private readonly DSLLuaLibraryFactory _libFactory;
        private readonly ILogger<DSLLuaEngine> _engineLogger;

        private NLua.Lua _engine;

        internal NLua.Lua Engine => _engine;

        public DSLLuaEngine(DSLLuaLibraryFactory libFactory, ILogger<DSLLuaEngine> engineLogger)
        {
            _engine = new();

            _libFactory = libFactory;
            _engineLogger = engineLogger;

            Clear();
        }

        public DSLLuaCompiled Compile(DSLLuaScript script)
        {
            using (var c = _compiledScripts.SingleOrDefault(x => x.Name == script.Name)) {
                if (c is not null)
                {
                    _compiledScripts.Remove(c);
                    c.Dispose();
                }
            }
            string code = _addCodeRequirements(script.ExecutionScript);

            _engine.LoadString(code, script.Name).Call();

            if (_engine["main"] is null)
            {
                string mainCode = "__initialized = false\r\nfunction main()\r\n";

                if (_engine["init"] is not null) {
                    mainCode += "\tif not __initialized then\r\n\t\tinit()\r\n\t\t__initialized = true\r\n\tend";
                }

                if (_engine["execute"] is not null) {
                    mainCode += "\texecute()\r\n";
                }

                mainCode += "end";

                _engine.DoString(mainCode);
            }

            DSLLuaCompiled compiled = new DSLLuaCompiled(this, _engineLogger)
            {
                Name = script.Name,
            };
            _compiledScripts.Add(compiled);
            return compiled;
        }

        /// <summary>
        /// Adds code requirements from lua.<br/>
        /// If the lua code contains <c>require XXX</c> this function will find the library with the <see cref="DSLLuaLibraryFactory"/>.
        /// </summary>
        /// <param name="executionScript">Lua code</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string _addCodeRequirements(string executionScript)
        {
            var requireRegex = new Regex(@"^\s*require\s+(?:[""']?)([\w\d_.]+)(?:[""']?)", RegexOptions.Multiline);

            var matches = requireRegex.Matches(executionScript);
            var uniqueLibs = new HashSet<string>();

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string libName = match.Groups[1].Value;
                    if (!uniqueLibs.Contains(libName))
                    {
                        uniqueLibs.Add(libName);

                        IDSLLibrary? lib = _libFactory.Get(libName);
                        if (lib is not null)
                        {
                            lib.AddToEngine(this);
                        }
                    }
                }
            }

            string modifiedScript = requireRegex.Replace(executionScript, match => $"-- {match.Value}");

            return modifiedScript;
        }

        private bool _addLibrary(string libName)
        {
            var lib = _libFactory.Get(libName);
            if (lib is null)
                return false;

            lib.AddToEngine(this);
            return true;
        }

        public void Clear()
        {
            _engine.Dispose();
            _engine = new NLua.Lua();

            _addLibrary("stdLib");
        }

        public void Dispose()
        {
            foreach (var item in _compiledScripts)
            {
                item.Dispose();
            }

            _engine.Dispose();
        }
    }
}
