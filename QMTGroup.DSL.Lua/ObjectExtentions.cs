using Neo.IronLua;
using System.Linq.Expressions;
using System.Reflection;

namespace QMTGroup.DSL.Lua;

public static class ObjectExtentions
{
    public static LuaTable ToLuaTable(this object self)
    {
        Type type = self.GetType();

        var result = new LuaTable();


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


        var constants = type.GetProperties().Where(x => x.CanRead && !x.CanWrite);

        foreach (var constant in constants)
        {
            // On essaye de construire un delegate à partir de la signature
            try
            {
                result["get" + constant.Name] = () =>
                {
                    var obj = constant.GetValue(self);

                    if (obj is Enum)
                        return obj.ToString();

                    return obj;
                };
            }
            catch
            {
                Console.WriteLine($"Impossible de créer un délégué pour la constante : {constant.Name}");
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
