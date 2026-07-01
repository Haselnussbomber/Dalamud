project(CoreCLR CXX)

file(GLOB SOURCE_FILES *.cpp)

add_library(CoreCLR STATIC ${SOURCE_FILES})

target_compile_definitions(CoreCLR PUBLIC UNICODE _UNICODE)
target_include_directories(CoreCLR PUBLIC .)
