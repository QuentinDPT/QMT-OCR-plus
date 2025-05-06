using QMTGroup.Core;
using QMTGroup.DSL.Library;

namespace QMTGroup.DSL.Lua;

public class DSLLuaLibraryFactory
{
    public DSLLuaLibraryFactory(AssemblyTypes executionContext, IServiceProvider serviceProvider)
    {
        _loadLibrariesFromAssembly(executionContext, serviceProvider);
    }

    private void _loadLibrariesFromAssembly(AssemblyTypes context, IServiceProvider serviceProvider)
    {
        _availableLibraries.Clear();

        Type libInterface = typeof(IDSLLibrary);

        IEnumerable<Type> implementations = context.Types
            .Where(t => libInterface.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var implementation in implementations)
        {
            IDSLLibrary service;
            try
            {
                service = serviceProvider.GetService(implementation) as IDSLLibrary ?? throw new InvalidCastException();
            }
            catch(Exception ex)
            {
                continue;
            }
            _availableLibraries.Add(service);
        }
    }

    public IEnumerable<IDSLLibrary> AvailableLibraries => _availableLibraries;
    private List<IDSLLibrary> _availableLibraries = new();

    public IDSLLibrary? Get(string name) => AvailableLibraries.SingleOrDefault(x => x.GetNamespace() == name);
}
