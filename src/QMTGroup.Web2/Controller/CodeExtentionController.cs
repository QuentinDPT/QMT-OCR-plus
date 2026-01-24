using LuaOrchestrator;
using Microsoft.AspNetCore.Mvc;

namespace QMTGroup.Web.Controller;

[Route("api/[controller]")]
[ApiController]
public class CodeExtentionController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<string[]> GetExtentionFor(string id)
    {
        switch (id)
        {
            case "QMT":
                return new LuaLibraryUtility(typeof(QMT)).ToLuaLibrary().Select(x => x.Key.ToString()).OrderByDescending(x => x).ToArray();
            case "EmguCV":
                return "ToGrayScales,Rotate,Translate,Scale,Transform".Split(",").OrderByDescending(x => x).ToArray();
            case "Halcon":
                return "ToGrayScales,Rotate,Translate,Scale,Transform".Split(",").OrderByDescending(x => x).ToArray();
            case "Math":
                return new LuaLibraryUtility(typeof(Math)).ToLuaLibrary().Select(x => x.Key.ToString()).OrderByDescending(x => x).ToArray();
        }

        return new string[] { };
    }

    [HttpGet]
    public ActionResult<string[]> GetExtentions()
    {
        return "QMT,EmguCV,Halcon,Cognex,Matlab".Split(",").OrderByDescending(x => x).ToArray();
    }

    [HttpGet("all")]
    public ActionResult<string[]> GetAllExtentions()
    {
        List<string> result = new List<string>();

        foreach(var id in GetExtentions().Value)
        {
            result.AddRange(GetExtentionFor(id).Value.Select(x => id + "." + x));
        }

        return result.ToArray();
    }
}
