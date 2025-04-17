using Neo.IronLua;
using QMTGroup.DSL.Library;
using System.Linq.Expressions;
using System.Reflection;

namespace QMTGroup.DSL.Lua;

public static class IDSLLibraryExtentions
{
    public static string GetNamespace(this IDSLLibrary self) => _getNamespace(self.GetType());

    public static LuaTable ToLuaLibrary(this IDSLLibrary self)
    {
        Type type = self.GetType();

        var result = new LuaTable();

        // Vérifie si le type implémente bien l'interface
        if (!typeof(IDSLLibrary).IsAssignableFrom(type))
            return result;

        // Filtre toutes les méthodes statiques avec l'attribut [DSLFunction]
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                          .Where(m => m.GetCustomAttribute<DSLFunctionAttribute>() != null);

        foreach (var method in methods)
        {
            // On essaye de construire un delegate à partir de la signature
            try
            {
                var parameters = method.GetParameters().Select(p => p.ParameterType).ToList();
                var delegateType = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(parameters.ToArray())
                    : Expression.GetFuncType(parameters.Concat([method.ReturnType]).ToArray());

                var del = Delegate.CreateDelegate(delegateType, self, method);
                result[method.Name] = del;
            }
            catch
            {
                Console.WriteLine($"Impossible de créer un délégué pour la méthode : {method.Name}");
            }
        }

        return result;
    }

    public static void AddToEngine(this IDSLLibrary self, DSLLuaEngine engine)
    {
        Type type = self.GetType();

        if (!typeof(IDSLLibrary).IsAssignableFrom(type))
            return;

        if (type.GetCustomAttributes(typeof(DSLNoNamespaceAttribute), inherit: true).Any())
        {
            AddToEngineNoNamespace(self, engine);
        }
        else
        {
            string name = _getNamespace(type);
            AddToEngineWithNamespace(self, engine, _decapitalize(name));
        }
    }

    private static void AddToEngineWithNamespace(IDSLLibrary self, DSLLuaEngine engine, string namespce)
    {
        Type type = self.GetType();

        string name = _getNamespace(type);

        engine.Engine.NewTable(name);

        var result = engine.Engine.GetTable(name);

        // Vérifie si le type implémente bien l'interface
        if (!typeof(IDSLLibrary).IsAssignableFrom(type))
            return;

        // Filtre toutes les méthodes statiques avec l'attribut [DSLFunction]
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                          .Where(m => m.GetCustomAttribute<DSLFunctionAttribute>() != null);

        foreach (var method in methods)
        {
            // On essaye de construire un delegate à partir de la signature
            try
            {
                var parameters = method.GetParameters().Select(p => p.ParameterType).ToList();
                var delegateType = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(parameters.ToArray())
                    : Expression.GetFuncType(parameters.Concat([method.ReturnType]).ToArray());

                var del = Delegate.CreateDelegate(delegateType, self, method);
                result[_decapitalize(method.Name)] = del;
            }
            catch
            {
                Console.WriteLine($"Impossible de créer un délégué pour la méthode : {method.Name}");
            }
        }
    }

    private static void AddToEngineNoNamespace(this IDSLLibrary self, DSLLuaEngine engine)
    {
        Type type = self.GetType();

        // Filtre toutes les méthodes statiques avec l'attribut [DSLFunction]
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                          .Where(m => m.GetCustomAttribute<DSLFunctionAttribute>() != null);

        foreach (var method in methods)
        {
            // On essaye de construire un delegate à partir de la signature
            try
            {
                var parameters = method.GetParameters().Select(p => p.ParameterType).ToList();
                var delegateType = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(parameters.ToArray())
                    : Expression.GetFuncType(parameters.Concat([method.ReturnType]).ToArray());

                var del = Delegate.CreateDelegate(delegateType, self, method);

                engine.Engine[_decapitalize(method.Name)] = del;
            }
            catch
            {
                Console.WriteLine($"Impossible de créer un délégué pour la méthode : {method.Name}");
            }
        }
    }

    private static string _decapitalize(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            return name;

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
    
    private static string _getNamespace(Type type)
    {
        // Vérifie si l'attribut est appliqué à la classe
        var attribute = (DSLNamespaceAttribute)Attribute.GetCustomAttribute(type, typeof(DSLNamespaceAttribute));

        if (attribute != null)
        {
            return _decapitalize(attribute.LibraryName);
        }
        else
        {
            return _decapitalize(type.Name);
        }
    }
}
