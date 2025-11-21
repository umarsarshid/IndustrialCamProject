cmake_minimum_required(VERSION 3.10)
project(SimCamSDK)
set(CMAKE_CXX_STANDARD 11)

# Find OpenCV (Ensure 'brew install opencv' was run)
find_package(OpenCV REQUIRED)

# Create Shared Library (.dylib on Mac)
add_library(SimCamSDK SHARED SimCamSDK.cpp)
target_link_libraries(SimCamSDK ${OpenCV_LIBS})