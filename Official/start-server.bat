@echo off
cls
:start
tdsm.microsoft.exe -config serverconfig.txt
@echo.
@echo Restarting server...
@echo.
goto start