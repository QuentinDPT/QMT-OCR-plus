using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QMTGroup.Web2.Pages
{
    public class StreamEditorModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public StreamEditorModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
