using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Web.Service;
using QMTGroup.Web.ViewModel;

namespace QMTGroup.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SequencerController : ControllerBase
    {
        public SequencerStorageService SequencerStorageService { get; }


        public SequencerController(SequencerStorageService sequencerStorageService)
        {
            SequencerStorageService = sequencerStorageService;
        }


        [HttpGet]
        public ActionResult<IEnumerable<SequenceStorageItemViewModel>> GetAllSequences()
        {
            try
            {
                return Ok(SequencerStorageService.GetAllSequences());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, null, 500);
            }
        }

        [HttpGet("{sequenceName}")]
        public ActionResult<string> GetCodeForFile(string sequenceName)
        {
            return Ok(SequencerStorageService.GetCode(sequenceName));
        }

        [HttpPost]
        public IActionResult Create([FromBody]string sequenceName)
        {
            if(SequencerStorageService.CreateSequence(sequenceName))
                return Ok();

            return Problem();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody]string sequenceName)
        {
            if (SequencerStorageService.Delete(sequenceName))
                return Ok();

            return Problem();
        }

        [HttpGet("exists/{sequenceName}")]
        public ActionResult<bool> Exists(string sequenceName)
        {
            return Ok(SequencerStorageService.Exists(sequenceName));
        }
    }
}
