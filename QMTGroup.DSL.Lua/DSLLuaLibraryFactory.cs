using Microsoft.Extensions.DependencyInjection;
using QMTGroup.Core;
using QMTGroup.DSL.Library;
using System.Reflection;

namespace QMTGroup.DSL.Lua;

public class DSLLuaLibraryFactory
{
    public DSLLuaLibraryFactory(AssemblyTypes executionContext, IServiceProvider serviceProvider)
    {
        _availableLibrariesTypes = _librariesIdentification(executionContext).ToList();
        _serviceProvider = serviceProvider;
    }

    private static IEnumerable<(Type, string)> _librariesIdentification(AssemblyTypes context)
    {
        Type libInterface = typeof(IDSLLibrary);

        var implementations = context.Types
            .Where(t => libInterface.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(x => (x, _extractLibraryNamespace(x)));


        return implementations;
    }

    private static string _extractLibraryNamespace(Type libraryType)
    {
        return _decapitalize(libraryType.GetCustomAttribute<DSLNamespaceAttribute>()?.LibraryName ?? libraryType.Name);
    }
    private static string _decapitalize(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            return name;

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    private List<(Type, string)> _availableLibrariesTypes = new List<(Type, string)>();

    private readonly IServiceProvider _serviceProvider;

    public IDSLLibrary? Get(string name)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService(
                    _availableLibrariesTypes.Single(x => x.Item2 == name).Item1
                ) as IDSLLibrary ?? throw new InvalidCastException();
            }
        }catch(Exception ex)
        {
            throw new DSLLibraryNotFound(name);
        }
    }
}
