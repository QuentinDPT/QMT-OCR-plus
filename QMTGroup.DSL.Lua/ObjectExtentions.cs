using Neo.IronLua;
using QMTGroup.DSL.Library;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace QMTGroup.DSL.Lua;

public static class ObjectExtentions
{
    public static LuaTable ToLuaTable(this object self)
    {
        Type type = self.GetType();

        var result = new LuaTable();

        // Filtre toutes les méthodes statiques avec l'attribut [DSLFunction]
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

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

        return result;
    }

    private static string _decapitalize(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            return name;

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
