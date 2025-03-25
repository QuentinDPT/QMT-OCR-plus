namespace QMTGroup.Web.Service;

public class CodeStorageService
{
    private static Dictionary<string, string> _codeStorage = new Dictionary<string, string>();

    public CodeStorageService()
    {

    }

    public string GetCode(string id)
    {
        if (_codeStorage.ContainsKey(id))
        {
            return _codeStorage[id];
        }
        return "require QMT\n\n-- je le préviens\nQMT.LogWarning(\"Attention a toi\")\n\nQMT.LogInfo(\"Oui, tkt\")\n\nQMT.LogError(\"AAARRRHHH\")\n\nimage = QMT.GetImage()\n\nQMT.LogInfo(\"Voici mon image \" .. image)\n\nQMT.DisplayImage(image)";
    }

    public void SaveCode(string id, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            _codeStorage.Remove(id);
        else
            _codeStorage[id] = code;
    }
}
