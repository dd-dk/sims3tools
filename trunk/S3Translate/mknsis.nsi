;!include "MUI.nsh"

!define PROGRAM_NAME "jonha's S3Translate"
!define tla "S3Translate"
!ifndef INSTFILES
  !error "Caller didn't define INSTFILES"
!endif
!ifndef UNINSTFILES
  !error "Caller didn't define UNINSTFILES"
!endif

XPStyle on
SetCompressor /SOLID LZMA

Var wasInUse
Var wantAll
Var wantSM
; Var delSettings

Name "${PROGRAM_NAME}"
InstallDir $PROGRAMFILES\${tla}
!define EXE ${tla}.exe

AddBrandingImage top 0
; Icon Resources\${tla}.ico
; UninstallIcon Resources\${tla}.ico

; Request application privileges for Windows Vista
RequestExecutionLevel admin

LicenseData "gpl.txt"
Page license
;!insertmacro MUI_PAGE_LICENSE "gpl.txt"

PageEx components
  ComponentText "Select the installation options.  Click Next to continue." " " " "
PageExEnd
Page directory
;Var StartMenuFolder
;!insertmacro MUI_PAGE_STARTMENU "Application" $StartMenuFolder
Page instfiles

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
  
  !include ${INSTFILES}

  StrCmp "Y" $wantSM wantSM noWantSM
wantSM:
  CreateDirectory "$SMPROGRAMS\${tla}"
  CreateShortCut "$SMPROGRAMS\${tla}\${tla}.lnk" "$INSTDIR\${EXE}" "" "" "" SW_SHOWNORMAL "" "${PROGRAM_NAME}"
  CreateShortCut "$SMPROGRAMS\${tla}\Uninstall.lnk" "$INSTDIR\uninst-${tla}.exe" "" "" "" SW_SHOWNORMAL "" "Uninstall"
  CreateShortCut "$SMPROGRAMS\${tla}\${tla}-Version.lnk" "$INSTDIR\${tla}-Version.txt" "" "" "" SW_SHOWNORMAL "" "Show version"
noWantSM:
SectionEnd

Function .onGUIInit
;  SetOutPath $TEMP
;  File ..\Resources\${tla}.ico
;  SetBrandingImage $TEMP\${tla}.ico
;  Delete $TEMP\${tla}.ico
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

  IfFileExists "$INSTDIR\${EXE}" Exists
  Return
Exists:
  ClearErrors
  FileOpen $0 "$INSTDIR\${EXE}" a
  IfErrors InUse
  FileClose $0
  Return
InUse:
  StrCpy $wasInUse 1

  MessageBox MB_RETRYCANCEL|MB_ICONQUESTION \
    "${EXE} is running.$\r$\nPlease close it and retry.$\r$\n$INSTDIR\${EXE}" \
    IDRETRY Exists

  MessageBox MB_OK|MB_ICONSTOP "Cannot continue to install if ${EXE} is running."
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




UninstPage uninstConfirm
PageEx un.components
  ComponentText "Select the uninstallation options.  Click Next to continue." " " " "
PageExEnd
UninstPage instfiles

; Section /o "un.Delete user settings"
;   StrCpy $delSettings "Y"
; SectionEnd

Section "Uninstall"
  SetShellVarContext all
  ClearErrors
  Push $0
  ReadRegStr $0 HKCU Software\s3pi\${tla} "InstallDir"
  Pop $0
  IfErrors notCU
  SetShellVarContext current
notCU:  

  DeleteRegKey SHCTX Software\Microsoft\Windows\CurrentVersion\Uninstall\${tla}
  DeleteRegKey SHCTX Software\s3pi\${tla}

  RMDir /r "$SMPROGRAMS\${tla}"

  !include ${UNINSTFILES}
  Delete $INSTDIR\uninst-${tla}.exe
  RMDir $INSTDIR ; safe - will not delete unless folder empty

;   StrCmp "Y" $delSettings DelSettings UninstallDone
; DelSettings:
;   Call un.InstallUserSettings
; UninstallDone:
SectionEnd

Function un.InstallUserSettings
  Push "${EXE}_Url_*"
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
