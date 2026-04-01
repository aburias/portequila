using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PortKiller.Models;
using PortKiller.Services;

namespace PortKiller
{
    public partial class MainWindow : Window
    {
        private readonly PortService _portService;
        private readonly ObservableCollection<int> _portTags = new();
        private List<PortProcess> _currentProcesses = new();

        public MainWindow()
        {
            InitializeComponent();
            _portService = new PortService();
            PortTagsControl.ItemsSource = _portTags;
            UpdatePlaceholderVisibility();
        }

        private void PortInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var text = PortInputBox.Text.Trim();
                if (int.TryParse(text, out int port) && port > 0 && port <= 65535)
                {
                    if (!_portTags.Contains(port))
                    {
                        _portTags.Add(port);
                        UpdatePlaceholderVisibility();
                    }
                    PortInputBox.Clear();
                }
                else if (!string.IsNullOrEmpty(text))
                {
                    StatusText.Text = "Invalid port number (1-65535)";
                }
                e.Handled = true;
            }
        }

        private void RemovePortTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int port)
            {
                _portTags.Remove(port);
                UpdatePlaceholderVisibility();
            }
        }

        private void UpdatePlaceholderVisibility()
        {
            PlaceholderText.Visibility = _portTags.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ScanPorts_Click(object sender, RoutedEventArgs e)
        {
            ScanSpecifiedPorts();
        }

        private void ScanSpecifiedPorts()
        {
            if (!_portTags.Any())
            {
                StatusText.Text = "Add some ports first (type + Enter)";
                return;
            }

            _currentProcesses = _portService.GetProcessesOnPorts(_portTags);
            ProcessList.ItemsSource = _currentProcesses;
            UpdateButtonStates();

            StatusText.Text = _currentProcesses.Count > 0
                ? $"Found {_currentProcesses.Count} process(es)"
                : "No processes found on specified ports";
        }

        private void ShowAllListening_Click(object sender, RoutedEventArgs e)
        {
            _currentProcesses = _portService.GetAllListeningPorts();
            ProcessList.ItemsSource = _currentProcesses;
            UpdateButtonStates();

            StatusText.Text = $"Found {_currentProcesses.Count} listening port(s)";
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (_portTags.Any())
            {
                ScanSpecifiedPorts();
            }
            else
            {
                ShowAllListening_Click(sender, e);
            }
            SelectAllCheckBox.IsChecked = false;
        }

        private void ProcessCheckBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateButtonStates();
            UpdateSelectAllState();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            var isChecked = SelectAllCheckBox.IsChecked == true;
            foreach (var process in _currentProcesses)
            {
                process.IsSelected = isChecked;
            }
            UpdateButtonStates();
        }

        private void UpdateSelectAllState()
        {
            if (!_currentProcesses.Any())
            {
                SelectAllCheckBox.IsChecked = false;
            }
            else if (_currentProcesses.All(p => p.IsSelected))
            {
                SelectAllCheckBox.IsChecked = true;
            }
            else if (_currentProcesses.Any(p => p.IsSelected))
            {
                SelectAllCheckBox.IsChecked = null;
            }
            else
            {
                SelectAllCheckBox.IsChecked = false;
            }
        }

        private void UpdateButtonStates()
        {
            var hasProcesses = _currentProcesses.Any();
            var hasSelected = _currentProcesses.Any(p => p.IsSelected);

            KillSelectedButton.IsEnabled = hasSelected;
            KillAllButton.IsEnabled = hasProcesses;
        }

        private void KillSelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = _currentProcesses.Where(p => p.IsSelected).ToList();
            if (!selected.Any()) return;

            var result = MessageBox.Show(
                $"Kill {selected.Count} selected process(es)?\n\n" +
                string.Join("\n", selected.Select(p => $"• {p.ProcessName} (PID: {p.ProcessId}) on port {p.Port}")),
                "Confirm Kill",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            KillProcesses(selected);
        }

        private void KillAll_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentProcesses.Any()) return;

            var result = MessageBox.Show(
                $"Kill ALL {_currentProcesses.Count} process(es)?\n\n" +
                string.Join("\n", _currentProcesses.Select(p => $"• {p.ProcessName} (PID: {p.ProcessId}) on port {p.Port}")),
                "Confirm Kill All",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            KillProcesses(_currentProcesses);
        }

        private void KillProcesses(List<PortProcess> processes)
        {
            int killed = 0;
            int failed = 0;
            var uniquePids = processes.Select(p => p.ProcessId).Distinct().ToList();

            foreach (var pid in uniquePids)
            {
                if (_portService.KillProcess(pid))
                    killed++;
                else
                    failed++;
            }

            if (failed > 0)
            {
                StatusText.Text = $"Killed {killed}, failed {failed}. Try as Administrator.";
            }
            else
            {
                StatusText.Text = $"Killed {killed} process(es)";
            }

            Refresh_Click(this, new RoutedEventArgs());
        }
    }
}
