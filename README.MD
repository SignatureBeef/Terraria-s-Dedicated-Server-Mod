# TDSM: Rebind [![Build Status](https://travis-ci.org/DeathCradle/Terraria-s-Dedicated-Server-Mod.svg?branch=master)](https://travis-ci.org/DeathCradle/Terraria-s-Dedicated-Server-Mod) [![Slack](https://img.shields.io/badge/Chat%20on-Slack-blue.svg)](http://tdsm.sithous.com/slack)
*Supporting Terraria v1.3!*  
TDSM is a server modification of the official Terraria dedicated server software. It extends functionallity giving the option for advanced permissions and plugin support. TDSM is a great choice for alternative server software for Terraria if you want stability, multi-platform, and added functionallity. It's also updated very quickly after a new Terraria release because of the way the new TDSM Rebind has been written.

#### Notable Features
* Plugins
* Permissions System
* MySQL and SQLite
* Additional Commands like; spawnnpc, spawnboss, hardmode, invasion (and custom ones too), ect...
* Web Interface (soon)
* Remote Console
* Memory optimisations

## Installing
1. You may get the latest release from the "[releases](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/releases)" link at the top or by clicking [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/releases). Want a the latest unstable build? Check [Testing](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod#testing) below.
2. Just extract the files somewhere and run tdsm.exe. :) (in linux run it with mono)  
Get more [Help](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/wiki/Installation-and-Running-The-Server) with installing.

#### Docker
Alternatively you can use docker to easily setup a pre-built vm running TDSM. [@parkervcp](https://github.com/parkervcp) has been kind enough to keep this up to date, and so you can use `parkervcp/tdsm:latest` to get the latest container. See [http://docs.docker.com/](http://docs.docker.com/) for the latest install instructions for docker.

Using a GUI is the easiest method for using docker, but for those of you who use it via command line; you can start the container with configuration like so `docker run -itd -p 7777:7777 -v /path/to/server.config:/srv/server.config parkervcp/tdsm:latest`

## Plugins
This will serve as a "known" plugin list until our website is complete. If you know of others or wish to add yours, fork and make a pull request. Or you could request on Slack.
- [Restrict](https://github.com/DeathCradle/TDSM_Restrict) - Player registration and world protection
- [SQLite](https://github.com/DeathCradle/tdsm-sqlite-connector) - Adds SQLite storage and permissions
- [MySQL](https://github.com/DeathCradle/tdsm-mysql-connector) - Adds MySQL storage and permissions
- [RenWorld](https://d1g1t4l3ch0.github.io/TDSM-RenWorld-Plugin/) - [Lua] Allows you to rename a world
- [Map](https://github.com/elevatorguy/TDSM_map) - Renders the loaded world as a PNG

## Testing
If you would like to help test and get the unstable development build, you can just grab all the files from the "[Binaries](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/tree/master/Binaries)" directory. Those are always updated with each commit. Please use the GitHub Issues section to report bugs. Of course you can always drop by #support in our Slack chat (read more below), but we ask that if it's an actual bug report, to please report it in GitHub so we can track it. It also serves as a reminder. You may also see the changes between builds and upcoming builds by checking out the  [CHANGELOG.MD](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/blob/master/CHANGELOG.MD) file.

## Getting Help
Need help? Sure, no problem. ;)
#### Wiki
There is a Wiki! Take a look at it from the Wiki link on the right side of the GitHub repo home page. Or click [here](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/wiki).
#### Live Chat [![Slack](https://img.shields.io/badge/Chat%20on-Slack-blue.svg)](http://tdsm.sithous.com/slack)
Want to chat with the core team members, other TDSM server operators like you, and plugin developers? Join our official [Slack](http://tdsm.sithous.com/slack) chat.

## Showcase Server
We have a showcase server running the latest release of TDSM (and maybe sometimes the latest build?)! Join us and play on it any time you want. We are glad to have you. Mainly it's for checking it out from the player's perspective and getting a feel for it, but you could play some too. Though at this time the world may not be persistant and there could be some bad bugs from time to time if we do update it to a dev build.

The VPS server we are hosting it on was provided to us by CreeperHost. We want to give a **big** shout out to CreeperHost for their generostity and awesome vps server. It really does perform well and we really like it. Thanks!  
**"Want it easy, get it set up for you at CreeperHost" - [CreeperHost](http://www.creeperhost.net/)**

#### Login Details
* IP   = **showcase.tdsm.org**
* PORT = **7777**

## Developers. We Need You!
If you can help with development, we would be happy to have your help! :) TDSM is written in C#. If you know other languages or would like to develop plugins instead, we need those too. Plugins can be written in C#, Lua, VB.NET, and F#. If you want to help with the main project, just create a pull request and we'll review it then merge it if all looks good. :)
#### Plugins Wanted
Here are a few Plugin ideas we would like to see. Of course you may make whatever kind of plugin you like.
* Anti-Cheat  
Different mechanisms for detecting cheating. One could be to detect post-hardmode items that exist in player inventory pre-hardmode and remove them, notify ops, or ban.
* Regions / Home Protection  
A block protection plugin for specific areas designated by a player.
* Sort Containers / Inventory
* PVP / Mini-game
* Rollback / Anit-Grief protection
* Player chat (Player prefixes/suffixes and colours)
* Connection/Chat history
* Configuration editor command

Also please take a look at our [DEVELOPMENT.MD](https://github.com/DeathCradle/Terraria-s-Dedicated-Server-Mod/blob/master/DEVELOPMENT.MD) file.
