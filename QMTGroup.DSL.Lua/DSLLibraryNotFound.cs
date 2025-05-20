namespace QMTGroup.DSL.Lua;

public class DSLLibraryNotFound : IOException
{
    public DSLLibraryNotFound() : base("A required library was not found during the compilation.")
    { }

    public DSLLibraryNotFound(string libraryName) : base("The '" + libraryName + "' library was not found during the compilation.")
    { }
}
