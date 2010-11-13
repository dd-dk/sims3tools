@echo off
set av=%*
set mydate=%date: =0%
set dd=%mydate:~0,2%
set mm=%mydate:~3,2%
set yy=%mydate:~8,2%
set mytime=%time: =0%
set h=%mytime:~0,2%
set version="%yy%%mm%.%dd%.%h%%m%.*"

(echo using System.Reflection;
echo using System.Runtime.CompilerServices;
echo using System.Runtime.InteropServices;
echo /*"*/ [assembly: AssemblyVersion(%version%)] /*"*/
)>"%av%"
