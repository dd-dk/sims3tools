;!include "MUI.nsh"

!define PROGRAM_NAME "Sims3 Demo Front End (S3PIDemoFE)"
!define tla "S3PIDemoFE"
!ifndef TARGET
  !error "Caller didn't define TARGET"
!endif
!ifndef S3PIVERSION
  !error "Caller didn't define S3PIVERSION"
!endif  
!cd ${TARGET}

XPStyle on
SetCompressor /SOLID LZMA

Var wasInUse
Var wantAll
Var wantSM
Var s3piDir


Name "${PROGRAM_NAME}"
InstallDir $PROGRAMFILES\S3PIDemoFE


; Request application privileges for Windows Vista
RequestExecutionLevel admin

LicenseData "gpl-3.0.txt"
Page license
;!insertmacro MUI_PAGE_LICENSE "gpl-3.0.txt"

PageEx components
  ComponentText "Select the installation options.  Click Next to continue." " " " "
PageExEnd
Page directory
;Var StartMenuFolder
;!insertmacro MUI_PAGE_STARTMENU "Application" $StartMenuFolder
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Section "Install for all users"
  StrCpy $wantAll "Y"
SectionEnd

Section "Create Start Menu entry"
  StrCpy $wantSM "Y"
SectionEnd

Section
  SetShellVarContext all
  StrCmp "Y" $wantAll gotAll
  SetShellVarContext current
gotAll:  

  SetOutPath $INSTDIR
  ; Write the installation path into the registry
  WriteRegStr HKLM Software\s3pi\${tla} "InstallDir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "DisplayName" "${PROGRAM_NAME}"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "UninstallString" '"$INSTDIR\uninst-${tla}.exe"'
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "NoModify" 1
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "NoRepair" 1

  WriteUninstaller uninst-${tla}.exe
  
  File /a S3PIDemoFE.exe gpl-3.0.txt ${tla}-Version.txt ;  thanks.txt

  ReadRegStr $0 SHCTX Software\s3pi\s3pi "InstallDir"
  CopyFiles /SILENT $0\s3pi.Template.Config $INSTDIR\S3PIDemoFE.exe.Config

  StrCmp "Y" $wantSM wantSM noWantSM
wantSM:
  CreateDirectory "$SMPROGRAMS\${tla}"
  CreateShortCut "$SMPROGRAMS\${tla}\${tla}.lnk" "$INSTDIR\S3PIDemoFE.exe" "" "" "" SW_SHOWNORMAL "" "${PROGRAM_NAME}"
  CreateShortCut "$SMPROGRAMS\${tla}\Uninstall.lnk" "$INSTDIR\uninst-${tla}.exe" "" "" "" SW_SHOWNORMAL "" "Uninstall"
  CreateShortCut "$SMPROGRAMS\${tla}\${tla}-Version.lnk" "$INSTDIR\${tla}-Version.txt" "" "" "" SW_SHOWNORMAL "" "Show version"
  CreateShortCut "$SMPROGRAMS\${tla}\s3pi-Version.lnk" "$0\s3pi-Version.txt" "" "" "" SW_SHOWNORMAL "" "Show library version"
noWantSM:
SectionEnd



Section "Uninstall"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}"
  DeleteRegKey HKCU Software\s3pi\${tla}
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}"
  DeleteRegKey HKLM Software\s3pi\${tla}
 
  Delete $INSTDIR\S3PIDemoFE.exe*
  Delete $INSTDIR\gpl-3.0.txt
;  Delete $INSTDIR\thanks.txt
  Delete $INSTDIR\${tla}-Version.txt
  RMDir /r "$SMPROGRAMS\${tla}"

  Delete $INSTDIR\uninst-${tla}.exe

SectionEnd


Function .onGUIInit
  Call Checks3pi
  Call Checks3piVersion
  Call GetInstDir
  Call CheckInUse
  Call CheckOldVersion
FunctionEnd

Function GetInstDir
  Push $0
  ReadRegStr $0 HKLM Software\s3pi\${tla} "InstallDir"
  StrCmp $0 "" NotInstalledLM
  StrCpy $INSTDIR $0
  Goto InstDirDone
NotInstalledLM:
  ReadRegStr $R0 HKCU Software\s3pi\${tla} "InstallDir"
  StrCmp $0 "" InstDirDone
  StrCpy $INSTDIR $0
InstDirDone:
  Pop $0
FunctionEnd

Function CheckInUse
  StrCpy $wasInUse 0

  IfFileExists "$INSTDIR\S3PIDemoFE.exe" Exists
  Return
Exists:
  ClearErrors
  FileOpen $0 "$INSTDIR\S3PIDemoFE.exe" a
  IfErrors InUse
  FileClose $0
  Return
InUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "S3PIDemoFE.exe is running.$\r$\nPlease close it and retry.$\r$\n$INSTDIR\S3PIDemoFE.exe" \
    IDRETRY Exists

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to install if S3PIDemoFE.exe is running."
  Quit
FunctionEnd

Function CheckOldVersion
  ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "UninstallString"
  StrCmp $R0 "" NotInstalledCU Installed
NotInstalledCU:
  ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}" "UninstallString"
  StrCmp $R0 "" NotInstalled
Installed:
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
    "${PROGRAM_NAME} is already installed.$\n$\nClick [OK] to remove the previous version or [Cancel] to abort this upgrade." \
    IDOK UnInstall
  Quit
UnInstall:
  ExecWait "$R0"
NotInstalled:
  ClearErrors
FunctionEnd

Function Checks3pi
  ReadRegStr $s3piDir HKLM Software\s3pi\s3pi "InstallDir"
  StrCmp "" $s3piDir NoCustomLM
  IfFileExists "$s3piDir\s3pi.Interfaces.dll" DoneChecks3pi
NoCustomLM:
  ReadRegStr $s3piDir HKCU Software\s3pi\s3pi "InstallDir"
  StrCmp "" $s3piDir DoneChecks3pi
  IfFileExists "$s3piDir\s3pi.Interfaces.dll" DoneChecks3pi
  MessageBox MB_OK "This program requires s3pi.  Please install the library first."
  Quit
DoneChecks3pi:
FunctionEnd

Function Checks3piVersion
  Push $R0
  Push $0
  Push $1
  Push $2
  
  ClearErrors
  FileOpen $R0 "$s3piDir\s3pi-Version.txt" r
  IfErrors Nos3piVersion
  FileRead $R0 $0
  FileClose $R0
  
  DetailPrint "s3pi Version installed: $0"
  DetailPrint "s3pi Version required:  ${S3PIVERSION}"

  StrCpy $1 $0 4
  StrCpy $2 "${S3PIVERSION}" 4
  IntCmp $1 $2 yymmequal NeedNew3piVersion DoneChecks3piVersion
yymmequal:
  StrCpy $1 $0 2 5
  StrCpy $2 "${S3PIVERSION}" 2 5
  IntCmp $1 $2 ddequal NeedNew3piVersion DoneChecks3piVersion
ddequal:
  StrCpy $1 $0 2 8
  StrCpy $2 "${S3PIVERSION}" 2 8
  IntCmp $1 $2 DoneChecks3piVersion NeedNew3piVersion DoneChecks3piVersion

NeedNew3piVersion:
  MessageBox MB_OK \
    "This program requires s3pi library version\$\n${S3PIVERSION}$\nor later.  Please install that first."
  Quit
Nos3piVersion:
  MessageBox MB_OK "This program requires s3pi.  Please install the library first."
  Quit
DoneChecks3piVersion:
  Pop $1
  Pop $0
  Pop $R0
FunctionEnd
