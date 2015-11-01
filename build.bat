@ECHO OFF

REM ===================
REM ====== Setup ======
REM ===================

REM Path to .net compiler executable
SET COMPILER="C:\Windows\Microsoft.NET\Framework\v3.5\csc"

REM Used CM3D2 Architecture (x86 | x64)
SET ARCH="x64"

REM Path to CM3D2
SET GAME_DIR="D:\KISS\CM3D2"

REM Path to ReiPatcher
SET RP_DIR="D:\KISS\_workdir\ReiPatcher"

REM ==============================
REM ==== SkillCommandShortcut ====
REM ==============================

REM Compile plugin
SET P_LIB=/lib:%GAME_DIR%\CM3D2%ARCH%_Data\Managed /r:Assembly-CSharp.dll /r:Assembly-CSharp-firstpass.dll /r:UnityEngine.dll /r:UnityInjector.dll /r:ExIni.dll
SET P_OUT=/out:%GAME_DIR%\UnityInjector\CM3D2.SkillCommandShortcut.Plugin.dll
SET P_SRC=Lib\GearMenu.cs .\CM3D2.SkillCommandShortcut.plugin.cs
%COMPILER% /t:library %P_LIB% %P_OUT% %P_SRC%

pause;
