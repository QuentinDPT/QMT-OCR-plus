namespace QMTGroup.Web.Plugin.Model;

/// <summary>
/// Data structure that stores the plugin meta data.
/// </summary>
public record PluginMetaData
{
    /// <summary>
    /// Name of the assembly.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Version of the assembly.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Location of the assembly.
    /// </summary>
    public string FileName => Name + ".dll";

    public override string ToString()
    {
        return Name + ":" + Version;
    }
}
