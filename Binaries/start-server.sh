#!/bin/bash
while true
do
	mono TerrariaServer.exe -config serverconfig.txt
	echo "Restarting server..."
	sleep 1
done
