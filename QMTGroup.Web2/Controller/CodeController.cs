using LuaOrchestrator;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Controller;

[Route("api/[controller]88")]
[ApiController]
public class CodeController : ControllerBase
{
    private CodeStorageService _codeStorageService;

    public CodeController(CodeStorageService codeStorageService)
    {
        _codeStorageService = codeStorageService;
    }

    [HttpGet("{id}")]
    public ActionResult<string> GetCodeFromId(string id)
    {
        return Ok(_codeStorageService.GetCode(id));
    }


    [HttpGet("execute/{id}")]
    public ActionResult<string> ExecuteId(string id)
    {
        var script = new LuaScript(id, _codeStorageService.GetCode(id));

        var errorReturn = script.ExecuteDebug(new LuaScriptExecutionOptions());

        if(errorReturn == null)
        {
            return Ok(script.ScriptLines);
        }

        return NotFound(script.ScriptLines);
    }

    [HttpPost("{id}")]
    public void SaveCodeFromId(string id, [FromBody] string code)
    {
        _codeStorageService.SaveCode(id, code);
    }
}
