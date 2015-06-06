clear
#Update DNU packages
echo Updating dnu packages...
dnu restore

cp ../Official/TerrariaServer.exe .

#Patch TDSM into Terraria
echo Patching Terraria
dnx . run -norun -nover

#Now setup TDSM in a isolated area
MKDIR corenet
MKDIR corenet/Plugins
cd corenet

#Crucial that we have tdsm.exe. This is because both the file name and assembly name must match.
cp ../tdsm.mono.exe tdsm.exe
cp ../tdsm.api.dll .
cp ../KopiLua.dll .
cp ../NLua.dll .
cp ../Plugins/tdsm.core.dll Plugins/.

echo Writing the run file
cat > tdsm.sh << EOF
ulimit -n 12288
dnx tdsm.exe
EOF

chmod u+x tdsm.sh

echo You now can run tdsm.exe using \"./tdsm.sh\" from inside the corenet directory.