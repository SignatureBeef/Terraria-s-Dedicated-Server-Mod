CLS
::When i was debugging...
::COPY ..\..\Terraria-s-Dedicated-Server-Mod\tdsm-patcher\APIWrapper.cs .
::COPY ..\..\Terraria-s-Dedicated-Server-Mod\tdsm-patcher\Program.cs .

::Update DNU packages
CALL dnu restore

::Patch TDSM into Terraria
dnx . run -norun

::Now setup TDSM in a isolated area
MKDIR corenet
MKDIR corenet\Plugins
cd corenet

::Crucial that we have tdsm.exe. This is because both the file name and assembly name must match.
COPY ..\tdsm.microsoft.exe tdsm.exe
COPY ..\tdsm.api.dll .
COPY ..\KopiLua.dll .
COPY ..\NLua.dll .
COPY ..\Plugins\tdsm.core.dll Plugins\.

REM You now can run tdsm.exe using "dnx tdsm.exe"