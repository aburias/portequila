using System.ComponentModel;

namespace PortKiller.Models
{
    public class PortProcess : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int Port { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public string LocalAddress { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public string DisplayInfo => $"{ProcessName} (PID: {ProcessId})";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
