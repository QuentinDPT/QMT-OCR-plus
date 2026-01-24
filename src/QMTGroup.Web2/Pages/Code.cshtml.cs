using Microsoft.AspNetCore.Mvc.RazorPages;
using QMTGroup.Web2.Pages;

namespace QMTGroup.Web.Pages
{
    public class CodeModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public CodeModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
