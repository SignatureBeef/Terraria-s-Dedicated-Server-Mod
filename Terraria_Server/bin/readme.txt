Install .NET Framework 4 and XNA Redistributable 4.

This will work on a VPS or dedicated server. It probably works on wine as well. I haven't tested that.

Please don't use this if you don't own Terraria on Steam. You can't play the game without it but I don't really want
people who don't own the game running servers, since that means they're using a cracked version.

Yes I do realise how lightly I'm using the term cracked here >_>

Also, I do realise how stupid the homes and warps folders are... it's pointless. I only did it because I was tired
and trying to spew that out quickly, I wasn't planning on releasing it and right now I just CBF to move it so... yeah.

Hook list: TILE_CHANGE, PLAYER_COMMAND, PLAYER_CHAT, SERVER_UPDATE, PLAYER_JOIN, NPC_SPAWN, PLAYER_SPAWN, ITEM_SPAWN


***
advertEnabled property. Please keep this set to true. You don't have to but it'd be quite nice if you did.

It just enables a message when people join saying "Server powered by tMod ~ tmod.us". Nothing else.

Feel free to disable it, but like I said, I'd prefer it if you keep it enabled.
***

DO NOT DELETE tmod.resource AS IT IS REQUIRED BY THE SERVER. THIS VERSION OF THE SERVER CAN NOT GENERATE MAPS.

PLEASE PUT YOUR MAP FILE FROM SINGLEPLAYER IN THE SAME DIRECTORY AS TMOD'S TERRARIA.EXE!
Just upload it to your VPS. It must be the first world. I'll fix this in the next version.

How to configure:

To activate whitelist change the whitelistEnabled property to true in server.properties

To add an IP to the whitelist in-game type /whitelist add {ip-address}
To remove an IP from the whitelist in-game type /whitelist remove {ip-address}
To add an IP OUTSIDE of the game (only do this when you first add the whitelist so you can join!) open
whitelist.txt and add the IP at the bottom of the file, with a newline.

Changelog:

007:
Added server properties: spawnRate, maxSpawns (both control spawn rate of NPCs, defaults 700 and 4)
Added hook: ITEM_SPAWN
Added ability to type /save or /quit in the console. /quit gives you a safe shutdown (saves the map)
Added commands: /spawnrate /maxspawns (to temporarily change the spawn rate in-game)
Fixed /time bugs
Fixed /give not showing item name

006:
Fixed PLAYER_CHAT glitch
Worked around plugin loading crash if property not set
Added hooks: SERVER_UPDATE, PLAYER_JOIN, NPC_SPAWN, PLAYER_SPAWN
Added Player.kick(string) method

005:
Fixed CPU usage issues (some my fault, some Terraria's fault)
Added spawnMonsters and pvp properties (pvp forces pvp, however the client has to enable
it to attack others, but not to be attacked - blame Terraria)
Fixed TILE_CHANGE setState
Added Hook: PLAYER_CHAT
Allowed plugins access to class: WorldGen (was defined as internal, preventing access)
Fixed glitch with /give where items with "of" in the middle wouldn't spawn
Fixed glitch where /give didn't show item name
More... it's 03:20am, I've probably forgotten a lot...

004:
Semi-fixed server Update() calls (causes jitteryness)
Fixed /give glitch
Added maxPlayers cap of 8
Added whitelist
Added whitelistEnabled, worldName, advertEnabled server properties
Added /whitelist command
Added ability to type in console and have it display as chat in-game. Will use for commands later, and make it look better.
ADDED PLUGINS! WOO!

003:
Fixed /home and /setwarp
Added server properties: maxPlayers, serverPassword
Added annoying beeps for problems in the files
Added warning message for default op password
Added /time command
Various other bugfixes

002:
Fixed various command bugs. I realised some things.
Added welcomeMessage property (to be fair, this was a bugfix in 001 but I may as well formalise it here)

001:
Initial release