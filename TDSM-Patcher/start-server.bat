@echo off
cls
:start
tdsm.exe -config server.config
@echo.
@echo Restarting server...
@echo.
goto start
