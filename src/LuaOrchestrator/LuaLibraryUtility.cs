using Neo.IronLua;
using System.Linq.Expressions;
using System.Reflection;

namespace LuaOrchestrator;

public class LuaLibraryUtility
{
    private Type _type;

    public Type Type => _type;

    public LuaLibraryUtility(Type type)
    {
        _type = type;
    }

    /// <summary>
    /// Génère un LuaTable contenant toutes les méthodes statiques d'une classe donnée.
    /// </summary>
    /// <param name="type">Type de la classe statique.</param>
    /// <returns>Un LuaTable contenant les méthodes statiques.</returns>
    public LuaTable ToLuaLibrary()
    {
        Type type = _type;

        // Vérifie que le type est une classe statique
        if (!type.IsClass || !type.IsAbstract || !type.IsSealed)
            throw new ArgumentException($"Le type {type.Name} n'est pas une classe statique.");

        LuaTable table = new LuaTable();

        // Parcourt toutes les méthodes publiques et statiques de la classe
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            // Capture la méthode dans un délégué
            table[method.Name] = _generateLuaFunction(method);
        }

        return table;
    }

    /// <summary>
    /// Génère une fonction Lua (délégué) à partir d'une méthode C#.
    /// </summary>
    /// <param name="method">Méthode à transformer.</param>
    /// <returns>Délégué représentant la méthode compatible avec Lua.</returns>
    private static Delegate _generateLuaFunction(MethodInfo method)
    {
        // Récupère les paramètres de la méthode
        var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();

        // Crée un type de délégué compatible avec les arguments et le retour de la méthode
        var delegateType = Expression.GetDelegateType(parameters.Append(method.ReturnType).ToArray());

        // Crée un délégué pour la méthode
        return Delegate.CreateDelegate(delegateType, method);
    }
}
