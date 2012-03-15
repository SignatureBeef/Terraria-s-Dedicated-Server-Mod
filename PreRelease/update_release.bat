MKDIR Release

:Delete files
DEL "Release\Terraria_Server.exe"
DEL "Release\Terraria_Server.pdb"
DEL "Release\Terraria_Server.mdb"

DEL "Release\Restrict.dll"
DEL "Release\Restrict.pdb"
DEL "Release\Restrict.mdb"

DEL "Release\TDSMPermissions.dll"
DEL "Release\TDSMPermissions.pdb"
DEL "Release\TDSMPermissions.mdb"

DEL "Release\TDSM_PermissionsX.dll"
DEL "Release\TDSM_PermissionsX.pdb"
DEL "Release\TDSM_PermissionsX.mdb"

DEL "Release\Regions.dll"
DEL "Release\Regions.pdb"
DEL "Release\Regions.mdb"

:Copy new versions
copy "..\Terraria_Server\bin\Release\Terraria_Server.exe" "Release\Terraria_Server.exe"
copy "..\Terraria_Server\bin\Release\Terraria_Server.pdb" "Release\Terraria_Server.pdb"

copy "..\Restrict\bin\Release\RestrictPlugin.dll" "Release\RestrictPlugin.dll"
copy "..\Restrict\bin\Release\RestrictPlugin.pdb" "Release\RestrictPlugin.pdb"

copy "..\TDSMPermissions\TDSMPermissions\bin\Release\TDSMPermissions.dll" "Release\TDSMPermissions.dll"
copy "..\TDSMPermissions\TDSMPermissions\bin\Release\TDSMPermissions.pdb" "Release\TDSMPermissions.pdb"

copy "..\TDSM_PermissionsX\bin\Release\TDSM_PermissionsX.dll" "Release\TDSM_PermissionsX.dll"
copy "..\TDSM_PermissionsX\bin\Release\TDSM_PermissionsX.pdb" "Release\TDSM_PermissionsX.pdb"

copy "..\Regions\bin\Release\Regions.dll" "Release\Regions.dll"
copy "..\Regions\bin\Release\Regions.pdb" "Release\Regions.pdb"

copy "..\Languages\bin\Release\Languages.exe" "Release\Languages.exe"
copy "..\Languages\bin\Release\Languages.pdb" "Release\Languages.pdb"

cd "Release\"

:Update exe to mdb
C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Terraria_Server.exe"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Regions.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "RestrictPlugin.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "TDSMPermissions.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "TDSM_PermissionsX.dll"

cd ..
:PAUSE