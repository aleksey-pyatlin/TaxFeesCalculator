@echo off

if exist "%CommonProgramFiles(x86)%" set commonFilesDir=%CommonProgramFiles(x86)%
if not exist "%CommonProgramFiles(x86)%" set commonFilesDir=%CommonProgramFiles%

if exist "%ProgramFiles(x86)%" set PF=%ProgramFiles(x86)%
if not exist "%ProgramFiles(x86)%" set PF=%ProgramFiles%

echo 'Load build/system settings...'
if exist sys.settings.bat call sys.settings.bat

%MSBUILD% "FeesCalc.sln" /t:Rebuild