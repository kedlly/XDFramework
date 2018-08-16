
@echo off

set BAT_DIR=%~dp0
set DIR=%BAT_DIR%..\Protocals
set CS_OUT=%BAT_DIR%..\..\..\Assets\Scripts\Protocals
set PY_OUT=%BAT_DIR%..\..\..\GameServer\Messages
set Files=Message.proto RequestMessages.proto RespondMessages.proto
set RawDataFiles=InternalData.proto

rem .\Windows\protogen -I..\Protocals\ +langver=4.0 +names=original --csharp_out=..\..\..\Assets\Scripts\Protocals\ %Files%

rem call :complieGeneralFiles
rem call :complieRawDataFiles

call :complieFiles . %FILES%
call :complieFiles RawData %RawDataFiles%

goto :eof

:complieFiles 
    set subDir=%1
    :loop
        shift
        if "%1" == "" (
            goto :endLoop
        ) else (
            rem rem generate cs code
                                                                                                                        @echo on
            %BAT_DIR%\Windows\protogen -I%DIR% +langver=4.0 +names=original --csharp_out=%CS_OUT% %subDir%\%1
                                                                                                                        @echo off
            rem generate python code
            if not exist %PY_OUT%\%subDir% (
                mkdir %PY_OUT%\%subDir%
                echo '''Module File''' > %PY_OUT%\RawData\__init__.py
             )
                                                                                                                        @echo on
            protoc.exe -I%DIR%\%subDir% --python_out=%PY_OUT%\%subDir% %1
            if %ERRORLEVEL% == 0 (
                echo compile %DIR%\%subDir%\%1 succeed when generating python code
            ) else (
                echo compile %DIR%\%subDir%\%1 failed when generating python code
            )
                                                                                                                        @echo off
            goto :loop
        )
    :endLoop
exit /b 0
    
