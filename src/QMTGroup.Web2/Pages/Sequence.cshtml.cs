using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Pages
{
    public class SequenceModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Ressource { get; set; }

        public string DirectoryLocation => Path.GetFullPath(_sequencerStorage.GetDirectory()).Replace("\\","/");

        public SequencerStorageService _sequencerStorage { get; }


        public SequenceModel(SequencerStorageService sequencerStorage)
        {
            _sequencerStorage = sequencerStorage;
        }


        public void OnGet()
        {
        }
    }
}
