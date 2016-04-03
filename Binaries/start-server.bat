@echo off
cls
:start
TerrariaServer.exe -config server.config
@echo.
@echo Restarting server...
@echo.
goto start