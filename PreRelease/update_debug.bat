MKDIR Debug

:Delete files
DEL "Debug\Terraria_Server.exe"
DEL "Debug\Terraria_Server.pdb"
DEL "Debug\Terraria_Server.mdb"

DEL "Debug\Restrict.dll"
DEL "Debug\Restrict.pdb"
DEL "Debug\Restrict.mdb"

DEL "Debug\TDSMPermissions.dll"
DEL "Debug\TDSMPermissions.pdb"
DEL "Debug\TDSMPermissions.mdb"

DEL "Debug\TDSM_PermissionsX.dll"
DEL "Debug\TDSM_PermissionsX.pdb"
DEL "Debug\TDSM_PermissionsX.mdb"

DEL "Debug\Regions.dll"
DEL "Debug\Regions.pdb"
DEL "Debug\Regions.mdb"

DEL "Debug\Languages.exe"
DEL "Debug\Languages.pdb"

:Copy new versions
copy "..\Terraria_Server\bin\Debug\Terraria_Server.exe" "Debug\Terraria_Server.exe"
copy "..\Terraria_Server\bin\Debug\Terraria_Server.pdb" "Debug\Terraria_Server.pdb"

copy "..\Restrict\bin\Debug\RestrictPlugin.dll" "Debug\RestrictPlugin.dll"
copy "..\Restrict\bin\Debug\RestrictPlugin.pdb" "Debug\RestrictPlugin.pdb"

copy "..\TDSMPermissions\TDSMPermissions\bin\Debug\TDSMPermissions.dll" "Debug\TDSMPermissions.dll"
copy "..\TDSMPermissions\TDSMPermissions\bin\Debug\TDSMPermissions.pdb" "Debug\TDSMPermissions.pdb"

copy "..\TDSM_PermissionsX\bin\Debug\TDSM_PermissionsX.dll" "Debug\TDSM_PermissionsX.dll"
copy "..\TDSM_PermissionsX\bin\Debug\TDSM_PermissionsX.pdb" "Debug\TDSM_PermissionsX.pdb"

copy "..\Regions\bin\Debug\Regions.dll" "Debug\Regions.dll"
copy "..\Regions\bin\Debug\Regions.pdb" "Debug\Regions.pdb"

copy "..\Languages\bin\Debug\Languages.exe" "Debug\Languages.exe"
copy "..\Languages\bin\Debug\Languages.pdb" "Debug\Languages.pdb"

cd "Debug\"

:Update exe to mdb
C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Terraria_Server.exe"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Regions.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "RestrictPlugin.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "TDSMPermissions.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "TDSM_PermissionsX.dll"

cd ..
:PAUSE