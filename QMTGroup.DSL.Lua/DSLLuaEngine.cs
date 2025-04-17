using Emgu.CV.Structure;
using QMTGroup.DSL.Core;
using QMTGroup.DSL.Hub;
using QMTGroup.DSL.Library;
using QMTGroup.DSL.Library.EmguCV;
using QMTGroup.DSL.Library.Math;
using QMTGroup.DSL.Library.Standard;
using System.Text.RegularExpressions;

namespace QMTGroup.DSL.Lua
{
    public class DSLLuaEngine : IDSLEngine<DSLLuaScript>
    {
        public IEnumerable<IDSLLibrary> Libraries => _libraries;
        private readonly List<IDSLLibrary> _libraries = new();

        private readonly List<DSLLuaCompiled> _compiledScripts = new();

        private readonly DSLLuaLibraryFactory _libFactory;

        private NLua.Lua _engine;

        internal NLua.Lua Engine => _engine;

        public DSLLuaEngine(DSLLuaLibraryFactory libFactory)
        {
            _engine = new();

            _libFactory = libFactory;

            _addLibrary("stdLib");
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

            DSLLuaCompiled compiled = new DSLLuaCompiled(this)
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
            // Regex pour matcher les lignes "require xxx", avec ou sans guillemets
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

            // Commente les lignes contenant require
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
