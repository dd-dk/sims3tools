@echo off
set TargetName=s3pe
set ConfigurationName=Release
set base=%TargetName%
rem -%ConfigurationName%
set src=%TargetName%-Source
set viewDDS=s3pe Helpers\ViewDDS\HelperApp\bin\ViewDDS

set out=S:\Sims3\Tools\s3pe\
set helpFolder=%out%HelpFiles

set mydate=%date: =0%
set dd=%mydate:~0,2%
set mm=%mydate:~3,2%
set yy=%mydate:~8,2%
set mytime=%time: =0%
set h=%mytime:~0,2%
set m=%mytime:~3,2%
set s=%mytime:~6,2%
set suffix=%yy%-%mm%%dd%-%h%%m%

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
7za a -r -t7z -mx9 -ms -xr!.?* -xr!*.suo -xr!zzOld -xr!bin -xr!obj -xr!Makefile -xr!*.Config "%out%%src%_%suffix%.7z" s3pe
popd

xcopy "..\%viewDDS%\*" "bin\%ConfigurationName%" /s /i /y
pushd bin\%ConfigurationName%
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
del /f /q HelpFiles
xcopy "%helpFolder%\*" HelpFiles /s /i /y
7za a -r -t7z -mx9 -ms -xr!x64 -xr!.?* -xr!*vshost* -xr!*.Config %pdb% "%out%%base%_%suffix%.7z" *
del /f %TargetName%-Version.txt
del /f /q HelpFiles
popd
for %%I in (..\%viewDDS%\*) do del "bin\%ConfigurationName%\%%~nxI"

7za x -o"%base%-%suffix%" "%out%%base%_%suffix%.7z"
pushd "%base%-%suffix%"
(
echo !cd %base%-%suffix%
for %%f in (*) do echo File /a %%f
pushd HelpFiles
echo SetOutPath $INSTDIR\HelpFiles
for %%f in (*) do echo File /a HelpFiles\%%f
echo SetOutPath $INSTDIR
popd
pushd Helpers
echo SetOutPath $INSTDIR\Helpers
for %%f in (*) do echo File /a Helpers\%%f
echo SetOutPath $INSTDIR
popd
dir /-c "..\%base%-%suffix%" | find " bytes" | for /f "tokens=3" %%f in ('find /v " free"') do @echo StrCpy $0 %%f
) > ..\INSTFILES.txt

(
for %%f in (*) do echo Delete $INSTDIR\%%f
pushd HelpFiles
for %%f in (*) do echo Delete $INSTDIR\HelpFiles\%%f
echo RmDir HelpFiles
popd
pushd Helpers
for %%f in (*) do echo Delete $INSTDIR\Helpers\%%f
echo RmDir Helpers
popd
) > UNINST.LOG
attrib +r +h UNINST.LOG
popd

"%MAKENSIS%" "/DINSTFILES=INSTFILES.txt" "/DUNINSTFILES=UNINST.LOG" "/DVSN=%suffix%" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%.exe"

rmdir /s/q %base%-%suffix%
del INSTFILES.txt

rem --- x64 packaging ---
if not exist bin\%ConfigurationName%\x64 goto nox64
xcopy "..\%viewDDS%\*" "bin\%ConfigurationName%\x64" /s /i /y
pushd bin\%ConfigurationName%\x64
echo %suffix% >%TargetName%-Version.txt
attrib +r %TargetName%-Version.txt
del /f /q HelpFiles
xcopy "%helpFolder%\*" HelpFiles /s /i /y
7za a -r -t7z -mx9 -ms -xr!x64 -xr!.?* -xr!*vshost* -xr!*.Config %pdb% "%out%%base%_%suffix%-x64.7z" *
del /f %TargetName%-Version.txt
del /f /q HelpFiles
popd
for %%I in (..\%viewDDS%\*) do del "bin\%ConfigurationName%\x64\%%~nxI"

7za x -o"%base%-%suffix%-x64" "%out%%base%_%suffix%-x64.7z"
pushd "%base%-%suffix%-x64"
(
echo !cd %base%-%suffix%-x64
for %%f in (*) do echo File /a %%f
pushd HelpFiles
echo SetOutPath $INSTDIR\HelpFiles
for %%f in (*) do echo File /a HelpFiles\%%f
echo SetOutPath $INSTDIR
popd
pushd Helpers
echo SetOutPath $INSTDIR\Helpers
for %%f in (*) do echo File /a Helpers\%%f
echo SetOutPath $INSTDIR
popd
dir /-c "..\%base%-%suffix%-x64" | find " bytes" | for /f "tokens=3" %%f in ('find /v " free"') do @echo StrCpy $0 %%f
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

"%MAKENSIS%" "/DINSTFILES=INSTFILES.txt" "/DUNINSTFILES=UNINST.LOG" "/DVSN=%suffix%" "/DX64" %nsisv% mknsis.nsi "/XOutFile %out%%base%_%suffix%-x64.exe"

rmdir /s/q %base%-%suffix%-x64
del INSTFILES.txt


:nox64:

:noNSIS:
pause
