using System.Collections;
using System.Reflection;

namespace QMTGroup.Core;

/// <summary>
/// This class is used when you want to find a type available in the current assembly.
/// </summary>
public class AssemblyTypes : IEnumerable<Type>
{
    public IEnumerable<Type> Types { get; private set; }

    public AssemblyTypes(Assembly[] assemblys)
    {
        Types = assemblys.Select(x => x.GetTypes()).SelectMany(x => x);
    }

    public IEnumerator<Type> GetEnumerator()
    {
        return Types.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
