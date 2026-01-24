using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QMTGroup.Web.Service;
using QMTGroup.Web2.Pages;

namespace QMTGroup.Web.Pages
{
    public class Code2Model : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Ressource { get; set; }

        public string DirectoryLocation => Path.GetFullPath(_codeStorage.GetDirectory()).Replace("\\", "/");

        private readonly ILogger<IndexModel> _logger;
        private readonly CodeStorageService _codeStorage;

        public Code2Model(ILogger<IndexModel> logger, CodeStorageService codeStorage)
        {
            _logger = logger;
            _codeStorage = codeStorage;
        }

        public void OnGet()
        {
        }
    }
}
