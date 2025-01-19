using Emgu.CV;
using QMTGroup.ImageFilters.Filters;
using QMTGroup.Models.Camera;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace QMTGroup.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _useFilter = false;

        protected Camera? _capture;

        /// <summary>
        /// The dataContext for the UI
        /// </summary>
        public MainWindowContext _dataContext = new MainWindowContext();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _dataContext;
        }

        /// <summary>
        /// Build and start a camera into another thread
        /// </summary>
        /// <param name="cameraCreatorDelegate">The camera build statement</param>
        public void StartCamera(Func<Camera> cameraCreatorDelegate)
        {
            if(cameraCreatorDelegate == null)
                throw new ArgumentNullException(nameof(cameraCreatorDelegate));

            new Thread(new ParameterizedThreadStart(x => {
                _capture = cameraCreatorDelegate.Invoke();
                if (_capture == null)
                    throw new ArgumentNullException();
                _capture.ImageFilter = new ToGrayScales();
                _capture.Start();
                _capture.ImageGrabbed += _onImageRecieved;
            })).Start();
        }

        /// <summary>
        /// Change the fill mode for the camera view
        /// </summary>
        public void ChangeFillMode()
        {
            if(VideoStream.Stretch == System.Windows.Media.Stretch.UniformToFill)
                VideoStream.Stretch = System.Windows.Media.Stretch.Uniform;
            else
                VideoStream.Stretch = System.Windows.Media.Stretch.UniformToFill;
        }

        /// <summary>
        /// Apply filter or reset it
        /// </summary>
        public void ToggleFilters() =>
            _useFilter = !_useFilter;

        /// <summary>
        /// When the main window is initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            CvInvoke.Init();
        }

        /// <summary>
        /// Function called when an image is recieved from the camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _onImageRecieved(object sender, EventArgs e)
        {
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                Mat _frame;

                if(_useFilter)
                    _frame = _capture.ImageFiltered;
                else
                    _frame = _capture.Image;

                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    VideoStream.Source = _convertBitmap(_frame.ToBitmap());
                }));
            }
        }

        /// <summary>
        /// Convert a Bitmap to a BitmapImage for the concurrency between camera thread and UI thread
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static BitmapImage _convertBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }


        private void _changeMenu(object sender, MouseButtonEventArgs e) => _changeMenu((sender as System.Windows.Controls.Grid).Name);
        private void _changeMenu(object sender, TouchEventArgs e) => _changeMenu((sender as System.Windows.Controls.Grid).Name);

        /// <summary>
        /// Change the aside menu and the content showed
        /// </summary>
        /// <param name="senderName"></param>
        private void _changeMenu(string senderName)
        {
            switch(senderName)
            {
                case "MenuSectionSupervision":
                    _dataContext.CurrentMenuId = 1;
                    break;
                case "MenuSectionRecipe":
                    _dataContext.CurrentMenuId = 2;
                    break;
                case "MenuSectionConfiguration":
                    _dataContext.CurrentMenuId = 3;
                    break;
                case "MenuSectionDiagnostic":
                    _dataContext.CurrentMenuId = 4;
                    break;
                default:
                    _dataContext.CurrentMenuId = 0;
                    break;
            }
        }

        /// <summary>
        /// Change the filter when key down
        /// </summary>
        private void QMTMainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            ToggleFilters();
        }
    }
}