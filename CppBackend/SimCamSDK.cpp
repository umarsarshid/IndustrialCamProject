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
}