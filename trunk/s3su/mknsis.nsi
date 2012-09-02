;!include "MUI.nsh"
!include "FileAssociationCmd.nsh"

!define tla "s3su"
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
Var wantSM
Var wantAssoc
;Var wantSendTo
;Var delSettings


  InstallDir $PROGRAMFILES64\${tla}
  !define PROGRAM_NAME "Sims3Pack Utility"
  !define INSTREGKEY "${tla}"
  !define SMDIR "$SMPROGRAMS\${tla}"
  !define EXE S3Pack.exe
  !define LNK1 "${tla} unpack.lnk"
  !define LNK2 "${tla} repack.lnk"
  !define LNK3 "${tla} pack.lnk"







SetCompressor /SOLID LZMA
XPStyle on
Name "${PROGRAM_NAME}"
AddBrandingImage top 0
Icon s3pe.ico
UninstallIcon s3pe.ico

; Request application privileges for Windows Vista and above
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

Section "Install for all users"
  StrCpy $wantAll "Y"
SectionEnd

Section "Create Start Menu entry"
  StrCpy $wantSM "Y"
SectionEnd

Section /o "Create file associations?"
  StrCpy $wantAssoc "Y"
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
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "HelpLink" "http://www.den.simlogical.com/denforum/index.php?topic=198.0"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "Publisher" "Peter L Jones"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "UninstallString" '"$INSTDIR\uninst-${tla}.exe"'
  ; $0 is set in ${INSTFILES} by the batch file...
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "EstimatedSize" $0
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "NoModify" 1
  WriteRegDWORD SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "NoRepair" 1

  StrCmp "Y" $wantSM wantSM noWantSM
wantSM:
  CreateDirectory "${SMDIR}"
  CreateShortCut "${SMDIR}\${LNK1}" "$INSTDIR\${EXE}" "/unpack" "" "" SW_SHOWNORMAL "" "${PROGRAM_NAME} Unpacker"
  CreateShortCut "${SMDIR}\${LNK2}" "$INSTDIR\${EXE}" "/repack" "" "" SW_SHOWNORMAL "" "${PROGRAM_NAME} Repacker"
  CreateShortCut "${SMDIR}\${LNK3}" "$INSTDIR\${EXE}" "/pack" "" "" SW_SHOWNORMAL "" "${PROGRAM_NAME} Packer"
  CreateShortCut "${SMDIR}\Uninstall.lnk" "$INSTDIR\uninst-${tla}.exe" "" "" "" SW_SHOWNORMAL "" "Uninstall"
  CreateShortCut "${SMDIR}\${tla}-Version.lnk" "$INSTDIR\${tla}-Version.txt" "" "" "" SW_SHOWNORMAL "" "Show version"
noWantSM:

  StrCmp "Y" $wantAssoc wantAssoc noWantAssoc
wantAssoc:
; ${RegisterCmdExtension} "[extension]" "[filetype name]" "[action]" "[executable]" "[arguments]"
  ${RegisterCmdExtension} ".Sims3Pack" "Sims3™ Custom Content" "Unpack" "$INSTDIR\${EXE}" "/unpack"
  ${RegisterCmdExtension} ".xml" "Sims3Pack XML" "Create Sims3Pack from" "$INSTDIR\${EXE}" "/repack"
  ${RegisterCmdExtension} ".package" "Sims3 package" "Create Sims3Pack from" "$INSTDIR\${EXE}" "/pack"
  WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "RemoveAssociation" "Y"
noWantAssoc:


SectionEnd

Function .onGUIInit
  SetOutPath $TEMP
  File ..\s3pe.ico
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
  IfFileExists "$0${EXE}" gidSetINSTDIR
gidNotCU:
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "InstallLocation"
  StrCmp $0 "" gidDone
  IfFileExists "$0${EXE}" gidSetINSTDIR gidDone
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
  Call un.GetWantAssoc

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

Function un.GetWantAssoc
  Push $0
  ReadRegStr $0 SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}" "RemoveAssociation"
  StrCmp $0 "" ungwaDone
  StrCpy $wantAssoc $0
ungwaDone:
  Pop $0
FunctionEnd










UninstPage uninstConfirm
PageEx un.components
  ComponentText "Select the uninstallation options.  Click Next to continue." " " " "
PageExEnd
UninstPage instfiles

;Section /o "un.Delete user settings"
;  StrCpy $delSettings "Y"
;SectionEnd

Section "Uninstall"

  DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTREGKEY}"
  DeleteRegKey SHCTX Software\s3pi\${tla}

  RMDir /r "${SMDIR}"

  StrCmp "Y" $wantAssoc wantAssoc noWantAssoc
wantAssoc:
; ${UnRegisterCmdExtension} "[extension]" "[filetype name]" "[action]"
  ${unregisterCmdExtension} ".Sims3Pack" "Sims3™ Custom Content" "Unpack"
  ${unregisterCmdExtension} ".package" "Sims3 package" "Create Sims3Pack from"
  ${unregisterCmdExtension} ".xml" "Sims3Pack XML" "Create Sims3Pack from"
noWantAssoc:



  !include ${UNINSTFILES}
  Delete $INSTDIR\uninst-${tla}.exe
  RMDir $INSTDIR ; safe - will not delete unless folder empty

;  StrCmp "Y" $delSettings DelSettings UninstallDone
;DelSettings:
;  Call un.InstallUserSettings
;UninstallDone:
SectionEnd

;Function un.InstallUserSettings
;  Push "${EXE}_Url_*"
;  Push "$LOCALAPPDATA"
;
;  Push $0
;  GetFunctionAddress $0 "un.DeleteFile"
;  Exch $0
;  
;  Push 1
;  Push 0
;  Call un.SearchFile
;FunctionEnd
;
;Function un.DeleteFile
;;  DetailPrint "Remove folder $R4"
;  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
;    "OK to remove folder $R4" \
;    IDOK removeFolder
;  Push "Stop"
;  Return
;removeFolder:  
;  RMDir /r "$R4"
;  Push "Go"
;FunctionEnd


/*----------------------------------------------------------------------------
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
*/
 
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
