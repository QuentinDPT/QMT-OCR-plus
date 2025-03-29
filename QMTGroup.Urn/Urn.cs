using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace QMTGroup.Urn;

/// <summary>
/// Represents a Uniform Resource Name (URN). A URN is a unique identifier for a resource.<br/>
/// This structure provides methods for parsing, comparing, and working with URNs.
/// </summary>
/// <remarks>
/// A URN is typically in the format of 'urn:<NID>:<NSS>', where:<br/>
/// - <NID> is the Namespace Identifier (e.g., 'isbn', 'doi', etc.),<br/>
/// - <NSS> is the Namespace Specific String that uniquely identifies the resource within the namespace.<br/>
/// This structure ensures that URNs are properly validated according to the standard.
/// </remarks>
public readonly struct Urn : IEquatable<Urn>, IComparable<Urn>, IParsable<Urn>
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    private static readonly string _urnValidation = @"^urn(:[a-zA-Z0-9-]+)+$";

    private readonly Uri _urnSource;

    /// <inheritdoc/>
    public bool Equals(Urn other)
    {
        return other.ToString() == ToString();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Urn"/> from a string representing the URN.
    /// </summary>
    /// <param name="urn">The string representing the URN.</param>
    /// <exception cref="ArgumentException">Thrown if the string does not match a valid URN format.</exception>
    public Urn([StringSyntax(StringSyntaxAttribute.Uri)] string urn)
    {
        if (!Regex.Match(urn, _urnValidation).Success)
            throw new ArgumentException($"Invalid URN format: '{urn}'. A valid URN must follow the pattern 'urn:<NID>:<NSS>'.", nameof(urn));

        _urnSource = new Uri(urn);
    }

    /// <inheritdoc/>
    public override string ToString() => _urnSource.ToString();

    /// <inheritdoc/>
    public int CompareTo(Urn other)
    {
        return ToString().CompareTo(other.ToString());
    }

    /// <inheritdoc/>
    public static Urn Parse(string s, IFormatProvider? provider)
    {
        return new Urn(s);
    }

    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Urn result)
    {
        result = null;
        if (s is null)
            return false;
        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
