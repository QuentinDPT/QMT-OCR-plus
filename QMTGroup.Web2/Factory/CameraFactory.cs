using QMTGroup.Camera;
using System.Collections;

namespace QMTGroup.Web.Factory;

/// <summary>
/// A concrete implementation of the <see cref="ICameraFactory"/> interface for creating and retrieving <see cref="ICamera"/> instances.
/// </summary>
public class CameraFactory : ICameraFactory
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<Guid, ICamera> _implementationList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies for creating camera instances.</param>
    public CameraFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates an instance of a specific type of <see cref="ICamera"/> and associates it with a unique identifier.
    /// </summary>
    /// <typeparam name="TCamera">The type of the camera to create. It must implement <see cref="ICamera"/>.</typeparam>
    /// <param name="parameters">Optional parameters to pass to the camera instance's constructor. If no parameters are required, this can be <c>null</c>.</param>
    /// <returns>A unique identifier (Guid) associated with the newly created camera instance.</returns>
    /// <remarks>
    /// The <typeparamref name="TCamera"/> type must be a class that implements <see cref="ICamera"/>.<br/>
    /// If parameters are provided, they will be passed to the camera's constructor. Otherwise, the camera will be created using the default constructor.
    /// </remarks>
    public Guid Create<TCamera>(object? parameters = null) where TCamera : class, ICamera
    {
        Guid cameraInstanceGuid = Guid.NewGuid();

        TCamera cameraInstance;
        if (parameters is null)
            cameraInstance = ActivatorUtilities.CreateInstance<TCamera>(_serviceProvider);
        else
            cameraInstance = ActivatorUtilities.CreateInstance<TCamera>(_serviceProvider, parameters);

        _implementationList.Add(cameraInstanceGuid, cameraInstance);

        return cameraInstanceGuid;
    }

    /// <summary>
    /// Retrieves a camera instance associated with the provided unique identifier.
    /// </summary>
    /// <typeparam name="TCamera">The type of the camera to retrieve. It must implement <see cref="ICamera"/>.</typeparam>
    /// <param name="id">The unique identifier of the camera instance to retrieve.</param>
    /// <returns>The camera instance of type <typeparamref name="TCamera"/> if found, otherwise <c>null</c>.</returns>
    /// <remarks>
    /// If no instance is found for the provided identifier, this method returns <c>null</c>.
    /// </remarks>
    public TCamera? Get<TCamera>(Guid id) where TCamera : class, ICamera
    {
        if (_implementationList.TryGetValue(id, out ICamera? camera))
            return (TCamera)camera;
        return default;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the camera instances stored in the factory.
    /// </summary>
    /// <returns>An enumerator for the dictionary of camera instances.</returns>
    public IEnumerator<KeyValuePair<Guid, ICamera>> GetEnumerator() => _implementationList.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the camera instances stored in the factory.<br/>
    /// This is an explicit interface implementation of <see cref="IEnumerable.GetEnumerator"/>.
    /// </summary>
    /// <returns>An enumerator for the dictionary of camera instances.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Retrieves the camera implementation associated with the specified <paramref name="cameraInstance"/> identifier.
    /// </summary>
    /// <param name="cameraInstance">The unique identifier (<c>Guid</c>) of the camera instance to retrieve.</param>
    /// <returns>
    /// The instance that implements <see cref="ICamera"/> corresponding to the provided <paramref name="cameraInstance"/> identifier.<br/>
    /// If no matching camera is found, an exception may be thrown (e.g., <see cref="KeyNotFoundException"/>).
    /// </returns>
    /// <exception cref="KeyNotFoundException">When the key was not found</exception>
    public ICamera? this[Guid cameraInstance] => _implementationList[cameraInstance];
}
