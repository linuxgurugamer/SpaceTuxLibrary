
@echo off

rem H is the destination game folder
rem GAMEDIR is the name of the mod folder (usually the mod name)
rem GAMEDATA is the name of the local GameData
rem VERSIONFILE is the name of the version file, usually the same as GAMEDATA,
rem    but not always

set H=%KSPDIR%
set H=R:\KSP_1.9.1_Tetrix_Debug
set H=R:\KSP_1.9.1_dev

set GAMEDIR=SpaceTuxLibrary
set GAMEDATA="GameData"
set VERSIONFILE=%GAMEDIR%.version
set VERSIONFILE2=%3.version

copy /Y "%1%2" "%GAMEDATA%\%GAMEDIR%\Plugins"
copy /Y %VERSIONFILE% %GAMEDATA%\%GAMEDIR%
copy /Y %VERSIONFILE2% %GAMEDATA%\%GAMEDIR%

xcopy /y /s /I %GAMEDATA%\%GAMEDIR% "%H%\GameData\%GAMEDIR%"

echo Deployed

pause