#!/bin/bash
while true
do
	mono tdsm.exe -config server.config
	echo "Restarting server..."
	sleep 1
done
