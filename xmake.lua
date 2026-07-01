set_languages("c++23")
set_symbols("debug", "embed")
add_defines("UNICODE", "_UNICODE")

if is_mode("debug") then
    set_runtimes("MDd") -- MultiThreadedDebugDLL
else
    set_runtimes("MD") -- MultiThreadedDLL
    add_ldflags("/OPT:REF", "/OPT:ICF") -- OptimizeReferences & EnableCOMDATFolding
end

set_objectdir("$(builddir)/.objs")

target("libminhook")
    set_kind("static")
    add_files("lib/TsudaKageyu-minhook/src/**.c")

target("CoreCLR")
    set_kind("static")
    add_files("lib/CoreCLR/**.cpp")

target("Dalamud.Shared")
    set_kind("static")
    add_files("shared/**.cpp")

target("Dalamud.Boot")
    set_kind("shared")
    set_targetdir("bin/$(mode)/$(arch)")

    add_ldflags("/MANIFESTUAC:NO") -- EnableUAC=false

    add_files(
        "Dalamud.Boot/**.cpp",
        "Dalamud.Boot/**.asm",
        "Dalamud.Boot/Dalamud.Boot.rc",
        "Dalamud.Boot/themes.manifest")

    add_rules("utils.symbols.export_list", {symbols = {
        "Initialize",
        "RewriteRemoteEntryPointW",
        "RewrittenEntryPoint"
    }})

    set_pcheader("Dalamud.Boot/pch.h")

    add_includedirs("lib/TsudaKageyu-minhook/include", "lib/CoreCLR")
    add_deps("Dalamud.Shared", "libminhook", "CoreCLR")
    add_syslinks("shell32", "version", "shlwapi", "advapi32")

    after_build(function (target)
        os.cp("lib/CoreCLR/nethost/nethost.dll", target:targetdir())
    end)

target("DalamudCrashHandler")
    set_kind("binary")
    set_targetdir("bin/$(mode)/$(arch)")

    add_files(
        "DalamudCrashHandler/**.c",
        "DalamudCrashHandler/**.cpp",
        "DalamudCrashHandler/DalamudCrashHandler.rc")

    add_deps("Dalamud.Shared")
    add_syslinks("shell32", "ole32", "version", "winhttp", "dbghelp")

after_build(function (target)
    local targetdir = target:targetdir()
    local extensions = {"*.ilk", "*.exp", "*.lib"}

    for _, ext in ipairs(extensions) do
        os.rm(path.join(targetdir, ext))
    end
end)
