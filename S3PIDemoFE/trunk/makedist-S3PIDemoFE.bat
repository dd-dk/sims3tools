@echo off
set TargetName=s3pe
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source
set viewDDS=ViewDDS\bin\ViewDDS

set out=S:\Sims3\Tools\s3peDemoFE\
set helpFolder=%out%\HelpFiles

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
del /q /f %out%%TargetName%*%suffix%.*

pushd ..
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*.suo -xr!zzOld -xr!bin -xr!obj -xr!Makefile -xr!*.Config "%out%%src%_%suffix%.7z" S3PIDemoFE
popd

pushd bin\%ConfigurationName%
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
del /f /q HelpFiles
xcopy "%helpFolder%\*" HelpFiles /s /i /y
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*vshost* -xr!*.Config %pdb% "%out%%base%_%suffix%.7z" *
del /f %TargetName%-Version.txt
del /f /q HelpFiles
popd

7za x -o"%base%-%suffix%" "%out%%base%_%suffix%.7z"
xcopy "..\%viewDDS%\*" "%base%-%suffix%" /s /i /y
pushd "%base%-%suffix%"
(
echo !cd %base%-%suffix%
for %%f in (*) do echo File /a %%f
pushd HelpFiles
echo SetOutPath $INSTDIR\HelpFiles
for %%f in (*) do echo File /a HelpFiles\%%f
echo SetOutPath $INSTDIR
popd
) > ..\INSTFILES.txt

(
for %%f in (*) do echo Delete $INSTDIR\%%f
pushd HelpFiles
for %%f in (*) do echo Delete $INSTDIR\HelpFiles\%%f
echo RmDir HelpFiles
popd
) > UNINST.LOG
attrib +r +h UNINST.LOG
popd

"%PROGRAMFILES%\nsis\makensis" "/DINSTFILES=INSTFILES.txt" "/DUNINSTFILES=UNINST.LOG" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%.exe"

:done:
rmdir /s/q %base%-%suffix%
del INSTFILES.txt
pause
