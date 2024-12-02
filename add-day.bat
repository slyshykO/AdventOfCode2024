@echo off

setlocal
set saved_dir=%cd%
set old_path=%PATH%

echo adding `%1%`...

set PRJ=%1%

cd %~dp0


dotnet new console -lang F# -o %PRJ% || goto error
dotnet sln add %PRJ%\%PRJ%.fsproj  || goto error
type nul > %PRJ%\input.txt || goto error

cd %saved_dir%
PATH=%OLD_PATH%
exit /b 0

:error
cd %saved_dir%
PATH=%OLD_PATH%
exit /b %errorlevel%