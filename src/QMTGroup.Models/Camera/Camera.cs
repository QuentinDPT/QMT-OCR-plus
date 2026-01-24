using Emgu.CV;
using QMTGroup.Models.ImageFilters;

namespace QMTGroup.Models.Camera
{
    /// <summary>
    /// A camera produces a video content
    /// </summary>
    public abstract class Camera : VideoCapture
    {
        protected Mat _image;

        protected IImageFilter? _imageFilter = null;

        /// <summary>
        /// The image from the video stream
        /// </summary>
        public Mat Image { get { return _image; } }

        /// <summary>
        /// The image from the video stream with a filter applied
        /// </summary>
        public Mat ImageFiltered {
            get {
                if (_imageFilter == null)
                    return _image;
                return _imageFilter.ApplyFilter(_image);
            }
        }

        /// <summary>
        /// Get or set the filter available with the ImageFiltered
        /// </summary>
        public IImageFilter? ImageFilter {
            get => _imageFilter;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _imageFilter = value;
            }
        }


        /// <summary>
        /// The basic way to get a camera
        /// </summary>
        public Camera() : base()
        { _configure(); }

        /// <summary>
        /// You want a specific camera in all you camera slots
        /// </summary>
        /// <param name="cameraSlot"></param>
        public Camera(int cameraSlot) : base(cameraSlot)
        { _configure(); }


        protected void _configure()
        {
            _image = new Mat();
            ImageGrabbed += _onImageRecieved;
        }

        private void _onImageRecieved(object sender, EventArgs e)
        {
            this.Retrieve(_image);
        }
    }
}
