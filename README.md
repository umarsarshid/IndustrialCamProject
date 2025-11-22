# **Industrial Camera SDK Simulation (C++ / C\# Interop)**

A cross-platform simulation of a machine vision pipeline.  
This project demonstrates the architecture of a standard industrial inspection system, integrating an unmanaged C++ hardware driver with a managed .NET 9 GUI via P/Invoke.  
**Purpose:** To demonstrate proficiency in systems engineering, memory management, and diagnostic troubleshooting required for Application Engineering roles in the Machine Vision industry.

## **Project Goals**

1. **Hardware Abstraction:** Create a "Mock SDK" in C++ that treats a standard webcam as an industrial sensor (handling trigger signals, exposure simulation, and buffer management).  
2. **Managed/Unmanaged Interop:** Demonstrate safe data marshaling between the C++ Heap and C\# Pinned Memory using IntPtr and memcpy.  
3. **Systems Diagnosis:** Implement and detect a "Simulated Memory Leak" to demonstrate profiling skills with OS-level tools.  
4. **Cross-Platform Engineering:** Developed on **macOS (Apple Silicon)** using .dylib and **Avalonia UI**, demonstrating the ability to adapt Windows-centric architectures (DLL/WPF) to other environments.

## **Software Architecture**

The solution follows a strictly layered architecture common in hardware control software:

| Layer | Technology | Responsibility |
| :---- | :---- | :---- |
| **Frontend (App)** | C\# (.NET 8\) / Avalonia | Visualization, User Input, Event Handling. |
| **Interop (Bridge)** | P/Invoke (DllImport) | Defines the ABI (Application Binary Interface). |
| **Backend (Driver)** | C++ 11 / OpenCV | Hardware control, image resizing, color conversion (BGR$\\rightarrow$RGBA), memory copying. |
| **Hardware** | AVFoundation / Webcam | Physical image acquisition. |

## **Key Features**

### **1\. Industrial Camera Simulation**

* **Trigger Mode:** The C++ backend implements a state machine that blocks frame acquisition until a software trigger signal is received, mimicking an external electrical trigger.  
* **Exposure Control:** Simulates sensor integration time by performing matrix multiplication on pixel data ($\\alpha \\cdot P\_{i,j} \+ \\beta$) before passing it to the UI.

### **2\. Optimized Memory Pipeline**

* **Zero-Copy (almost) Rendering:** The C\# frontend allocates a pinned WriteableBitmap. The C++ backend writes directly into this locked memory address using memcpy, avoiding double-allocation overhead.  
* **Stride Alignment:** Handles the mismatch between OpenCV's default 3-channel BGR layout and modern UI 4-channel RGBA requirements via cv::cvtColor in the driver layer.

### **3\. Diagnostic Mode (The "Bug")**

* **Simulated Memory Leak:** Includes a specific test mode that allocates 20MB of unmanaged heap memory per frame without freeing it.  
* **Dirty Page Injection:** Uses memset to force the OS to commit physical RAM immediately, bypassing lazy allocation optimization, ensuring the leak is visible in profiling tools (Activity Monitor / Visual Studio Diagnostic Tools).

## **Build & Run Instructions (macOS)**

**Prerequisites:**

* .NET 8.0 SDK  
* CMake (brew install cmake)  
* OpenCV (brew install opencv)

### **Automated Build (Recommended)**

This project includes a root Makefile that orchestrates the entire build process (compiling C++, compiling C\#, linking the library, and running the app).

1. **Run the full pipeline:**  

```bash
make
```
2. **Clean artifacts:**  
```bash   
   make clean
```
### **Manual Build Steps**

If you prefer to build components individually:  
**Step 1: Build the C++ Driver**  
```bash
cd IndustrialCamProject/CppBackend  
mkdir build && cd build  
cmake ..  
make  
```
\# Output: libSimCamSDK.dylib

**Step 2: Link & Run the C\# Frontend**

1. **Build the C\# App:**  
```bash
   cd ../../CsFrontend/IndustrialCamGUI  
   dotnet build
```
2. Manually Link the Library:  
   Note: Because this is a custom simulation, we manually copy the compiled driver to the execution directory.  
```bash
   cp ../../CppBackend/build/libSimCamSDK.dylib ./bin/Debug/net8.0/
```
3. **Run:**  
```bash
   dotnet run
```
## **Debugging Scenario: How to Verify the Leak**

1. Run the application.  
2. Open **Activity Monitor** (or htop) and filter for dotnet.  
3. Click the red **"⚠ Simulate Leak"** button in the UI.  
4. **Observation:** Real Memory usage will spike by \~20MB per frame update.  
5. **Resolution:** Toggle the button off. The growth stops (demonstrating leak containment).

## **Code Highlights**

**The Interop Contract (C\#):**  
// We use IntPtr to handle the raw memory address of the bitmap buffer  
\[DllImport("libSimCamSDK.dylib")\]   
public static extern int GetFrame(IntPtr outBuffer, int width, int height);

**The Data Pump (C++):**  
// Strictly resize and convert color to match the UI's expected format  
cv::resize(frame, frame, cv::Size(width, height));  
cv::cvtColor(frame, frame, cv::COLOR\_BGR2RGBA);

// Safe copy to the pinned memory address  
memcpy(outBuffer, frame.data, width \* height \* 4);

### **Author**

Umar Arshid  
Application Engineering Portfolio