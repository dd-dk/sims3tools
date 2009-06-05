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
Var delSettings
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
PageEx un.components
  ComponentText "Select the uninstallation options.  Click Next to continue." " " " "
PageExEnd
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



Section /o "un.Delete user settings"
  StrCpy $delSettings "Y"
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
  StrCmp "Y" $delSettings DelSettings UninstallDone
DelSettings:
  Call un.InstallUserSettings
UninstallDone:
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

Function un.InstallUserSettings
  Push "S3PIDemoFE.exe_Url_*"
  Push "$LOCALAPPDATA"

  Push $0
  GetFunctionAddress $0 "un.DeleteFile"
  Exch $0
  
  Push 1
  Push 0
  Call un.SearchFile
FunctionEnd

Function un.DeleteFile
;  DetailPrint "Remove folder $R4"
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
    "OK to remove folder $R4" \
    IDOK removeFolder
  Push "Stop"
  Return
removeFolder:  
  RMDir /r "$R4"
  Push "Go"
FunctionEnd


;----------------------------------------------------------------------------
; Title             : Search file or directory (alternative)
; Short Name        : SearchFile
; Last Changed      : 22/Feb/2005
; Code Type         : Function
; Code Sub-Type     : One-way Input, Callback Dependant
;----------------------------------------------------------------------------
; Description       : Searches for a file or folder into a folder of your
;                     choice.
;----------------------------------------------------------------------------
; Function Call     : Push "(filename.ext|foldername)"
;                       File or folder to search. Wildcards are supported.
;
;                     Push "Path"
;                       Path where to search for the file or folder.
;
;                     Push $0
;
;                     GetFunctionAddress $0 "CallbackFunction"
;                       Custom callback function name where the search is
;                       returned to.
;
;                     Exch $0
;
;                     Push "(1|0)"
;                       Include subfolders in search. (0= false, 1= true)
;
;                     Push "(1|0)"
;                       Enter subfolders with ".". This only works if
;                       "Include subfolders in search" is set to 1 (true).
;                       (0= false, 1= true)
;
;                     Call SearchFile
;----------------------------------------------------------------------------
; Callback Variables: $R0 ;Directory being searched at that time.
;                     $R1 ;File or folder to search (same as 1st push).
;                     $R2 ;Reserved.
;                     $R3 ;File or folder found without path.
;                     $R4 ;File or folder found with path (same as $R0/$R3).
;                     $R5 ;Function address provided by "GetFunctionAddress".
;                     $R6 ;"Include subfolders in search" option.
;                     $R7 ;"Enter subfolders with "."" option.
;----------------------------------------------------------------------------
; Author            : Diego Pedroso
; Author Reg. Name  : deguix
;----------------------------------------------------------------------------
 
Function un.SearchFile
 
  Exch 4
  Exch
  Exch 3
  Exch $R0 ; directory in which to search
;DetailPrint "directory in which to search: $R0"
  Exch 4
  Exch
  Exch $R1 ; file or folder name to search in
;DetailPrint "file or folder name to search in: $R1"
  Exch 3
  Exch 2
  Exch $R2
  Exch 2
  Exch $R3
  Exch
  Push $R4
  Exch
  Push $R5
  Exch
  Push $R6
  Exch
  Exch $R7 ;search folders with "."
 
  StrCpy $R5 $R2 ;$R5 = custom function name
  StrCpy $R6 $R3 ;$R6 = include subfolders
 
  StrCpy $R2 ""
  StrCpy $R3 ""
 
  # Remove \ from end (if any) from the file name or folder name to search
  StrCpy $R2 $R1 1 -1
  StrCmp $R2 \ 0 +2
  StrCpy $R1 $R1 -1
 
  # Detect if the search path have backslash to add the backslash
  StrCpy $R2 $R0 1 -1
  StrCmp $R2 \ +2
  StrCpy $R0 "$R0\"
 
  # File (or Folder) Search
  ##############
 
  # Get first file or folder name
 
  FindFirst $R2 $R3 "$R0$R1"
 
  FindNextFile:
 
  # This loop, search for files or folders with the same conditions.
 
    StrCmp $R3 "" NoFiles
      StrCpy $R4 "$R0$R3"
 
  # Preparing variables for the Callback function
 
    Push $R7
    Push $R6
    Push $R5
    Push $R4
    Push $R3
    Push $R2
    Push $R1
    Push $R0
 
  # Call the Callback function
 
    Call $R5
 
  # Returning variables
 
    Push $R8
    Exch
    Pop $R8
 
    Exch
    Pop $R0
    Exch
    Pop $R1
    Exch
    Pop $R2
    Exch
    Pop $R3
    Exch
    Pop $R4
    Exch
    Pop $R5
    Exch
    Pop $R6
    Exch
    Pop $R7
 
    StrCmp $R8 "Stop" 0 +3
      Pop $R8
      Goto Done
 
    Pop $R8
 
  # Detect if have another file
 
    FindNext $R2 $R3
      Goto FindNextFile ;and loop!
 
  # If don't have any more files or folders with the condictions
 
  NoFiles:
 
  FindClose $R2
 
  # Search in Subfolders
  #############
 
  # If you don't want to search in subfolders...
 
  StrCmp $R6 0 NoSubfolders 0
 
  # SEARCH FOLDERS WITH DOT
 
  # Find the first folder with dot
 
  StrCmp $R7 1 0 EndWithDot
 
    FindFirst $R2 $R3 "$R0*.*"
    StrCmp $R3 "" NoSubfolders
      StrCmp $R3 "." FindNextSubfolderWithDot 0
        StrCmp $R3 ".." FindNextSubfolderWithDot 0
          IfFileExists "$R0$R3\*.*" RecallingOfFunction 0
 
  # Now, detect the next folder with dot
 
      FindNextSubfolderWithDot:
 
      FindNext $R2 $R3
      StrCmp $R3 "" NoSubfolders
        StrCmp $R3 "." FindNextSubfolder 0
          StrCmp $R3 ".." FindNextSubfolder 0
            IfFileExists "$R0$R3\*.*" RecallingOfFunction FindNextSubfolderWithDot
 
  EndWithDot:
 
  # SEARCH FOLDERS WITHOUT DOT
 
  # Skip ., and .. (C:\ don't have .., so have to detect if is :\)
 
  FindFirst $R2 $R3 "$R0*."
 
  Push $R6
 
  StrCpy $R6 $R0 "" 1
  StrCmp $R6 ":\" +2
 
  FindNext $R2 $R3
 
  Pop $R6
 
  # Now detect the "really" subfolders, and loop
 
  FindNextSubfolder:
 
  FindNext $R2 $R3
  StrCmp $R3 "" NoSubfolders
    IfFileExists "$R0$R3\" FindNextSubfolder
 
  # Now Recall the function (making a LOOP)!
 
  RecallingOfFunction:
 
  Push $R1
  Push "$R0$R3\"
  Push "$R5"
  Push "$R6"
  Push "$R7"
    Call un.SearchFile
 
  # Now, find the next Subfolder
 
    Goto FindNextSubfolder
 
  # If don't exist more subfolders...
 
  NoSubfolders:
 
  FindClose $R2
 
  # Returning Values to User
 
  Done:
 
  Pop $R7
  Pop $R6
  Pop $R5
  Pop $R4
  Pop $R3
  Pop $R2
  Pop $R1
  Pop $R0
 
FunctionEnd
