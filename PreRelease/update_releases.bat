:Delete files
DEL "Terraria_Server.exe"
DEL "Terraria_Server.pdb"
DEL "Terraria_Server.mdb"

DEL "Restrict.dll"
DEL "Restrict.pdb"
DEL "Restrict.mdb"

DEL "TDSMPermissions.dll"
DEL "TDSMPermissions.pdb"
DEL "TDSMPermissions.mdb"

DEL "Regions.dll"
DEL "Regions.pdb"
DEL "Regions.mdb"

:Copy new versions
copy "..\Terraria_Server\bin\Release\Terraria_Server.exe" "Terraria_Server.exe"
copy "..\Terraria_Server\bin\Release\Terraria_Server.pdb" "Terraria_Server.pdb"

copy "..\Restrict\bin\Release\RestrictPlugin.dll" "RestrictPlugin.dll"
copy "..\Restrict\bin\Release\RestrictPlugin.pdb" "RestrictPlugin.pdb"

copy "..\TDSMPermissions\TDSMPermissions\bin\Release\TDSMPermissions.dll" "TDSMPermissions.dll"
copy "..\TDSMPermissions\TDSMPermissions\bin\Release\TDSMPermissions.pdb" "TDSMPermissions.pdb"

copy "..\Regions\bin\Release\Regions.dll" "Regions.dll"
copy "..\Regions\bin\Release\Regions.pdb" "Regions.pdb"

:Update exe to mdb
C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Terraria_Server.exe"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Regions.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "RestrictPlugin.dll"

C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "TDSMPermissions.dll"

:PAUSE