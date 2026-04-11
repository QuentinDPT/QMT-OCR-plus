using Microsoft.VisualBasic.FileIO;
using QMTGroup.Web.ViewModel;

namespace QMTGroup.Web.Service;

public class CodeStorageService
{
    private static readonly string _defaultScript = "require \"logger\"\n\n-- Fonction d'initialisation\nfunction init()\n  logger.logDebug(\"Initialisation du script\")\n  -- Vous pouvez charger des dépendances, dxf, et autres ressources ici.\nend\n\n-- Boucle de traitement\nfunction execute()\n  logger.logDebug(\"Execution du script\")\n  -- C'est ici que se trouve les opérations de traitement vision.\nend\n";

    private string _folderLocation;

    private readonly string[] _blackList = ["_default"];

    public CodeStorageService(IConfiguration configuration)
    {
        _folderLocation = configuration["ScriptFolder"] ?? throw new ArgumentNullException("'ScriptFolder' don't exist in the configuration file.");
        Path.GetFullPath(_folderLocation);
        if(!Path.Exists(_folderLocation))
            Directory.CreateDirectory(_folderLocation);

        _getOrCreateDefaultScript();
    }

    private string _getOrCreateDefaultScript()
    {
        var defaultFilePath = Path.Combine(_folderLocation, "_default.lua");
        if (!File.Exists(defaultFilePath))
            File.WriteAllText(defaultFilePath, _defaultScript);

        return File.ReadAllText(defaultFilePath);
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






    public IEnumerable<ScriptStorageItemViewModel> GetAllScripts()
    {
        var files = Directory.GetFiles(_folderLocation, "*.lua");

        return files
            .Where(x => !_blackList.Contains(Path.GetFileNameWithoutExtension(x)))
            .Select(x =>
            {
                var fi = new FileInfo(x);

                return new ScriptStorageItemViewModel()
                {
                    Title = Path.GetFileNameWithoutExtension(x),
                    Description = string.Empty,
                    CreationDate = fi.CreationTime,
                    LastUpdateDate = fi.LastWriteTime,
                    ResourceLocation = x,
                };
            });
    }

    public bool CreateScript(string sequenceName)
    {
        string filePath;
        try
        {
            filePath = Path.Combine(_folderLocation, sequenceName + ".lua");
        }
        catch (Exception)
        {
            return false;
        }

        if (File.Exists(filePath))
            return false;

        string defaultSequence = _getOrCreateDefaultScript();

        File.WriteAllText(filePath, defaultSequence);

        return true;
    }

    public bool Delete(string sequenceName)
    {
        string filePath;

        try
        {
            filePath = Path.Combine(_folderLocation, sequenceName + ".lua");
        }
        catch (Exception)
        {
            return false;
        }

        if (!File.Exists(filePath))
            return true;

        FileSystem.DeleteFile(
            filePath,
            UIOption.OnlyErrorDialogs,
            RecycleOption.SendToRecycleBin);

        return true;
    }

    public bool Exists(string sequenceName)
    {
        string filePath;
        try
        {
            filePath = Path.Combine(_folderLocation, sequenceName + ".lua");
        }
        catch (Exception)
        {
            return false;
        }

        return File.Exists(filePath);
    }

    public string GetCode(string sequenceName)
    {
        string filePath;
        try
        {
            filePath = Path.Combine(_folderLocation, sequenceName + ".lua");
        }
        catch (Exception)
        {
            return string.Empty;
        }

        if (!File.Exists(filePath))
            return string.Empty;

        return File.ReadAllText(filePath);
    }

    public string GetDirectory()
        => _folderLocation;
}
