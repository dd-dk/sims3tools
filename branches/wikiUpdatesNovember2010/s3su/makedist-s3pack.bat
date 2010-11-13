@echo off
set TargetName=s3su
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source

set out=S:\Sims3\Tools\S3PackUnpacker\


set mydate=%date: =0%
set dd=%mydate:~0,2%
set mm=%mydate:~3,2%
set yy=%mydate:~8,2%
set mytime=%time: =0%
set h=%mytime:~0,2%
set m=%mytime:~3,2%
set s=%mytime:~6,2%
set suffix=%yy%%mm%-%dd%-%h%%m%

if EXIST "%PROGRAMFILES%\nsis\makensis.exe" goto gotNotX86
if EXIST "%PROGRAMFILES(x86)%\nsis\makensis.exe" goto gotX86
echo "Could not find makensis."
goto noNSIS

:gotNotX86:
set MAKENSIS=%PROGRAMFILES%\nsis\makensis.exe
goto gotNSIS
:gotX86:
set MAKENSIS=%PROGRAMFILES(x86)%\nsis\makensis.exe
:gotNSIS:
set nsisv=/V3

if x%ConfigurationName%==xRelease goto REL
set pdb=
goto noREL
:REL:
set pdb=-xr!*.pdb
:noREL:


rem there shouldn't be any to delete...
del /q /f %out%%TargetName%*%suffix%.*

pushd ..
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*.suo -xr!zzOld -xr!bin -xr!obj -xr!Makefile -xr!*.Config "%out%%src%_%suffix%.7z" "s3pi S3Pack"
popd

pushd bin\%ConfigurationName%
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
copy S3Pack.exe unpack.exe
copy S3Pack.exe pack.exe
7za a -r -t7z -mx9 -ms -xr!.?* -xr!S3Pack.exe -xr!*vshost* -xr!*.Config %pdb% "%out%%base%_%suffix%.7z" *
del /f %TargetName%-Version.txt
del /f unpack.exe
del /f pack.exe
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

"%MAKENSIS%" "/DINSTFILES=INSTFILES.txt" "/DUNINSTFILES=UNINST.LOG" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%.exe"

:done:
rmdir /s/q %base%-%suffix%
del INSTFILES.txt
:noNSIS:
pause
