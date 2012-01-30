:Delete files
DEL "Terraria_Server.exe"
DEL "Terraria_Server.pdb"
DEL "Terraria_Server.mdb"

:Copy new versions
copy "..\Terraria_Server\bin\Debug\Terraria_Server.exe" "Terraria_Server.exe"
copy "..\Terraria_Server\bin\Debug\Terraria_Server.pdb" "Terraria_Server.pdb"

:Update exe to mdb
C:/PROGRA~1/MONO-2~1.8/bin/mono C:/PROGRA~1/MONO-2~1.8/lib/mono/4.0/pdb2mdb.exe "Terraria_Server.exe"

PAUSE