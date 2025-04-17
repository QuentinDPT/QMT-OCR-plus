namespace QMTGroup.DSL.Library;

[AttributeUsage(AttributeTargets.Class)]
public class DSLNamespaceAttribute : Attribute
{
    public readonly string LibraryName;

    public DSLNamespaceAttribute(string libraryName)
    {
        LibraryName = libraryName;
    }
}
