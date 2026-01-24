using System.Runtime.CompilerServices;

namespace QMTGroup.DSL.Library.Standard;

internal static class ObjectExtensions
{
    public static string ToHTML(this object? self)
    {
        return self?.ToString() ?? string.Empty;
    }
}
