namespace QMTGroup.Core;

/// <summary>
/// Defines a generic interface for cloning objects.
/// </summary>
/// <typeparam name="T">
/// The type of the object that implements this interface. Must be a reference type.
/// </typeparam>
public interface ICloneable<T> where T : class
{
    /// <summary>
    /// Creates a new instance of the current object with the same values.
    /// </summary>
    /// <returns>
    /// A new instance of <typeparamref name="T"/> that is a copy of the current object.
    /// </returns>
    public T Clone();
}
