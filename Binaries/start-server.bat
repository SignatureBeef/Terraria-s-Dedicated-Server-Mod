@echo off
cls
:start
tdsm.exe -config serverconfig.txt
@echo.
@echo Restarting server...
@echo.
goto start