using Avalonia.Controls;
using Avalonia.Controls.Primitives; // <--- CRITICAL for Slider events
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using System;

namespace IndustrialCamGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // --- STUB FUNCTIONS TO SATISFY THE XAML COMPILER ---

        // Matches Click="OnConnect"
        private void OnConnect(object sender, RoutedEventArgs e)
        {
            // Logic will be added 
        }

        // Matches ValueChanged="OnExposureChanged"
        // Note: Requires 'using Avalonia.Controls.Primitives;'
        private void OnExposureChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Logic will be added 
        }

        // Matches IsCheckedChanged="OnTriggerModeChanged"
        private void OnTriggerModeChanged(object sender, RoutedEventArgs e)
        {
            // Logic will be added 
        }

        // Matches Click="OnSoftwareTrigger"
        private void OnSoftwareTrigger(object sender, RoutedEventArgs e)
        {
            // Logic will be added 
        }

        // Matches Click="OnBugToggle"
        private void OnBugToggle(object sender, RoutedEventArgs e)
        {
            // Logic will be added 
        }
    }
}