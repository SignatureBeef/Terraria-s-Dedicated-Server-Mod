# TDSM Changelog

## Build 6 *(in progress)*
- Server side characters
- Upgrade to .NET 4.5
- `give` command: New syntax and optional player name argument.
- `give` command: Fixed not giving number of items asked for. Limit is 10 stacks of any item.
- Whitelist is back in, see command 'whitelist', and config property is now 'whitelist' instead of 'usewhitelist'
- OWIN based web server for files and a REST API
- Command line arguments can now be specified for TDSM commands, and supersede configuration properties.
- Fixed the heartbeat not disabling
- Fixed character data reading - other players now look like they are meant to.

## Build 5 - 1.3.0.8
- Added send data hook
- Missing shims for mono ahead of time compilation
- Wrong disconnectee fix
- Send net message hook
- Motd new line fix
- New command: fastforwardtime
- New command: heal

## Build 4 - 1.3.0.7
- Fixed vanilla Terraria permission nodes
- Fixed invasion warnings and fixed support for multiple npc's
- Fixed some cases where libraries weren't being loaded from the Libraries folder
- Reimplemented coloured console
- Added MDB files for debugging
- Fixed heartbeat code where the build was more than expected
- Added SQLite dependency for windows machines
- Fixed assembly resolution issues. tdsm.api.dll is now TDSM.API.dll, and tdsm.core.dll is TDSM.Core.dll

## Build 3 - 1.3.0.7
- Official support for Terraria 1.3.0.7
- Brand new Data API (SQLite & MySQL connectors provided - MSSQL soon)
- Brand new Permissions system
- New commands such as auth, user, group
- All invasions are now up to date
- Spawnboss now covers all bosses
- All latest world events
- Custom invasions
- Auto server list publishing to the TDSM website (upcoming)
- Many stability fixes
- Fixed LUA issues when using AccessLevel
- Fixed RCON invalid user exception
- Op system now utilises the Data API when possible
- Fixed problems when running TDSM from another directory
- Give command now supports prefixes
- Exit crash fixes (a couple of these)
- Mimic names corrected
- Plugins now disable on exit
- The default World path for TDSM is now only created when required
- TDSM config files can now read space-seperated keys
- Ghost crash fix
- API can now handle passwords (allow server and user passwords)
- World removal fix
- Mono client connection code does not require a custom server (this may however be reintroduced)
- Exit command now saves before exiting
- Namespacing change (Plugin developers please adjust - this was done sooner than later)
- NPC and item definition updates
- Added an F# plugin
- No need to two versions of TDSM (mono/windows) anymore
- Reintroduced the Libraries folder for organisation
- Fixed the help command not showing the commands where the plugin name was the same
- PID file reintroduced
- Heartbeat to the TDSM master server
