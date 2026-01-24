using QMTGroup.DSL.Library;

namespace PluginBidon;

[DSLNamespace("Bidon")]
public class FeatureBidonPlugin : IDSLLibrary
{
    [DSLFunction]
    public string ToLower(string input)
    {
        return input.ToLower();
    }
}
