cmake_minimum_required(VERSION 3.23)

project(Dalamud C CXX ASM_MASM)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_CXX_EXTENSIONS OFF)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/bin")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/bin")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/build/lib_intermediates")

set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /Zi")
set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF /OPT:ICF")
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>DLL")

set(CIMGUI_INCLUDE_DIRS
    ${CMAKE_CURRENT_SOURCE_DIR}/lib/cimgui
    ${CMAKE_CURRENT_SOURCE_DIR}/lib/cimgui/imgui
    CACHE INTERNAL "cimgui: Include Directories")

set(CIMGUI_DEFINITIONS
    IMGUI_DISABLE_OBSOLETE_FUNCTIONS=1
    IMGUI_USER_CONFIG="${CMAKE_CURRENT_SOURCE_DIR}/lib/cimgui/cimgui_user.h"
    CACHE INTERNAL "cimgui: Additional Definitions")

add_subdirectory(lib/CoreCLR)
add_subdirectory(lib/TsudaKageyu-minhook)
add_subdirectory(lib/cimgui)
add_subdirectory(lib/cimguizmo)
add_subdirectory(lib/cimplot)

add_subdirectory(Dalamud.Boot)
add_subdirectory(DalamudCrashHandler)
