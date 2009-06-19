@echo off
set TargetName=S3PIDemoFE
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source

set out=S:\Sims3\s3pi\

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
7za a -r -t7z -mx9 -ms -xr!.?* -xr!thanks.txt -xr!*vshost* %pdb% "%out%%base%_%suffix%.7z" *
del /f %TargetName%-Version.txt
popd

7za x -o"%base%-%suffix%" "%out%%base%_%suffix%.7z"
pushd "%base%-%suffix%"
(
echo !cd %base%-%suffix%
for %%f in (*) do echo File /a %%f
) > ..\INSTFILES.txt

(
for %%f in (*) do echo Delete $INSTDIR\%%f
) > UNINST.LOG
attrib +r +h UNINST.LOG
popd

"%PROGRAMFILES%\nsis\makensis" "/DINSTFILES=INSTFILES.txt" "/DUNINSTFILES=UNINST.LOG" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%.exe"

:done:
rmdir /s/q %base%-%suffix%
del INSTFILES.txt
pause
