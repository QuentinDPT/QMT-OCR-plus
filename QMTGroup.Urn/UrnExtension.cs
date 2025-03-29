using System.Diagnostics.CodeAnalysis;

namespace QMTGroup.Urn;

public static class UrnExtension
{
    /// <summary>
    /// Converts the given <see cref="Urn"/> instance into a <see cref="Uri"/> object.
    /// </summary>
    /// <param name="urn">The <see cref="Urn"/> instance to be converted into a <see cref="Uri"/>.</param>
    /// <returns>A <see cref="Uri"/> that represents the same URN instance.</returns>
    /// <remarks>
    /// This method allows you to easily convert a <see cref="Urn"/> to a <see cref="Uri"/> when you need to use the URN in contexts that expect a <see cref="Uri"/> type, such as networking, resource identification, or URI manipulation.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="urn"/> is null.</exception>
    public static Uri AsUri(this Urn urn) => new Uri(urn.ToString());
}
