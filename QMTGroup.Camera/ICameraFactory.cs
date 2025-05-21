using QMTGroup.Camera;
using QMTGroup.Core;

namespace QMTGroup.Camera;

/// <summary>
/// Represents a factory interface for creating and retrieving <see cref="ICamera"/> instances.
/// </summary>
/// <remarks>
/// This interface extends the <see cref="IFactory{TInterface}"/> interface, specifically for managing camera instances.<br/>
/// It provides methods to create and retrieve instances of <see cref="ICamera"/>, identified by a unique <see cref="Guid"/>.
/// </remarks>
public interface ICameraFactory : IFactory<ICamera>
{ }
