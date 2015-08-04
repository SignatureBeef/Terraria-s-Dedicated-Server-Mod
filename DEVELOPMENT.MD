##Plugin Development
Plugins are supported via the tdsm-api project. This DLL is hooked into the vanilla server using the tdsm-patcher project. However you must reference both the TDSM.API.dll as well as the patched tdsm.exe.

The Plugin API is essentially the same as the old TDSM, so you can go about creating your plugin as you normally would.
In addition to the old .NET plugins we now also support LUA. They function in the same manner as if it was a standard .NET plugin. I do suggest that if you need a high performance plugin/event that you use .NET.
<br/>
<br/>
This time TDSM is in fact a plugin to this API, and is known as the "core". So in reality any developer may come along and write another server mod for say their client mod (it's also possible for the patcher to patch the client executable to run using MonoGame).
<br/>
<br/>
Intereracting with TDSM's core dll is as easy as simply creating the reference to it, and then using it's exposed methods.
<br/>
<br/>
The wiki and API documentation will be created very soon, and as a priority.

##Core Development
There are three core components of a TDSM server.
<br/>
 1. The API surrounding the official server code (TDSM.API.dll)
 2. The TDSM patcher that hooks the API into the official code (tdsm.patcher.exe)
 3. The TDSM core plugin that consumes the API and provides the additional features (TDSM.Core.dll)

######Why do we not use our existing code base?
For the core developers updating our previous codebase to match the official code each update takes by far too long and to be brutally honest, it is mind numbing.
The new patching approach is mostly dynamic and where it's not will only need adjusting (apart from where Re-Logic removes functonality). This allows for updates measured in hours rather than weeks, mainly consising of analysis of changes and packet updates (core plugin).
<br/>
<br/>
Note: The API is not for new functionality, rather it is for exposing the official server events to be processed by a plugin whom provides functionality.

##Compiling the solution
First open the correct solution file, tdsm.sln for Visual Studio or tdsm-md.sln for MonoDevelop and ensure that the Debug x86 platform is selected.


1. Build the tdsm-api without the API reference (see defined symbol Full_API in the project properties/options compiler settings for tdsm-api) 
2. Then build and run tdsm-patcher (in Visual Studio right click the tdsm-patcher project: Debug -> Start New Instance). Do not start the server at this stage.
3. By default the tdsm solution already is referencing the generated server exe, but if not search the tdsm-patcher\bin\x86\Debug directory for the generated executable and add it as a reference.
4. Readd Full_API to the tdsm-api project and rebuild it.
5. Rebuild the entire solution so all projects are now using the latest api and server executable
6. You are ready to run the patcher again. This time the generated tdsm.[platform].exe is the executable you can use to run a server.

##API Development
Currently essential hooks are being implemented. However should you need a missing hook simply request it or you can submit a pull request.
