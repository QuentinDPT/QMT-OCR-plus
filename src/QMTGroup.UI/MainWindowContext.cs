using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QMTGroup.UI
{
    /// <summary>
    /// DataContext for the main window
    /// </summary>
    public class MainWindowContext : INotifyPropertyChanged
    {
        private int _currentMenuId = 0;

        /// <summary>
        /// This is the current menu id in the aside part of the software
        /// </summary>
        public int CurrentMenuId
        {
            get => _currentMenuId;
            set
            {
                _currentMenuId = value;
                OnPropertyChanged(null);
            }
        }

        public bool IsInSupervision { get => _currentMenuId == 1; }
        public bool IsInRecipe { get => _currentMenuId == 2; }
        public bool IsInConfiguration { get => _currentMenuId == 3; }
        public bool IsInDiagnostic { get => _currentMenuId == 4; }


        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Call this method to update WPF binds
        /// </summary>
        /// <param name="name">The name of the bind to update. (all binds if null)</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
