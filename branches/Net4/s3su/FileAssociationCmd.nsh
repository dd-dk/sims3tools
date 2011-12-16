/*
_____________________________________________________________________________
 
                       File Association
_____________________________________________________________________________
 
 Based on code taken from http://nsis.sourceforge.net/File_Association 
 This version:
 a) doesn't set the default file association, only adds menu entries
 b) allows command line arguments to be given as well as the exe name
 
 Usage in script:
 1. !include "FileAssociationCmd.nsh"
 2. [Section|Function]
      ${FileAssociationCmdFunction} "Param1" "Param2" "..." $var
    [SectionEnd|FunctionEnd]
 
 FileAssociationCmdFunction=[RegisterCmdExtension|UnRegisterCmdExtension]
 
_____________________________________________________________________________
 
 ${RegisterCmdExtension} "[extension]" "[filetype name]" "[action]" "[executable]" "[arguments]"
 
"[extension]"      ; extension, which represents the file format to open
                   ;
"[filetype name]"  ; description for the extension. This will be display in Windows Explorer.
                   ;
"[action]"         ; the "verb" to be performed on the file, displayed in the context menu.
                   ;
"[executable]"     ; executable which opens the file format
                   ;
"[arguments]"      ; arguments after executable before filename
                   ;
 
 
 ${UnRegisterCmdExtension} "[extension]" "[filetype name]" "[action]"
 
"[extension]"      ; extension, which represents the file format to open
                   ;
"[filetype name]"  ; description for the extension. This will be display in Windows Explorer.
                   ;
"[action]"         ; the "verb" to be performed on the file, displayed in the context menu.
                   ;

_____________________________________________________________________________
 
                         Macros
_____________________________________________________________________________
 
 Change log window verbosity (default: 3=no script)
 
 Example:
 !include "FileAssociationCmd.nsh"
 !insertmacro RegisterCmdExtension
 ${FileAssociationCmd_VERBOSE} 4   # all verbosity
 !insertmacro UnRegisterCmdExtension
 ${FileAssociationCmd_VERBOSE} 3   # no script
*/
 
 
!ifndef FileAssociationCmd_INCLUDED
!define FileAssociationCmd_INCLUDED
 
!include Util.nsh
 
!verbose push
!verbose 3
!ifndef _FileAssociationCmd_VERBOSE
  !define _FileAssociationCmd_VERBOSE 3
!endif
!verbose ${_FileAssociationCmd_VERBOSE}
!define FileAssociationCmd_VERBOSE `!insertmacro FileAssociationCmd_VERBOSE`
!verbose pop
 
!macro FileAssociationCmd_VERBOSE _VERBOSE
  !verbose push
  !verbose 3
  !undef _FileAssociationCmd_VERBOSE
  !define _FileAssociationCmd_VERBOSE ${_VERBOSE}
  !verbose pop
!macroend
 
 
 
!macro RegisterCmdExtensionCall _EXTENSION _TYPE _ACTION _EXECUTABLE _ARGS
  !verbose push
  !verbose ${_FileAssociationCmd_VERBOSE}
  Push `${_ARGS}`
  Push `${_EXECUTABLE}`
  Push `${_ACTION}`
  Push `${_TYPE}`
  Push `${_EXTENSION}`
  ${CallArtificialFunction} RegisterCmdExtension_
  !verbose pop
!macroend
 
!macro UnRegisterCmdExtensionCall _EXTENSION _TYPE _ACTION
  !verbose push
  !verbose ${_FileAssociationCmd_VERBOSE}
  Push `${_ACTION}`
  Push `${_TYPE}`
  Push `${_EXTENSION}`
  ${CallArtificialFunction} UnRegisterCmdExtension_
  !verbose pop
!macroend
 
 
 
!define RegisterCmdExtension `!insertmacro RegisterCmdExtensionCall`
!define un.RegisterCmdExtension `!insertmacro RegisterCmdExtensionCall`
 
!macro RegisterCmdExtension
!macroend
 
!macro un.RegisterCmdExtension
!macroend
 
!macro RegisterCmdExtension_
  !verbose push
  !verbose ${_FileAssociationCmd_VERBOSE}
 
  Exch 4
  Exch $R3 ;args
  Exch 4
  Exch 3
  Exch $R2 ;exe
  Exch 3
  Exch 2
  Exch $R1 ;action
  Exch 2
  Exch
  Exch $R4 ;type
  Exch
  Exch $R0 ;ext
  Push $0
  Push $1
 
  ReadRegStr $1 HKCR $R0 ""  ; read current file association
  StrCmp "$1" "" SetOurs  ; is it empty
  StrCmp "$1" "$R4" NoSetOurs  ; is it our own already
  ReadRegStr $R4 HKCR $R0 ""  ; read current file association type into $R4
  Goto NoSetOurs
SetOurs:
  WriteRegStr HKCR $R0 "" "$R4"  ; set our file association type
NoSetOurs:
 
  ReadRegStr $0 HKCR $R4 "" ; Read the current file association type
  StrCmp $0 "" 0 Skip ; If there's one set, skip...
  WriteRegStr HKCR "$R4" "" "$R4"
  WriteRegStr HKCR "$R4\DefaultIcon" "" "$R2,0"
Skip:
  WriteRegStr HKCR "$R4\shell\$R1" "" "$R1 $R4"
  WriteRegStr HKCR "$R4\shell\$R1\command" "" '"$R2" $R3 "%1"'

  Pop $1
  Pop $0
  Pop $R0
  Pop $R4
  Pop $R1
  Pop $R2
  Pop $R3
 
  !verbose pop
!macroend
 
 
 
!define UnRegisterCmdExtension `!insertmacro UnRegisterCmdExtensionCall`
!define un.UnRegisterCmdExtension `!insertmacro UnRegisterCmdExtensionCall`
 
!macro UnRegisterCmdExtension
!macroend
 
!macro un.UnRegisterCmdExtension
!macroend
 
!macro UnRegisterCmdExtension_
  !verbose push
  !verbose ${_FileAssociationCmd_VERBOSE}
 
  Exch 2
  Exch $R2 ;action
  Exch 2
  Exch
  Exch $R1 ;type
  Exch
  Exch $R0 ;ext
  Push $0
  Push $1

  ReadRegStr $1 HKCR $R0 ""  ; read current file association
  StrCmp "$1" "" NotSet ; no current value, give up
  ReadRegStr $0 HKCR $1 "" ; Read the current file association type
  StrCmp "$0" "" NotSet ; no current value, give up

  ReadRegStr $1 HKCR "$0\shell\$R2" "" ; menu entry
  StrCmp "$1" "$R2 $R1" 0 NotSet ; not our value, give up
  ReadRegStr $1 HKCR "$0\shell\$R2\command" "" ; command line
  StrCmp "$1" "" NotSet ; no value, give up

  ;; OK, so we've checked everything out - delete the association
  DeleteRegKey HKCR "$0\shell\$R2"
 
NotSet:
  Pop $1
  Pop $0
  Pop $R0
  Pop $R1
 
  !verbose pop
!macroend
 
!endif # !FileAssociationCmd_INCLUDED
