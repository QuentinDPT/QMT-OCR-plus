namespace QMTGroup.Web.Plugin.Model;

public class PluginFileItem
{
    public string Location { get; set; }

    public bool IsVisible { get; set; } = true;

    public PluginType PluginType { get; set; }
}