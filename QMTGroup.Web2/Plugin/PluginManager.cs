using QMTGroup.DSL.Library;
using QMTGroup.Web.Plugin.Model;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace QMTGroup.Web.Plugin;

public static class PluginManager
{
    public static void AddPluginFromConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var pluginsSection = configuration.GetSection("Plugins");
        List<string> pluginsFilesList = pluginsSection.Get<List<string>>() ?? throw new InvalidCastException();

        foreach (string pluginFile in pluginsFilesList)
            _addPluginFrom(services, pluginFile);
    }

    private static void _addPluginFrom(IServiceCollection services, string pluginFile)
    {
        string originalDirectory = Directory.GetCurrentDirectory();

        try
        {
            string pluginFileLocation = Path.GetDirectoryName(pluginFile) ?? string.Empty;
            string pluginFileName = Path.GetFileName(pluginFile);
            Directory.SetCurrentDirectory(pluginFileLocation);
            
            var pluginFileContent = JsonSerializer.Deserialize<PluginFile>(File.ReadAllText(pluginFileName));

            if (pluginFileContent?.Plugins is null)
                throw new ArgumentNullException(nameof(pluginFileContent));

            foreach (var ct in pluginFileContent.Plugins)
            {
                switch (ct.PluginType)
                {
                    case PluginType.VPLUG:
                        AddPlugin(services, ct.Location);
                        break;
                    case PluginType.ASSEMBLY:
                        AddAssembly(services, ct.Location);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
        }
    }


    public static void AddAssembly(this IServiceCollection services, string assemblyLocation)
    {
        var pluginAssembly = Assembly.LoadFrom(assemblyLocation);

        // Chercher les types qui implémentent IMonService
        var typeInterface = typeof(IDSLLibrary);

        var typesImpl = pluginAssembly.GetTypes()
            .Where(t => typeInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in typesImpl)
        {
            services.AddSingleton(type);
        }
    }

    public static void AddPlugin(this IServiceCollection services, string pluginLocation)
    {
        // unzip
        using (FileStream zipStream = new FileStream(pluginLocation, FileMode.Open, FileAccess.Read))
        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
        {
            string lf_filename = Path.GetFileNameWithoutExtension(pluginLocation);
            string lf_dll = lf_filename + ".vplug";
            string lf_properties = lf_filename + ".deps.json";

            PluginDependencies root = _extractDependencyHierarchy(archive, lf_filename, lf_properties);

            // load assembly
            _loadDependencies(root, archive.Entries, out Assembly ass);

            // add to serviceProvider
            var typeInterface = typeof(IDSLLibrary);

            var typesImpl = ass.GetTypes()
                .Where(t => typeInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in typesImpl)
            {
                services.AddSingleton(type);
            }
        }
    }

    private static bool _loadDependencies(PluginDependencies parentPlugin, ReadOnlyCollection<ZipArchiveEntry> sources, out Assembly? assembly)
    {
        bool loadError = false;

        foreach (var childPlugin in parentPlugin.Dependencies)
        {
            loadError |= !_loadDependencies(childPlugin, sources, out Assembly ass);
        }
        assembly = null;

        if(loadError)
            return false;

        try
        {
            var pluginDll = sources.Single(x => x.FullName.EndsWith(parentPlugin.MetaData.Name + ".dll", StringComparison.OrdinalIgnoreCase));

            bool alreadyLoaded = _isAssemblyAlreadyLoaded(pluginDll.FullName);

            if (alreadyLoaded)
                return true;

            // Charger la DLL directement en mémoire
            using (var entryStream = pluginDll.Open())
            using (var ms = new MemoryStream())
            {
                entryStream.CopyTo(ms);
                ms.Position = 0;

                assembly = Assembly.Load(ms.ToArray());
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    private static PluginDependencies _extractDependencyHierarchy(ZipArchive archive, string lf_filename, string lf_properties)
    {
        PluginDependencies root;
        var file_properties = archive.Entries.Single(x => x.Name == lf_properties);

        // read dependencies assemblies
        using (var entryStream = file_properties.Open())
        using (var ms = new MemoryStream())
        {
            entryStream.CopyTo(ms);
            ms.Position = 0;

            using (var reader = new StreamReader(ms))
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();

                var firstTarget = configuration.GetSection("targets").AsEnumerable().Skip(1).First().Key;

                var dependencies = configuration.GetSection(firstTarget).AsEnumerable()
                    .Select(x => new KeyValuePair<string, string>(x.Key.Replace(firstTarget + ":", ""), x.Value ?? string.Empty))
                    .Where(x => x.Key.Contains("dependencies:"))
                    .Select(x =>
                    (
                        Key: new PluginMetaData()
                        {
                            Name = x.Key.Split(":").First().Split("/").First(),
                            Version = x.Key.Split(":").First().Split("/").Last(),
                        },
                        DependsOn: new PluginMetaData()
                        {
                            Name = x.Key.Split(":").Last(),
                            Version = x.Value
                        }
                    ))
                    .ToList();

                root = _createAllDependencies(dependencies.First(x => x.Key.Name == lf_filename).Key, dependencies);
            }
        }

        return root;
    }

    private static PluginDependencies _createAllDependencies(PluginMetaData parent, List<(PluginMetaData, PluginMetaData)> ancestorList)
    {
        return new PluginDependencies()
        {
            MetaData = parent,
            Dependencies = ancestorList
                .Where(x => x.Item1 == parent)
                .Select(x => x.Item2)
                .Select(x => _createAllDependencies(x, ancestorList))
                .ToList(),
        };
    }

    private static bool _isAssemblyAlreadyLoaded(string assemblyName)
    {
        assemblyName = Path.GetFileNameWithoutExtension(assemblyName);

        // Obtenez tous les assemblies chargés dans le domaine d'application actuel
        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Vérifiez si l'assembly avec le nom spécifié est déjà chargé
        return loadedAssemblies.Any(assembly => assembly.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
    }
}
