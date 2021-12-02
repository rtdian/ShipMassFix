:: This script creates a symlink to the game binaries to account for different installation directories on different systems.

@echo off
set dir="3rd\SpaceEngineers"
set /p path="Please enter the folder location of your SpaceEngineers.exe: "
cd %~dp0
if not exist "%dir%" mkdir "%dir%"
mklink /J "%dir%\Bin64" "%path%"
if errorlevel 1 goto Error
echo Done! You can now open the solution without issue.
goto End
:Error
echo An error occured creating the symlink.
:End
pause
