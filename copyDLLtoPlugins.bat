
@echo off

rem H is the destination game folder
rem GAMEDIR is the name of the mod folder (usually the mod name)
rem GAMEDATA is the name of the local GameData
rem VERSIONFILE is the name of the version file, usually the same as GAMEDATA,
rem    but not always

set H=R:\KSP_1.9.1_Tetrix_Debug
set GAMEDIR=SpaceTuxLibrary
set GAMEDATA="GameData"
set VERSIONFILE=%3.version

copy /Y "%1%2" "%GAMEDATA%\%GAMEDIR%\Plugins"
copy /Y %VERSIONFILE% %GAMEDATA%\%GAMEDIR%
echo %1
echo %2
