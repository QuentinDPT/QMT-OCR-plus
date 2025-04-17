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
        return "function init()\r\n  print(\"Hello world !\")\r\nend\r\n\r\nfunction execute()\r\n  \r\nend";
    }

    public void SaveCode(string id, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            _codeStorage.Remove(id);
        else
            _codeStorage[id] = code;
    }
}
