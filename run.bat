@echo off

echo 'Load settings...'
call settings.properties.bat

rem build console application
call build.bat

rem Run feesCalc with profile
cd FeesCalculator.ConsoleApplication\bin\Debug\
call FeesCalculator.ConsoleApplication.exe  -p "%profilePath%"

pause