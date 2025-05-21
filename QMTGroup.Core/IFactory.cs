namespace QMTGroup.Core;

/// <summary>
/// Represents a factory interface that is responsible for creating and retrieving instances of a given type.
/// </summary>
/// <typeparam name="TInterface">The type of interface that the factory is responsible for creating and retrieving.</typeparam>
public interface IFactory<TInterface> : IEnumerable<KeyValuePair<Guid, TInterface>>
{
    /// <summary>
    /// Creates an instance of the specified type and associates it with a unique identifier.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance to create. It must implement <see cref="TInterface"/>.</typeparam>
    /// <param name="parameters">Optional parameters to be passed to the constructor of the <typeparamref name="TInstance"/>.</param>
    /// <returns>A unique identifier (Guid) associated with the created instance.</returns>
    /// <remarks>
    /// The <typeparamref name="TInstance"/> type must be a class that implements <see cref="TInterface"/>.
    /// The parameters can be used to pass constructor arguments or configuration settings for the instance.
    /// </remarks>)
    public Guid Create<TInstance>(object? parameters) where TInstance : class, TInterface;

    /// <summary>
    /// Retrieves an instance of the specified type using the unique identifier.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance to retrieve. It must implement <see cref="TInterface"/>.</typeparam>
    /// <param name="id">The unique identifier associated with the desired instance.</param>
    /// <returns>The instance of type <typeparamref name="TInstance"/> associated with the provided identifier, or null if no instance is found.</returns>
    /// <remarks>
    /// The <typeparamref name="TInstance"/> type must be a class that implements <see cref="TInterface"/>.
    /// If no instance is found for the provided identifier, this method returns null.
    /// </remarks>
    public TInstance? Get<TInstance>(Guid id) where TInstance : class, TInterface;

    /// <summary>
    /// Retrieves the instance of type <typeparamref name="TInterface"/> associated with the specified <paramref name="instanceId"/> identifier.
    /// </summary>
    /// <param name="instanceId">The unique identifier (<c>Guid</c>) of the instance to retrieve.</param>
    /// <returns>
    /// The instance of type <typeparamref name="TInterface"/> associated with the provided <paramref name="instanceId"/> identifier.<br/>
    /// If no matching instance is found, an exception may be thrown (e.g., <see cref="KeyNotFoundException"/>).
    /// </returns>
    public TInterface? this[Guid instanceId] { get; }
}
