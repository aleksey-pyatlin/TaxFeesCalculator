@echo off

if exist "%CommonProgramFiles(x86)%" set commonFilesDir=%CommonProgramFiles(x86)%
if not exist "%CommonProgramFiles(x86)%" set commonFilesDir=%CommonProgramFiles%

if exist "%ProgramFiles(x86)%" set PF=%ProgramFiles(x86)%
if not exist "%ProgramFiles(x86)%" set PF=%ProgramFiles%

if exist build.properties.bat call build.properties.bat

%MSBUILD% "FeesCalc.sln" /t:Rebuild