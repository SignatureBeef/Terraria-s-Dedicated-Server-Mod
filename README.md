Terraria's Dedicated Server Mod
-------------

Aims to provide a stable Server + API.

If you like TDSM or want to help support us, we simply ask you to tell your friends about it, and help spread the word!.

* Jump on over to the Forums at: http://www.tdsm.org/
* Dev's are more than welcome to post their commits to my page, I'm all for it. 


TDSM IS NOT tMod. TDSM was actually under development before tMod was released.

-
Apparently Asyncronous Sockets are not acheived in Mono yet, I was aware of this, But not as much as I do now.
I cannot say how long or when Mono will support it.
-


Status
-------------
TDSM is currently runnable as a server, You may also find that Developers can also now make plugins for it if they feel so.

How To Use
-------------
To use TDSM place the executable in the location you wish, Then run it. It will generate a properties file & start generating a world!
This properties file contains server properties such as Max Players, Greeting (MOTD) and the port to use.

TDSM has included the use for Operators. To use this feature simply set a password in the server properties and when you log into the server, Enter your password and you ill be set to an OP. This OP can do commands such as /exit & /reload etc.

TODO
-------------
* Yet to clean up (Minor)
* Yet to removed un-needed code for client purpose. (Major)
* Yet to test on other OS's
* Yet to add more plugin hooks! (Possibly wanting to organise current Events)
* Yet to rememeber what else!

Developers
=============
TDSM is developed in C# so you will find it reasonably easy to manipulate. Also if ever have done Bukkit (Minecraft) Programming you should see some things are similar when programming a plugin.
Plugins HAVE to be developed on the .Net 3.5 framework (In Visual Studio you have multiple choices, Choose 3.5) otherwise TDSM will not be able to load your plugin!. And until we add a method for dynamic loading your plugin name will need to be the same as the namespace and class.

On a side note, 
I would love for you to assist in the Development TDSM, you may fully well do so; It's why there is a GitHub :D


