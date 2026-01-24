namespace QMTGroup.Web.Plugin.Model;

public class PluginDependencies
{
    public PluginMetaData MetaData { get; set; }

    public List<PluginDependencies> Dependencies { get; set; }
}
