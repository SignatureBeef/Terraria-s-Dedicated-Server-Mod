@echo off
cls
:start
TerrariaServer.exe -config serverconfig.txt
@echo.
@echo Restarting server...
@echo.
goto start