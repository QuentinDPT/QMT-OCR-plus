using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Pages.S
{
    public class IndexModel : PageModel
    {
        public string DirectoryLocation => Path.GetFullPath(_sequencerStorage.GetDirectory()).Replace("\\", "/");

        public string Id { get; set; }

        public SequencerStorageService _sequencerStorage { get; }

        public IndexModel(SequencerStorageService sequencerStorage)
        {
            _sequencerStorage = sequencerStorage;
        }

        public void OnGet()
        {
        }
    }
}
