using Emgu.CV;
using QMTGroup.Models.ImageFilters;

namespace QMTGroup.Models.Camera
{
    public abstract class Camera : VideoCapture
    {
        protected Mat _image;

        protected IImageFilter? _imageFilter = null;

        public Mat Image { get { return _image; } }

        public Mat ImageFiltered {
            get {
                if (_imageFilter == null)
                    return _image;
                return _imageFilter.ApplyFilter(_image);
            }
        }

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
