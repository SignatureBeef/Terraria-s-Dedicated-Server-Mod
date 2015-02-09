Running TDSM Rebind:
=============

Linux (Ubuntu):
-------------
	Install prerequisites
	apt-get install mono-complete


	Running
	sudo mono --runtime=v4.0 tdsm.exe

	NOTE: You need a 4.0 compatible run time. If you get mscorlib errors then this is a sure sign. Otherwise, "--runtime=4.0" is optional

	
Plugin Development
=============
	Plugins are supported via the tdsm.api.dll. This DLL will allow you to hook directly into official server code.
	The API is essentially the same as the old TDSM, so you can go about creating your plugin as you normally would.
	In addition to the old .NET plugins we now also support LUA. They function in the same manner as if it was a standard .NET plugin. I do suggest that if you need a high performance plugin/event that you use .NET.

	This time TDSM's core is in fact a plugin to this API and any developer may come a long and write another server mod for say
	their client mod (it's also possible for the patcher to patch the client executable, though implementation upon request).

	Intereracting with TDSM's core dll is as easy as simply creating the reference to it, and then using it's exposed 
	methods.

	The wiki and API documentation will be created very soon, and as a priority.

Core Development
=============
	There are three core components of a TDSM server.
		1) The API surrounding the official server code
		2) The TDSM patcher that hooks the API into the official code
		3) The TDSM core plugin that consumes the API and provides the additional features

	File names are in accordance to above:
		tdsm.api.dll
		tdsm.patcher.exe
		tdsm.core.dll

	Why do we not use our existing code base?
	For the core developers updating our previous codebase to match the official code each update takes by far too long and to be brutally honest, it is mind numbing.
	The new patching approach is mostly dynamic and where it's not will only need adjusting (apart from where Re-Logic removes functonality). This allows for 
	updates measured in hours rather than weeks, mainly consising of analysis of changes and packet updates (core plugin).

	Note: The API is not for new functionality, rather it is for exposing the official server events to be processed by a plugin whom provides functionality.

Compiling the solution
=============
	Build the tdsm-api without the API reference (see defined symbol Full_API in the project properties) and run it beside 
	the official TerrariaServer.exe.
	It will output tdsm.exe, you now will need to reference tdsm.exe in the tdsm-api project then redefine Full_API and
	rebuild the solution/project in order to expose all of the (Cecil altered) API to the api and core dll.

	Please note you will also require the latest Mono.Cecil and NLua libraries.

API Development
=============
	Currently essential hooks are being implemented. However should you need a missing hook simply request it or you can submit
	a pull request.

Development Status
=============
	Extreme beta - handle with care.
	Until Terraria is compatible on mono the TDSM Core is essential for Linux compatability. If you do not require mono you can actually run the server without the core DLL
	and it will function as a vanilla server with plugin support. We do plan for the core to be disabled and still run a mono compatible server.

Road map:
=============
		[Achievement]												[Status]							[Priority]
		Command line hook											Completed							-
		Player chat hook											Not started							Low
		Mono compatability											Completed							-
		Plugin system												Completed							-
			- LUA													Completed							-
			- Cecil events
				- On custom patch									Completed							-
				- Cancel default patch								Not started							Low
		Tile memory usage											WIP									Low
		Remove netMode = 1 code blocks								WIP									Low
		XNA replacement												Completed							-
		Implement the slot server for mono until official			[Verifying packets]					Done
		supports it
		Defer from using the official world selection, using		Not started							Medium
		what previously had with the properties file				
		Import the old RCON server									Not started							Low
		Make the tdsm.api.dll the API								Completed							-
			- TDSM will be a plugin
		Create the TDSM plugin										Completed							-
			- Add a web server (configurable, on by default)		Not started							Low
			- Create a world viewer as the default web page			Not started							Low
				- Allow admins to log in, and they can edit			Not started							Low
				  blocks, as well as view tile history
				  (players whom changed)
				  [This may be an external plugin requiring SQL]
		Refactor pretty much each project correctly					Finalisation						Low
		Add a hook to initialise, in order to create the plugin		Completed							-
		directory and properties file						
		Bring back all the old hooks where possible					WIP									Low
		Hook [Console.WriteLine], and by default actually use it	Not started							Low
		Rename packet message classes with a prefix of the			Completed							-
		packet number
			e.g. 1_ConnectionRequest.cs								
		Correct packet names [Packet.cs]							Not started							Low
		Import the old commands										Not started							Medium
		Cleanup the dummy XNA										Not started							Low
		Split up NewNetMessage packets into seperate files			Not started							Low
		as like the outbound.										
		Verify each packet is working the way it should				Not started							High
		Allow the core to be disabled but still run a linux			Not started							High
		compatable server
		Allow persistent plugin states.								Not started							High
			ie If core is disabled then ensure it's still
				disabled next startup until reenabled.
		TDSM's properties file										Not started							High
		Make a plugin tester command for developers					Not started							Low
			e.g. plugin invoke <eventname>
		LUA command implementation									Completed							-
