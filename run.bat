@echo off

setlocal
set saved_dir=%cd%
set old_path=%PATH%


echo run advent of code 2024 `%1%`...

set PRJ=%1%

cd %~dp0%PRJ%
dotnet run --project %~dp0%PRJ%\%PRJ%.fsproj -f net9.0  || goto error


cd %saved_dir%
PATH=%OLD_PATH%
exit /b 0

:error
cd %saved_dir%
PATH=%OLD_PATH%
exit /b %errorlevel%