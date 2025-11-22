#include <opencv2/opencv.hpp>
#include <iostream>

//So that InitCamera is visible outside the shared library
#define EXPORT __attribute__((visibility("default")))

cv::VideoCapture capture;

extern "C" {
    // Initialize Camera with Mac-specific driver
    EXPORT bool InitCamera(int camIndex) {
        // Force AVFoundation for macOS permissions
        capture.open(camIndex, cv::CAP_AVFOUNDATION);
        
        if (!capture.isOpened()) return false;
        
        // Set standard resolution
        capture.set(cv::CAP_PROP_FRAME_WIDTH, 640);
        capture.set(cv::CAP_PROP_FRAME_HEIGHT, 480);
        return true;
    }
    // ... inside extern "C" ...

    // The Main Data Pump
    // Param 'outBuffer': A pointer to memory allocated by C# (The Application Layer).
    //                    We DO NOT allocate memory here; we just fill what C# gave us.
    // Param 'width/height': The dimensions the C# UI is expecting.
    EXPORT int GetFrame(unsigned char* outBuffer, int width, int height) {
        
        // 1. Hardware Check
        // Ensure the camera is actually on. If we try to grab from a closed camera, we crash.
        if (!capture.isOpened()) return 0;
        
        // 2. Capture
        // Create a local matrix and ask the driver for the latest image.
        cv::Mat frame;
        capture >> frame;
        
        // 3. Validation
        // Sometimes hardware sends empty packets. If so, abort immediately.
        if (frame.empty()) return 0;

        // 4. Buffer Safety (Crucial Step)
        // The camera might be 1080p, but C# might only expect 640x480.
        // If we try to write 1080p data into a 640x480 buffer, we cause a Buffer Overflow.
        // We strictly resize the image to match what C# requested.
        cv::resize(frame, frame, cv::Size(width, height));
        
        // 5. Format Translation (The "Stride" Fix)
        // CRITICAL: OpenCV uses 3 channels (BGR). Avalonia/WPF uses 4 channels (BGRA).
        // If we skip this, the image will look "slanted" or scrambled because 
        // the byte alignment (stride) will be off by 1 byte per pixel.
        //came out bluue so // We convert BGR -> RGBA (Swaps Blue and Red, adds Alpha)
        cv::cvtColor(frame, frame, cv::COLOR_BGR2RGBA);

        // 6. Calculate Size
        // Width * Height * 4 bytes (Blue, Green, Red, Alpha)
        size_t expectedSize = width * height * 4;

        // 7. Final Safety Check & Copy
        // Verify that our OpenCV matrix actually contains the amount of data we calculated.
        if (frame.total() * frame.elemSize() == expectedSize) {
            
            // 8. The Marshaling (Interop)
            // Copy the raw bytes from the Unmanaged Heap (C++) to the Pinned Memory (C#).
            // memcpy is the fastest way to move memory.
            memcpy(outBuffer, frame.data, expectedSize);
            
            return 1; // Success code
        }
        
        return 0; // Failure code (Size mismatch)
    }
}