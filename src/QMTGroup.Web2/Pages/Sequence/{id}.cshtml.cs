using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Pages.S
{
    public class ItemModel : PageModel
    {
        public string DirectoryLocation => Path.GetFullPath(_sequencerStorage.GetDirectory()).Replace("\\", "/");

        public string Id { get; set; }

        public SequencerStorageService _sequencerStorage { get; }

        public ItemModel(SequencerStorageService sequencerStorage)
        {
            _sequencerStorage = sequencerStorage;
        }

        public void OnGet()
        {
            Id = RouteData.Values["id"]?.ToString();
        }
    }
}
