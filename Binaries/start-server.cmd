@echo off
:: AutoRestart: Set to 1 to enable it, 0 (zero) to disable. When
:: enabled, the server will automatically restart if it's
:: terminated for any reason including using the exit command or
:: a server crash.
:: ~~~~ BEGIN USER EDIT ~~~~
set AutoRestart=1
:: ~~~~ END USER EDIT ~~~~

:start
TerrariaServer.exe -config serverconfig.txt

if %AutoRestart%==1 (
	echo.
	echo Restarting server...
	echo.
	goto start
)
