
.\Windows\protogen -I..\Protocals\ +langver=4.0 +names=original --csharp_out=..\..\..\Assets\Scripts\Protocals\ *.proto

set DIR=D:\GitHub\XDFramework\Tools\protobuf\Protocals\
protoc.exe -I%DIR% --python_out=. Login.proto
protoc.exe -I%DIR% --python_out=. Messages.proto