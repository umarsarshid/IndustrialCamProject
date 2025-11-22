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
        // --- 1. CLASS-LEVEL VARIABLES (State) ---
        
        // Constants for image size (Must match C++ logic)
        private const int WIDTH = 640;
        private const int HEIGHT = 480;

        // The bitmap buffer that connects UI to C++ memory
        private WriteableBitmap _bmp;

        // The loop timer (runs at ~30 FPS)
        private DispatcherTimer _timer;

        // State flags
        private bool _isConnected = false;
        private bool _bugActive = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        // --- STUB FUNCTIONS TO SATISFY THE XAML COMPILER ---

        // Matches Click="OnConnect"
        // 1. Connection Handler
        // This initializes the hardware link when the user clicks "Connect".
        private void OnConnect(object sender, RoutedEventArgs e) {
            
            // Attempt to initialize the backend driver via the DLL.
            // Index '0' is the default system webcam. (Use '1' if you have an external USB cam).
            if (CamWrapper.InitCamera(0)) { 
                
                _isConnected = true;
                
                // Start the "Game Loop" (Polling Timer).
                // We use a timer to poll the camera at ~30 FPS. 
                // This ensures the UI thread controls the render speed and doesn't get flooded by the hardware.
                _timer.Start(); 
            }
        }

        // 2. The Main Data Loop (Ticks every ~33ms)
        // This is the critical "Hot Path" where data moves from Unmanaged -> Managed memory.
        private void GameLoop(object? sender, EventArgs e) {
            
            // Safety check: Don't request data if the hardware isn't ready.
            if (!_isConnected) return;

            // CRITICAL: Lock the WriteableBitmap.
            // This creates a "Memory Fence" that does two things:
            // 1. It gives us a raw pointer (IntPtr) to the pixel buffer.
            // 2. It pins the memory, preventing the C# Garbage Collector from moving it while C++ is writing.
            using (var buffer = _bmp.Lock()) {
                
                // Pass the raw memory address to C++.
                // The C++ DLL will 'memcpy' the pixel data directly to this address.
                // This avoids creating a second copy of the image, keeping latency low.
                CamWrapper.GetFrame(buffer.Address, WIDTH, HEIGHT);
            }
            
            // Now that the buffer is updated and unlocked, force a UI redraw.
            CameraFeed.InvalidateVisual(); 
        }
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