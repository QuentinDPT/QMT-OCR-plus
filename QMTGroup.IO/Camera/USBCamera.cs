using Emgu.CV;
using QMTGroup.Models.Camera;

namespace QMTGroup.IO.Camera
{
    public class USBCamera : Models.Camera.Camera
    {
        public USBCamera() : base()
        { }

        public USBCamera(int cameraSlot) : base(cameraSlot)
        { }
    }
}
