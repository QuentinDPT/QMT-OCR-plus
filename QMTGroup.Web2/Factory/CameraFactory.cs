using QMTGroup.Camera;
using System.Collections;

namespace QMTGroup.Web.Factory
{
    public class CameraFactory : ICameraFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<Guid, ICamera> _implementationList = new();


        public CameraFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public Guid Create<TCamera>(object? parameters) where TCamera : class, ICamera
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

        public TCamera? Get<TCamera>(Guid id) where TCamera : class, ICamera
        {
            if (_implementationList.TryGetValue(id, out ICamera? camera))
                return (TCamera)camera;
            return default;
        }

        public IEnumerator<KeyValuePair<Guid, ICamera>> GetEnumerator() => _implementationList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
