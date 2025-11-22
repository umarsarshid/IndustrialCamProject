using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using System;

namespace IndustrialCamGUI
{
    public partial class MainWindow : Window
    {
        // --- 1. CLASS-LEVEL VARIABLES (State) ---
        private const int WIDTH = 640;
        private const int HEIGHT = 480;
        private WriteableBitmap _bmp;
        private DispatcherTimer _timer;
        private bool _isConnected = false;
        private bool _bugActive = false;

        // --- UI CONTROL REFERENCES (The Fix) ---
        // We declare these explicitly so we don't rely on "Magic" binding
        private Button _btnConnect;
        private Button _btnTrigger;
        private Button _btnBug;
        private CheckBox _chkTrigger;
        private Image _cameraFeed;
        // Note: We don't strictly need the slider reference for logic, 
        // but good practice to have if we wanted to reset it programmatically.

        public MainWindow()
        {
            InitializeComponent();

            // --- MANUAL CONTROL LOOKUP ---
            // This creates the hard link between XAML names and C# variables.
            // If these are null, your XAML "Name" tags don't match these strings.
            _btnConnect = this.FindControl<Button>("BtnConnect");
            _btnTrigger = this.FindControl<Button>("BtnTrigger");
            _btnBug = this.FindControl<Button>("BtnBug");
            _chkTrigger = this.FindControl<CheckBox>("ChkTrigger");
            _cameraFeed = this.FindControl<Image>("CameraFeed");

            // --- INITIALIZATION ---
            _bmp = new WriteableBitmap(new PixelSize(WIDTH, HEIGHT), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Opaque);
            
            // Use the manually found reference
            _cameraFeed.Source = _bmp;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(33); 
            _timer.Tick += GameLoop;
        }

        // --- EVENT HANDLERS ---

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            if (CamWrapper.InitCamera(0))
            {
                _isConnected = true;
                
                // Use the manual references (Fixes NullReferenceException)
                _btnConnect.Content = "Connected";
                _btnConnect.IsEnabled = false; 
                
                _timer.Start(); 
            }
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (!_isConnected) return;

            using (var buffer = _bmp.Lock())
            {
                CamWrapper.GetFrame(buffer.Address, WIDTH, HEIGHT);
            }
            // Use the manual reference
            _cameraFeed.InvalidateVisual();
        }
        // Add event handlers
        private void OnExposureChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if (_isConnected) CamWrapper.SetExposure((int)e.NewValue);
        }
        private void OnTriggerModeChanged(object sender, RoutedEventArgs e) {
            bool mode = ChkTrigger.IsChecked ?? false;
            BtnTrigger.IsEnabled = mode;
            if (_isConnected) CamWrapper.SetTriggerMode(mode);
        }
        private void OnSoftwareTrigger(object sender, RoutedEventArgs e) {
            if (_isConnected) CamWrapper.SoftwareTrigger();
        }

        // Matches Click="OnBugToggle"
        private void OnBugToggle(object sender, RoutedEventArgs e) {
            _bugActive = !_bugActive;
            CamWrapper.SetBugState(_bugActive);
            BtnBug.Content = _bugActive ? "LEAKING..." : "⚠ Simulate Leak";
        }
            }
}