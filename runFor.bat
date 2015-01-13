@echo off

echo 'Load settings...'
call %1

rem build console application
call build.bat

cls 
rem Run feesCalc with profile
cd FeesCalculator.ConsoleApplication\bin\Debug\
call FeesCalculator.ConsoleApplication.exe  -p "%profilePath%"