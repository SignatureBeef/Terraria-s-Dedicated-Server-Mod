##Summary
TDSM Rebind for Terraira 1.2.4.1 replaces the old TDSM that we stopped supporting a long time ago. It is a completely new approach which does not require us to update all of our code base to match the latest Terraria; rather we patch into the official code.

Should you need help the wiki is currently in progress, but you still may view it right [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/wiki) on GitHub.

##Getting the new TDSM Rebind:
You can either compile from source or grab the latest release located [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/releases).

See [this wiki page](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/wiki/A-Beginner%27s-Guide-for-TDSM-Rebind%3A-Installation-and-Running) for how to run the new TDSM.
	
##Plugin Development
Plugins are supported via the tdsm-api project. This DLL is hooked into the vanilla server using the tdsm-patcher project. However you must reference both the tdsm.api.dll as well as the patched tdsm.exe.

The Plugin API is essentially the same as the old TDSM, so you can go about creating your plugin as you normally would.
In addition to the old .NET plugins we now also support LUA. They function in the same manner as if it was a standard .NET plugin. I do suggest that if you need a high performance plugin/event that you use .NET.
<br/>
<br/>
This time TDSM's is in fact a plugin to this API, and is known as the "core". So in fact any developer may come along and write another server mod for say their client mod (it's also possible for the patcher to patch the client executable, though implementation upon request).
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


Build the tdsm-api without the API reference (see defined symbol Full_API in the project properties/options compiler settings for tdsm-api) then build and run tdsm-patcher. 
It will output tdsm.exe, you now will need to reference the newly generated tdsm.exe in each project where the Terraria namespace is used. You must then redefine Full_API and rebuild the solution/project in order to expose all of the (Cecil altered) API to the rest of the projects.

You are then ready to run the patcher for one last time, this will output the tdsm.exe that you can run a server with.
<br/>
<br/>
Please note you will also require the latest Mono.Cecil and NLua libraries.

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
