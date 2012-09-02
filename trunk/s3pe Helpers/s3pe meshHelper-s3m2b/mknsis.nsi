;!include "MUI.nsh"


!define tla "meshHelper-s3m2b"
!ifndef INSTFILES
  !error "Caller didn't define INSTFILES"
!endif
!ifndef UNINSTFILES
  !error "Caller didn't define UNINSTFILES"
!endif
!ifndef VSN
  !error "Caller didn't define VSN"
!endif

Var wasInUse
Var wantAll






InstallDir $PROGRAMFILES64\${tla}
!define PROGRAM_NAME "s3pe meshHelper for Blender"
!define INSTREGKEY "${tla}"

!define EXE ${tla}.exe






  
  
  
  
SetCompressor /SOLID LZMA
XPStyle on
Name "${PROGRAM_NAME}"
AddBrandingImage top 0
Icon ..\..\s3pe\Resources\s3pe.ico
UninstallIcon ..\..\s3pe\Resources\s3pe.ico




LicenseData "gpl-3.0.txt"
Page license


PageEx components
  ComponentText "Select the installation options.  Click Next to continue." " " " "
PageExEnd
Page directory
; Request application privileges for Windows Vista and above
RequestExecutionLevel admin
Page instfiles

Section "Install for all users"
  StrCpy $wantAll "Y"
SectionEnd














Section
  SetShellVarContext all
  StrCmp "Y" $wantAll gotAll
  SetShellVarContext current
gotAll:  

  SetOutPath $INSTDIR
  
  !include ${INSTFILES}
  IntOp $0 $0 / 1024

  WriteUninstaller uninst-${tla}.exe
  
  ; Write the uninstall keys for Windows
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "DisplayIcon" "$INSTDIR\${EXE}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "DisplayName" "${PROGRAM_NAME}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "DisplayVersion" "${VSN}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "HelpLink" "http://www.den.simlogical.com/denforum/index.php?topic=803.0"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "Publisher" "Peter L Jones"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "UninstallString" '"$INSTDIR\uninst-${tla}.exe"'
  ; $0 is set in ${INSTFILES} by the batch file...
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "EstimatedSize" $0
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "NoModify" 1
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "NoRepair" 1

SectionEnd

Function .onGUIInit
  SetOutPath $TEMP
  File ..\..\..\s3pe\Resources\s3pe.ico
  SetBrandingImage $TEMP\s3pe.ico
  Delete $TEMP\s3pe.ico
  Call GetInstDir
  Call CheckInUse
  Call CheckOldVersion
FunctionEnd

Function GetInstDir
  Push $0
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  StrCmp $0 "" gidNotCU
  IfFileExists "$0\${tla}\${EXE}" gidSetINSTDIR
gidNotCU:
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  StrCmp $0 "" gidNotLM
  IfFileExists "$0\${tla}\${EXE}" gidSetINSTDIR
gidNotLM:
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\s3pe" "InstallLocation"
  StrCmp $0 "" gidNotCUs3pe
  IfFileExists "$0\s3pe.exe" gidSetINSTDIRSub
gidNotCUs3pe:
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\s3pe" "InstallLocation"
  StrCmp $0 "" gidDone
  IfFileExists "$0\s3pe.exe" gidSetINSTDIRSub gidDone
gidSetINSTDIRSub:
  StrCpy $INSTDIR $0\Helpers
  Goto gidDone
gidSetINSTDIR:
  StrCpy $INSTDIR $0
gidDone:
  Pop $0
  ClearErrors
FunctionEnd

Function CheckInUse
  StrCpy $wasInUse 0
cuiRetry:
  IfFileExists "$INSTDIR\${EXE}" cuiExists
  Return
cuiExists:
  ClearErrors
  FileOpen $0 "$INSTDIR\${EXE}" a
  IfErrors cuiInUse
  FileClose $0
  Return
cuiInUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "${EXE} is running.$\r$\nPlease close it and retry.$\r$\n$INSTDIR\${EXE}" \
    IDRETRY cuiRetry

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to install if ${EXE} is running."
  Quit
FunctionEnd

Function CheckOldVersion
  ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "UninstallString"
  StrCmp $R0 "" covNotCU covFound
covNotCU:
  ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "UninstallString"
  StrCmp $R0 "" covNotLM covFound
covNotLM:
  ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\meshExpImp" "UninstallString"
  StrCmp $R0 "" covNotOldCU covFound
covNotOldCU:
  ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\meshExpImp" "UninstallString"
  StrCmp $R0 "" covDone
covFound:
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
    "${PROGRAM_NAME} is already installed.$\n$\nClick [OK] to remove the previous version or [Cancel] to abort this upgrade." \
    IDOK covUninstall
  Quit

covUninstall:
  ExecWait $R0
covDone:
  ClearErrors
FunctionEnd



Function un.onGUIInit
  Call un.GetInstDir
  Call un.CheckInUse


FunctionEnd

Function un.GetInstDir
  SetShellVarContext all
  ClearErrors
  Push $0
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  Pop $0
  IfErrors notCU
  SetShellVarContext current
notCU:  
  ClearErrors

  Push $0

  ReadRegStr $0 SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  StrCmp $0 "" ungidBadInstallLocation
  IfFileExists "$0" ungidSetINSTDIR
ungidBadInstallLocation:
  MessageBox MB_OK|MB_ICONSTOP "Cannot find Install Location."
  Abort
  
ungidSetINSTDIR:
  StrCpy $INSTDIR $0
  ; ReadRegStr $s3peInstDir SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "s3peInstDir"
  Pop $0
FunctionEnd

Function un.CheckInUse
  StrCpy $wasInUse 0

uncuiRetry:
  IfFileExists "$INSTDIR" uncuiExists
  MessageBox MB_OK|MB_ICONSTOP "Cannot find $INSTDIR to uninstall."
  Abort
uncuiExists:
  ClearErrors
  FileOpen $0 "$INSTDIR\${EXE}" a
  IfErrors uncuiInUse
  FileClose $0
  Return
uncuiInUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "${EXE} is running.$\r$\nPlease close it and retry.$\r$\n$INSTDIR\${EXE}" \
    IDRETRY uncuiRetry

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to uninstall if ${EXE} is running."
  Abort
FunctionEnd

UninstPage uninstConfirm
UninstPage instfiles

Section "Uninstall"
  DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}"
  DeleteRegKey SHCTX Software\s3pi\${tla}

  !include ${UNINSTFILES}
  Delete $INSTDIR\uninst-${tla}.exe
  RMDir $INSTDIR ; safe - will not delete unless folder empty
SectionEnd
