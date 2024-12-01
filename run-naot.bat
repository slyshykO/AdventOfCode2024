@echo off

setlocal
set saved_dir=%cd%
set old_path=%PATH%


PATH=C:\Users\alex3\bin\zig-windows-x86_64-0.13.0;C:\msys64\clang64\bin;%~dp0;%PATH%

echo run test for `%1%`...

set PRJ=%1%

dotnet publish %~dp0%PRJ%\%PRJ%.fsproj --configuration Release -f net9.0 --tl:on --verbosity d --self-contained True --runtime win-x64  /property:PublishTrimmed=True /property:IncludeNativeLibrariesForSelfExtract=True /property:DebugType=None /property:DebugSymbols=False /property:PublishSingleFile=False /property:PublishAot=True /p:StripSymbols=True  || goto error
cd %~dp0%PRJ%
%~dp0%PRJ%\bin\Release\net9.0\win-x64\publish\%PRJ%.exe

cd %saved_dir%
PATH=%OLD_PATH%
exit /b 0

:error
cd %saved_dir%
PATH=%OLD_PATH%
exit /b %errorlevel%