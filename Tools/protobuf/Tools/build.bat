

set DIR=%cd%\..\Protocals
set CS_OUT=..\..\..\Assets\Scripts\Protocals\
set PY_OUT=..\..\..\GameServer\Messages\
set Files=Messages.proto RequestMessages.proto RespondMessages.proto

rem .\Windows\protogen -I..\Protocals\ +langver=4.0 +names=original --csharp_out=..\..\..\Assets\Scripts\Protocals\ %Files%
.\Windows\protogen -I%DIR% +langver=4.0 +names=original --csharp_out=%CS_OUT% %Files%
protoc.exe -I%DIR% --python_out=%PY_OUT% %Files%