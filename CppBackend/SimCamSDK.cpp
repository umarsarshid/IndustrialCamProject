#include <iostream>
// Just a dummy function to test compiling
extern "C" void TestHello() { 
    std::cout << "Backend builds successfully!" << std::endl; 
}