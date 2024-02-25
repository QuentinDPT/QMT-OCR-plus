namespace QMTGroup.IO.Camera
{
    /// <summary>
    /// A file camera is a file used as a video stream instead of a real camera
    /// </summary>
    public class FileCamera : Models.Camera.Camera
    {
        public FileCamera() : base()
        { }

        public FileCamera(string filePath) : base()
        { }
    }
}
