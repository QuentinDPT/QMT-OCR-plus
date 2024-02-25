using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QMTGroup.UI
{
    public class MainWindowContext : INotifyPropertyChanged
    {
        private int _currentMenuId = 0;

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

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
