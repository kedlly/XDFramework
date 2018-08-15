
set BAT_DIR=%~dp0
set DIR=%BAT_DIR%\..\Protocals
set CS_OUT=%BAT_DIR%\..\..\..\Assets\Scripts\Protocals\
set PY_OUT=%BAT_DIR%\..\..\..\GameServer\Messages\
set Files=Messages.proto RequestMessages.proto RespondMessages.proto

rem .\Windows\protogen -I..\Protocals\ +langver=4.0 +names=original --csharp_out=..\..\..\Assets\Scripts\Protocals\ %Files%
%BAT_DIR%\Windows\protogen -I%DIR% +langver=4.0 +names=original --csharp_out=%CS_OUT% %Files%
protoc.exe -I%DIR% --python_out=%PY_OUT% %Files%