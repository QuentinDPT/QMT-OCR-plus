namespace QMTGroup.Web.Service;

public class CodeStorageService
{
    private static Dictionary<string, string> _codeStorage = new Dictionary<string, string>();

    private string _folderLocation;

    public CodeStorageService(IConfiguration configuration)
    {
        _folderLocation = configuration["ScriptFolder"] ?? throw new ArgumentNullException("'ScriptFolder' don't exist in the configuration file.");
        Path.GetFullPath(_folderLocation);
        if(!Path.Exists(_folderLocation))
            Directory.CreateDirectory(_folderLocation);
    }

    public string GetCode(string id)
    {
        string? sourceCodeLocation = Path.Combine(_folderLocation, $"{id}.lua");

        if (File.Exists(sourceCodeLocation))
            return File.ReadAllText(sourceCodeLocation);

        if (_codeStorage.ContainsKey(id))
        {
            return _codeStorage[id];
        }
        return "require \"logger\"\n\n-- Fonction d'initialisation\nfunction init()\n  logger.logDebug(\"Initialisation du script\")\n  -- Vous pouvez charger des dépendances, dxf, et autres ressources ici.\nend\n\n-- Boucle de traitement\nfunction execute()\n  logger.logDebug(\"Execution du script\")\n  -- C'est ici que se trouve les opérations de traitement vision.\nend\n";
    }

    public void SaveCode(string id, string code)
    {
        string? sourceCodeLocation = Path.Combine(_folderLocation, $"{id}.lua");

        File.WriteAllText(sourceCodeLocation, code);
        return;

        //if (string.IsNullOrWhiteSpace(code))
        //    _codeStorage.Remove(id);
        //else
        //    _codeStorage[id] = code;
    }

    public string GetDirectory()
        => _folderLocation;
}
