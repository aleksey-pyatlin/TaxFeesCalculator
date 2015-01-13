@echo off

echo 'System settings...'
call sys.settings.bat

echo 'Load settings...'
call %1

rem build console application
call build.bat

rem Run feesCalc with profile
cd FeesCalculator.ConsoleApplication\bin\Debug\
call FeesCalculator.ConsoleApplication.exe  -p "%profilePath%"