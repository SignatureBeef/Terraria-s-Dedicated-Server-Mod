#Running TDSM Rebind:

##Linux (Ubuntu):
Install prerequisites
apt-get install mono-complete

Running
mono --runtime=v4.0 tdsm.exe

NOTE:
	You need a 4.0 compatible run time.
	If you get mscorlib errors then this is a sure sign.
	Otherwise, "--runtime=4.0" is optional

	
##Plugin Development
Plugins are supported via the tdsm.api.dll. This DLL will allow you to hook directly into official server code.
The API is essentially the same as the old TDSM, so you can go about creating your plugin as you normally would.
In addition to the old .NET plugins we now also support LUA. They function in the same manner as if it was a standard .NET plugin. I do suggest that if you need a high performance plugin/event that you use .NET.

This time TDSM's core is in fact a plugin to this API and any developer may come a long and write another server mod for say their client mod (it's also possible for the patcher to patch the client executable, though implementation upon request).

Intereracting with TDSM's core dll is as easy as simply creating the reference to it, and then using it's exposed methods.

The wiki and API documentation will be created very soon, and as a priority.

##Core Development
There are three core components of a TDSM server.
1. The API surrounding the official server code
2. The TDSM patcher that hooks the API into the official code
3. The TDSM core plugin that consumes the API and provides the additional features

File names are in accordance to above:
* tdsm.api.dll
* tdsm.patcher.exe
* tdsm.core.dll

Why do we not use our existing code base?
For the core developers updating our previous codebase to match the official code each update takes by far too long and to be brutally honest, it is mind numbing.
The new patching approach is mostly dynamic and where it's not will only need adjusting (apart from where Re-Logic removes functonality). This allows for updates measured in hours rather than weeks, mainly consising of analysis of changes and packet updates (core plugin).

Note: The API is not for new functionality, rather it is for exposing the official server events to be processed by a plugin whom provides functionality.

##Compiling the solution
Build the tdsm-api without the API reference (see defined symbol Full_API in the project properties) and run it beside the official TerrariaServer.exe. 
It will output tdsm.exe, you now will need to reference tdsm.exe in the tdsm-api project then redefine Full_API and rebuild the solution/project in order to expose all of the (Cecil altered) API to the api and core dll.

Please note you will also require the latest Mono.Cecil and NLua libraries.

##API Development
Currently essential hooks are being implemented. However should you need a missing hook simply request it or you can submit a pull request.

##Development Status
Extreme beta - handle with care.
Until Terraria is compatible on mono the TDSM Core is essential for Linux compatability. If you do not require mono you can actually run the server without the core DLL
and it will function as a vanilla server with plugin support. We do plan for the core to be disabled and still run a mono compatible server.
