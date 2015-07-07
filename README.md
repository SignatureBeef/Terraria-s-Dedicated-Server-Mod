# TDSM supports Terraria 1.3
Work is still in progress for mono compatibility, and there are still quirks in the windows version but you can run a 1.3 compatible server using TDSM.

Use the tdsm.exe in the Binaries folder.

##Summary [![Build Status](https://travis-ci.org/DeathCradle/Terraria-s-Dedicated-Server-Mod.svg?branch=master)](https://travis-ci.org/DeathCradle/Terraria-s-Dedicated-Server-Mod)
TDSM Rebind for Terraria 1.2.4.1 replaces the old TDSM that we stopped supporting a long time ago. It is a completely new approach which does not require us to update all of our code base to match the latest Terraria; rather we patch into the official code.

Should you need help the wiki is currently in progress, but you still may view it right [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/wiki) on GitHub.

## Live Chat
You can chat with the team live or among other TDSM users from the community live! All are welcome to join the chat, hang out, arrange games, ask questions, and chat with the team. Join us on [Slack](http://tdsm.sithous.com/slack).

##Getting the new TDSM Rebind:
You can either compile from source or grab the latest release located [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/releases).
Alternatively you can get one of the latest pre-release binaries [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/tree/master/Binaries).

See [this wiki page](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/wiki/Installation-and-Running-The-Server) for how to run the new TDSM.
	
##Plugin Development
Plugins are supported via the tdsm-api project. This DLL is hooked into the vanilla server using the tdsm-patcher project. However you must reference both the tdsm.api.dll as well as the patched tdsm.exe.

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
 1. The API surrounding the official server code (tdsm.api.dll)
 2. The TDSM patcher that hooks the API into the official code (tdsm.patcher.exe)
 3. The TDSM core plugin that consumes the API and provides the additional features (tdsm.core.dll)

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

##Development Status
Extreme beta - handle with care.
<br/>
<br/>
Until Terraria is compatible on mono the TDSM Core is essential for Linux compatability.
<br/>
If you do not require mono you can actually run the server without the core DLL and it will function as a vanilla server with plugin support.
<br/>
<br/>
We do plan for the core to be disabled and still run a mono compatible vanilla server.
