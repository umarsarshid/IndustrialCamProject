using System;
using System.Runtime.InteropServices; // Required for P/Invoke (DllImport)

namespace IndustrialCamGUI {
    
    // This class serves as the bridge (Interop) between our Managed C# UI 
    // and the Unmanaged C++ backend.
    public static class CamWrapper {
        
        // NOTE: On macOS, dynamic libraries are .dylib. 
        // If porting to Windows later, change this to "SimCamSDK.dll".
        // Make sure this file is actually in the bin output folder!
        const string LibName = "libSimCamSDK.dylib"; 

        // 1. Setup
        // Tries to open the camera via OpenCV. Returns false if hardware is missing.
        [DllImport(LibName)] 
        public static extern bool InitCamera(int camIndex);

        // 2. The Critical Loop
        // We pass an IntPtr (raw memory address) instead of a byte[] to avoid 
        // the Garbage Collector trying to move memory while C++ is writing to it.
        [DllImport(LibName)] 
        public static extern int GetFrame(IntPtr outBuffer, int width, int height);

        // --- Control Functions ---
        
        // Sets the simulated exposure value (0-100)
        [DllImport(LibName)] 
        public static extern void SetExposure(int value);

        // Toggles between "Free Run" (video) and "Trigger" (snapshot) modes
        [DllImport(LibName)] 
        public static extern void SetTriggerMode(bool enabled);

        // If trigger mode is on, this tells the C++ loop to release one frame
        [DllImport(LibName)] 
        public static extern void SoftwareTrigger();

        // --- Debugging ---
        // Turns on the intentional memory leak to test the profiler
        [DllImport(LibName)] 
        public static extern void SetBugState(bool enabled);
    }
}