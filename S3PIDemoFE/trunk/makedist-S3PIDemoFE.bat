@echo off
set TargetName=S3PIDemoFE
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source

set out=S:\Sims3\s3pi\
set s3piDir=%CommonProgramFiles%\s3pi

set mydate=%date: =0%
set dd=%mydate:~0,2%
set mm=%mydate:~3,2%
set yy=%mydate:~8,2%
set mytime=%time: =0%
set h=%mytime:~0,2%
set m=%mytime:~3,2%
set s=%mytime:~6,2%
set suffix=%yy%%mm%-%dd%-%h%%m%

set nsisv=/V3

if x%ConfigurationName%==xRelease goto REL
set pdb=
goto noREL
:REL:
set pdb=-xr!*.pdb
:noREL:


rem there shouldn't be any to delete...
del %out%%TargetName%*%suffix%.*

pushd ..
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*.suo -xr!zzOld -xr!bin -xr!obj -xr!Makefile -xr!*.Config "%out%%src%_%suffix%.7z" S3PIDemoFE
popd

pushd bin\%ConfigurationName%
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
7za a -r -t7z -mx9 -ms -xr!.?* -xr!thanks.txt -xr!*vshost* -xr!*.Config %pdb% "%out%%base%_%suffix%.7z" *
del /f %TargetName%-Version.txt
popd


set s3piV=%s3piDir%\s3pi-Version.txt
if exist "%s3piV%" goto found
echo s3pi-Version.txt was not found: %s3piV%
goto done

:found:
for /f "usebackq" %%v in ("%s3piV%") do set S3PIVERSION=%%v
if not x%S3PIVERSION%==x goto gotVersion
echo s3pi-Version.txt did not contain a version string
goto done

:gotVersion:
7za x -o"%base%-%suffix%" "%out%%base%_%suffix%.7z"
"%PROGRAMFILES%\nsis\makensis" "/DTARGET=%base%-%suffix%" "/DS3PIVERSION=%S3PIVERSION%" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%.exe"

:done:
rmdir /s/q %base%-%suffix%
pause
