using LuaOrchestrator;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Web.Service;
using QMTGroup.Web.ViewModel;

namespace QMTGroup.Web.Controller;

[Route("api/[controller]")]
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

    [HttpPost("{id}")]
    public void SaveCodeFromId(string id, [FromBody] string code)
    {
        _codeStorageService.SaveCode(id, code);
    }

    [HttpGet("execute/{id}")]
    public ActionResult<string> ExecuteId(string id)
    {
        var script = new LuaScript(id, _codeStorageService.GetCode(id));

        var errorReturn = script.ExecuteDebug(new LuaScriptExecutionOptions());

        if (errorReturn == null)
        {
            return Ok(script.ScriptLines);
        }

        return NotFound(script.ScriptLines);
    }







    [HttpGet]
    public ActionResult<IEnumerable<ScriptStorageItemViewModel>> GetAllScripts()
    {
        try
        {
            return Ok(_codeStorageService.GetAllScripts());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, 500);
        }
    }

    [HttpGet("{scriptName}")]
    public ActionResult<string> GetCodeForFile(string scriptName)
    {
        return Ok(_codeStorageService.GetCode(scriptName));
    }

    [HttpPost]
    public IActionResult Create([FromBody] string scriptName)
    {
        if (_codeStorageService.CreateScript(scriptName))
            return Ok();

        return Problem();
    }

    [HttpDelete]
    public IActionResult Delete([FromBody] string scriptName)
    {
        if (_codeStorageService.Delete(scriptName))
            return Ok();

        return Problem();
    }

    [HttpGet("exists/{scriptName}")]
    public ActionResult<bool> Exists(string scriptName)
    {
        return Ok(_codeStorageService.Exists(scriptName));
    }
}
