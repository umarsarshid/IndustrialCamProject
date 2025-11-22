# Master Makefile for Industrial Camera SDK Simulation
# Usage: 
#   make        -> Builds everything and runs the app
#   make clean  -> Removes build artifacts

# --- Directories ---
ROOT_DIR := .
CPP_DIR := $(ROOT_DIR)/CppBackend
CS_DIR := $(ROOT_DIR)/CsFrontend/IndustrialCamGUI
CPP_BUILD_DIR := $(CPP_DIR)/build
CS_BIN_DIR := $(CS_DIR)/bin/Debug/net9.0

# --- Artifacts ---
LIB_NAME := libSimCamSDK.dylib

.PHONY: all run build_cpp build_cs link clean

# Default target: Build everything and run
all: run

# 1. Build the C++ Backend (The Driver)
build_cpp:
	@echo "Building C++ Driver..."
	@mkdir -p $(CPP_BUILD_DIR)
	@cd $(CPP_BUILD_DIR) && cmake .. && make

# 2. Build the C# Frontend (The GUI)
build_cs:
	@echo "Building C# Frontend..."
	@cd $(CS_DIR) && dotnet build

# 3. Link (Copy the .dylib to the .NET output folder)
link: build_cpp build_cs
	@echo "Linking Driver to Application..."
	@mkdir -p $(CS_BIN_DIR)
	@cp $(CPP_BUILD_DIR)/$(LIB_NAME) $(CS_BIN_DIR)/
	@echo "Link Complete: $(LIB_NAME) -> $(CS_BIN_DIR)"

# 4. Run the Application
run: link
	@echo "Launching Industrial Camera GUI..."
	@cd $(CS_DIR) && dotnet run

# Clean up build files
clean:
	@echo "Cleaning..."
	@rm -rf $(CPP_BUILD_DIR)
	@cd $(CS_DIR) && dotnet clean
	@echo "Clean Complete"