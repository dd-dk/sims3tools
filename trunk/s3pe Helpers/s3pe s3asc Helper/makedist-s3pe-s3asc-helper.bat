@echo off
set TargetName=s3pe-s3asc-helper
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source
set out=S:\Sims3\Tools\s3pe\

set mydate=%date: =0%
set dd=%mydate:~0,2%
set mm=%mydate:~3,2%
set yy=%mydate:~8,2%
set mytime=%time: =0%
set h=%mytime:~0,2%
set m=%mytime:~3,2%
set s=%mytime:~6,2%
set suffix=%yy%-%mm%%dd%-%h%%m%

if x%ConfigurationName%==xRelease goto REL
set pdb=
goto noREL
:REL:
set pdb=-xr!*.pdb
:noREL:

rem there shouldn't be any to delete...
del /q /f %out%%TargetName%*%suffix%.*

pushd ..
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*.suo -xr!*.csproj.user -xr!zzOld -xr!bin -xr!obj -xr!Makefile -xr!*.Config "%out%%src%_%suffix%.7z" "s3pe s3asc Helper"
popd

pushd bin\%ConfigurationName%
mkdir Helpers
copy ..\..\*.helper Helpers
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
7za a -r -t7z -mx9 -ms "%out%%base%_%suffix%.7z" *.exe Helpers
del /f %TargetName%-Version.txt
rmdir /s/q Helpers
popd

pause
