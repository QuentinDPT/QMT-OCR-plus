using Microsoft.AspNetCore.Mvc.RazorPages;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Pages.Script;

public class IndexModel : PageModel
{
    public string DirectoryLocation => Path.GetFullPath(_sequencerStorage.GetDirectory()).Replace("\\", "/");

    public string Id { get; set; }

    public CodeStorageService _sequencerStorage { get; }

    public IndexModel(CodeStorageService sequencerStorage)
    {
        _sequencerStorage = sequencerStorage;
    }

    public void OnGet()
    {
    }
}
