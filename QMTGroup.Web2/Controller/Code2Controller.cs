using Microsoft.AspNetCore.Mvc;
using QMTGroup.DSL.Lua;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Controller;

[Route("api/code")]
[ApiController]
public class Code2Controller : ControllerBase
{
    private CodeStorageService _codeStorageService;
    private readonly DSLLuaEngine _engine;

    public Code2Controller(DSLLuaEngine engine, CodeStorageService codeStorageService)
    {
        _codeStorageService = codeStorageService;
        _engine = engine;
    }

    [HttpGet("{id}")]
    public ActionResult<string> GetCodeFromId(string id)
    {
        return Ok(_codeStorageService.GetCode(id));
    }


    [HttpGet("execute/{id}")]
    public ActionResult<string> ExecuteId(string id)
    {
        try
        {
            var script = new DSLLuaScript(id);
            script.ExecutionScript = _codeStorageService.GetCode(id);

            var scriptCompiled = _engine.Compile(script);

            scriptCompiled.Initialize();

            scriptCompiled.Execute();

            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}")]
    public void SaveCodeFromId(string id, [FromBody] string code)
    {
        _codeStorageService.SaveCode(id, code);
    }
}
