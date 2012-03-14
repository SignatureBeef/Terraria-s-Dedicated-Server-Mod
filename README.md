Terraria's Dedicated Server Mod
-------------

Aims to provide a stable Server + API which is compatible with Mono while also being 100% free of charge. However if you wish to donate, please checkout the donation options over at http://tdsm.org

(Donating will encourage us to work harder, Because at the moment we are doing this out of shear good will for people in a predicament when we originally left.)

If you like TDSM or want to help support us, we simply ask you to tell your friends about it, and help spread the word!.

* Jump on over to the Forums at: http://www.tdsm.org/
* Dev's are more than welcome to post their commits to my page, I'm all for it. 


TDSM IS NOT tMod nor is it TShock. TDSM was actually under development before tMod was released.

Status
-------------
Currently it is in a PreRelease state until further notice.

Features
-------------
Oh god. There is too many to tell, Such things as 

	- Memory/CPU Optimizations
	
	- No Client Code (Runs a hell of a lot faster...)
	
	- Permissions System

	- Hack Preventions
	
	- [WIP] Player Sandboxing! (This keeps track of player world alterations!)
	
	- [WIP] Language System
	
	
These are just only a few, But there is a larger list located at http://tdsm.org


How To Use
-------------
To use TDSM place the executable in the location you wish, Then run it. It will generate a properties file (You may opt to exit for editing) & start generating a world!
This properties file contains server properties such as Max Players, Greeting (MOTD) and the port to use.

TDSM has included the use for Operators. To use this feature simply set a password in the server properties and when you log into the server, enter your password and you will be given OP status. This allows you to use commands such as /exit & /reload etc.

The server also allows command line arguments, We will be adding documentation to the Wiki whenever we can.

Contact
-------------
We have IRC channels on esper, #TDSM, #TDSMDev, #TDSMGit or you can get us over at http://tdsm.org.

Developers
=============
TDSM is developed in C# so you will find it reasonably easy to manipulate.
Plugins HAVE to be developed on the .Net 4.0 framework (In Visual Studio you have multiple choices, Choose 4.0) otherwise TDSM will not be able to load your plugin!.

Usually when the TDSM Team has an upcoming release which changes a lot of code, We will post a prerelease for Developers to update prior to release; Located in the PreRelease directory on this GitHub page.

On a side note, 
I would love for you to assist in the Development TDSM, you may fully well do so; It's why there is a GitHub :D


