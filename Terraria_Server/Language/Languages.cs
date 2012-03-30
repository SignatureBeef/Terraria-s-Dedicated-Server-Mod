using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using Terraria_Server.Logging;

namespace Terraria_Server.Language
{
	public static class Languages
	{
#region Plugins
		public static Dictionary<String, String> ExtendedLanguages;
#endregion
#region Properties
		public static String Disconnected { get; set; }
		public static String PermissionsError { get; set; }
		public static String ExitRequestCommand { get; set; }
		public static String ExpiredCommandMessage { get; set; }
		public static String XRequestedShutdown { get; set; }
		public static String PropertiesReload { get; set; }
		public static String CurrentPlayers { get; set; }
		public static String NoPlayers { get; set; }
		public static String SavingWorld { get; set; }
		public static String SavingData { get; set; }
		public static String SavingComplete { get; set; }
		public static String InvalidPage { get; set; }
		public static String Add { get; set; }
		public static String Added { get; set; }
		public static String Remove { get; set; }
		public static String RemovedFrom { get; set; }
		public static String PleaseReview { get; set; }
		public static String WhilelistFailedSave { get; set; }
		public static String OPlistFailedSave { get; set; }
		public static String Command { get; set; }
		public static String IPExpected { get; set; }
		public static String TimeSet { get; set; }
		public static String CurrentTime { get; set; }
		public static String BossNotSpecified { get; set; }
		public static String BossSummonedBy { get; set; }
		public static String Clear { get; set; }
		public static String NoItemIDNameProvided { get; set; }
		public static String ExplosionsAreNow { get; set; }
		public static String ThisIsPlayerCommand { get; set; }
		public static String CannotQuestGiverWithoutTDCM { get; set; }
		public static String QuestGiverAlreadySpawned { get; set; }
		public static String YouMustWaitBeforeAnotherCommand { get; set; }
		public static String MoreThanOneItemFoundNameId { get; set; }
		public static String MoreThanOneItemFoundType { get; set; }
		public static String TooManyArguments { get; set; }
		public static String NobodyOnline { get; set; }
		public static String NPCDoesntExist { get; set; }
		public static String DontSpawnThatMany { get; set; }
		public static String ExpectedSpawnInteger { get; set; }
		public static String NeedTeleportTarget { get; set; }
		public static String TeleportedToSpawn { get; set; }
		public static String TeleportFailed { get; set; }
		public static String PlayerNotFound { get; set; }
		public static String InvalidCoords { get; set; }
		public static String CouldNotFindPlayer { get; set; }
		public static String OnlyPlayerCanUseCommand { get; set; }
		public static String SettlingLiquids { get; set; }
		public static String Complete { get; set; }
		public static String LiquidsAlreadySettled { get; set; }
		public static String YouAreNowOP { get; set; }
		public static String YouAreNowDeop { get; set; }
		public static String SuccessfullyLoggedInOP { get; set; }
		public static String FailedLoginWrongPassword { get; set; }
		public static String IncorrectOPPassword { get; set; }
		public static String YouNeedPrivileges { get; set; }
		public static String SuccessfullyLoggedOutOP { get; set; }
		public static String NPCSpawningIsNow { get; set; }
		public static String YouHaveBeenKickedBy { get; set; }
		public static String HasBeenKickedBy { get; set; }
		public static String KickSlotIsEmpty { get; set; }
		public static String KickPlayerNameNull { get; set; }
		public static String RestartingServer { get; set; }
		public static String StartingServer { get; set; }
		public static String PurgingProjectiles { get; set; }
		public static String PurgingNPC { get; set; }
		public static String PurgingItems { get; set; }
		public static String LoadedPlugins { get; set; }
		public static String NoPluginsLoaded { get; set; }
		public static String IssueFindingPlayer { get; set; }
		public static String NoOnlinePlayersToSpawnNear { get; set; }
		public static String NeedsToBeNightTime { get; set; }
		public static String HardModeAlreadyEnabled { get; set; }
		public static String StartingHardMode { get; set; }
		public static String RPGMode_Allowed { get; set; }
		public static String RPGMode_Refused { get; set; }
		public static String ItemRejection_Added { get; set; }
		public static String ItemRejection_ItemExists { get; set; }
		public static String ItemRejection_Removed { get; set; }
		public static String ItemRejection_ItemDoesntExist { get; set; }
		public static String ItemRejection_Cleared { get; set; }
		public static String Ban_You { get; set; }
		public static String Ban_Banned { get; set; }
		public static String Ban_FailedToSave { get; set; }
		public static String Ban_UnBanned { get; set; }
		public static String CommandDescription_Exit { get; set; }
		public static String CommandDescription_SaveAll { get; set; }
		public static String CommandDescription_ReloadPlugins { get; set; }
		public static String CommandDescription_OnlinePlayers { get; set; }
		public static String CommandDescription_Me { get; set; }
		public static String CommandDescription_Say { get; set; }
		public static String CommandDescription_Slots { get; set; }
		public static String CommandDescription_Kick { get; set; }
		public static String CommandDescription_Ban { get; set; }
		public static String CommandDescription_UnBan { get; set; }
		public static String CommandDescription_Whitelist { get; set; }
		public static String CommandDescription_Rcon { get; set; }
		public static String CommandDescription_Status { get; set; }
		public static String CommandDescription_Time { get; set; }
		public static String CommandDescription_Help { get; set; }
		public static String CommandDescription_Give { get; set; }
		public static String CommandDescription_SpawnNpc { get; set; }
		public static String CommandDescription_Teleport { get; set; }
		public static String CommandDescription_TeleportHere { get; set; }
		public static String CommandDescription_SettleLiquids { get; set; }
		public static String CommandDescription_Op { get; set; }
		public static String CommandDescription_DeOp { get; set; }
		public static String CommandDescription_OpLogin { get; set; }
		public static String CommandDescription_OpLogout { get; set; }
		public static String CommandDescription_NpcSpawns { get; set; }
		public static String CommandDescription_Restart { get; set; }
		public static String CommandDescription_Purge { get; set; }
		public static String CommandDescription_Plugins { get; set; }
		public static String CommandDescription_Plugin { get; set; }
		public static String CommandDescription_SpawnBoss { get; set; }
		public static String CommandDescription_ItemRej { get; set; }
		public static String CommandDescription_Explosions { get; set; }
		public static String CommandDescription_MaxPlayers { get; set; }
		public static String CommandDescription_Q { get; set; }
		public static String CommandDescription_Refresh { get; set; }
		public static String CommandDescription_HardMode { get; set; }
		public static String CommandDescription_LanguageReload { get; set; }
		public static String CommandDescription_Backups { get; set; }
		public static String Generation_Terrain { get; set; }
		public static String Generation_AddingNPCs { get; set; }
		public static String Generation_AddingSand { get; set; }
		public static String Generation_GeneratingHills { get; set; }
		public static String Generation_AddingSnow { get; set; }
		public static String Generation_PuttingDirtBehindDirt { get; set; }
		public static String Generation_PlacingRocks { get; set; }
		public static String Generation_PlaceDirtWithinRocks { get; set; }
		public static String Generation_AddingClay { get; set; }
		public static String Generation_MakingRandomHoles { get; set; }
		public static String Generation_GeneratingSmallCaves { get; set; }
		public static String Generation_GeneratingLargeCaves { get; set; }
		public static String Generation_GeneratingSurfaceCaves { get; set; }
		public static String Generation_GeneratingJungle { get; set; }
		public static String Generation_GeneratingFloatingIslands { get; set; }
		public static String Generation_AddingMushroomPatches { get; set; }
		public static String Generation_PlacingMudInDirt { get; set; }
		public static String Generation_AddingShinies { get; set; }
		public static String Generation_AddingWebs { get; set; }
		public static String Generation_CreatingTheUnderworld { get; set; }
		public static String Generation_AddingWaterBodies { get; set; }
		public static String Generation_MakingTheWorldEvil { get; set; }
		public static String Generation_GeneratingMoutainCaves { get; set; }
		public static String Generation_CreatingBeachesOnEitherSide { get; set; }
		public static String Generation_AddingGems { get; set; }
		public static String Generation_GravitatingSand { get; set; }
		public static String Generation_CleaningUpDirtBackgrounds { get; set; }
		public static String Generation_PlacingAltars { get; set; }
		public static String Generation_SettlingLiquids { get; set; }
		public static String Generation_PlacingLifeCrystals { get; set; }
		public static String Generation_HidingTreasure { get; set; }
		public static String Generation_HidingMoreTreasure { get; set; }
		public static String Generation_HidingJungleTreasure { get; set; }
		public static String Generation_HidingWaterTreasure { get; set; }
		public static String Generation_AddingIslandHouses { get; set; }
		public static String Generation_PlacingBreakables { get; set; }
		public static String Generation_PlacingHellForges { get; set; }
		public static String Generation_SpreadingGrass { get; set; }
		public static String Generation_GrowingCacti { get; set; }
		public static String Generation_PlantingSunflowers { get; set; }
		public static String Generation_PlantingTrees { get; set; }
		public static String Generation_PlantingHerbs { get; set; }
		public static String Generation_PlantingWeeds { get; set; }
		public static String Generation_GrowingVines { get; set; }
		public static String Generation_PlantingFlowers { get; set; }
		public static String Generation_PlantingMushrooms { get; set; }
		public static String Generation_CreatingDungeons { get; set; }
		public static String Generation_PlacingTraps { get; set; }
		public static String Startup_Initializing { get; set; }
		public static String Startup_SettingUpPaths { get; set; }
		public static String Startup_SettingUpProperties { get; set; }
		public static String Startup_NoPropertiesFileFound { get; set; }
		public static String Startup_PropertiesCreationComplete { get; set; }
		public static String Startup_IssueDeletingPID { get; set; }
		public static String Startup_PressAnyKeyToExit { get; set; }
		public static String Startup_IssueCreatingPID { get; set; }
		public static String Startup_PIDCreated { get; set; }
		public static String Startup_RestartingIntoNewUpdate { get; set; }
		public static String Startup_ErrorUpdating { get; set; }
		public static String Startup_StartingRCON { get; set; }
		public static String Startup_StartingPermissions { get; set; }
		public static String Startup_PreparingServerData { get; set; }
		public static String Startup_LoadingItemDefinitions { get; set; }
		public static String Startup_LoadingNPCDefinitions { get; set; }
		public static String Startup_LoadingProjectileDefinitions { get; set; }
		public static String Startup_LanguageFileOOD { get; set; }
		public static String Startup_LanguageFileUpdate { get; set; }
		public static String Startup_LoadingPlugins { get; set; }
		public static String Startup_PluginsLoaded { get; set; }
		public static String Startup_GeneratingWorld { get; set; }
		public static String Startup_GeneratedSeed { get; set; }
		public static String Startup_WorldSizingError { get; set; }
		public static String Startup_GeneratingWithCustomSize { get; set; }
		public static String Startup_StartingTheServer { get; set; }
		public static String Startup_YouCanNowInsertCommands { get; set; }
		public static String Startup_IssueParsingConsoleCommand { get; set; }
		public static String Startup_Exiting { get; set; }
		public static String Startup_CrashlogGeneratedBy { get; set; }
		public static String Startup_ProgramCrash { get; set; }
		public static String Startup_PleaseSend { get; set; }
		public static String Startup_LogEnd { get; set; }
		public static String CreatingTileArrayOf { get; set; }
		public static String InitializingSlotManagerFor { get; set; }
		public static String Players { get; set; }
		public static String ServerStartedOn { get; set; }
		public static String Water_WaterHasBeenSettled { get; set; }
		public static String Water_PerformingWaterCheck { get; set; }
		public static String Water_PreparingLiquids { get; set; }
		public static String CheckingTileAlignment { get; set; }
		public static String LoadingWorldTiles { get; set; }
		public static String ResettingGameObjects { get; set; }
		public static String SavedFile { get; set; }
		public static String SavedTo { get; set; }
		public static String Failed { get; set; }
		
		static Languages()
		{
			Disconnected = "You have been Kicked from this Server.";
			PermissionsError = "You cannot perform that action.";
			ExitRequestCommand = "Exiting on request.";
			ExpiredCommandMessage = "This command is no longer available";
			XRequestedShutdown = " requested that TDSM is to shutdown.";
			PropertiesReload = "Reloading server.properties.";
			CurrentPlayers = "Current players: ";
			NoPlayers = "No players online.";
			SavingWorld = "Saving world.....";
			SavingData = "Saving data.....";
			SavingComplete = "Saving Complete.";
			InvalidPage = "Invalid page! Use";
			Add = "-add";
			Added = "added to";
			Remove = "-remove";
			RemovedFrom = "removed from";
			PleaseReview = "Please review that command";
			WhilelistFailedSave = "WhiteList Failed to Save due to ";
			OPlistFailedSave = "OpList Failed to Save due to ";
			Command = "command";
			IPExpected = "A player or IP was expected.";
			TimeSet = "Time set to ";
			CurrentTime = "Current Time";
			BossNotSpecified = "You have not specified a Boss.";
			BossSummonedBy = " has been been summoned by ";
			Clear = "-clear";
			NoItemIDNameProvided = "No item/id provided with your command";
			ExplosionsAreNow = "Explosions are now ";
			ThisIsPlayerCommand = "This is a player command.";
			CannotQuestGiverWithoutTDCM = "You cannot spawn the Quest Giver without allowing TDCM Clients!";
			QuestGiverAlreadySpawned = "The Quest Giver is already spawned!";
			YouMustWaitBeforeAnotherCommand = "You must wait {0:0} more seconds before using this command.";
			MoreThanOneItemFoundNameId = "There were {0} Items found regarding the specified name/id";
			MoreThanOneItemFoundType = "There were {0} Items found regarding the specified Type Id";
			TooManyArguments = "Too many arguments. NPC and player names with spaces require quotes.";
			NobodyOnline = "There is nobody online.";
			NPCDoesntExist = "Specified NPC does not exist";
			DontSpawnThatMany = "Don't spawn that many.";
			ExpectedSpawnInteger = "Expected integer for number to spawn.";
			NeedTeleportTarget = "Need specify to who to teleport.";
			TeleportedToSpawn = "Teleported {0} to spawn.";
			TeleportFailed = "Teleportation failed.";
			PlayerNotFound = "Target player not found.";
			InvalidCoords = "Invalid coordinates.";
			CouldNotFindPlayer = "Could not find that Player on the Server.";
			OnlyPlayerCanUseCommand = "Only a player can call this command!";
			SettlingLiquids = "Settling Liquids...";
			Complete = "Complete";
			LiquidsAlreadySettled = "Liquids are already settling";
			YouAreNowOP = "You are now OP!";
			YouAreNowDeop = "You have been De-Opped!";
			SuccessfullyLoggedInOP = "Successfully Logged in as OP.";
			FailedLoginWrongPassword = "Failed to log in as OP due to incorrect password.";
			IncorrectOPPassword = "Incorrect OP Password.";
			YouNeedPrivileges = "You need to be Assigned OP Privledges.";
			SuccessfullyLoggedOutOP = "Successfully Logged Out.";
			NPCSpawningIsNow = "NPC Spawning is now ";
			YouHaveBeenKickedBy = "You have been kicked by ";
			HasBeenKickedBy = " has been kicked by ";
			KickSlotIsEmpty = "kick: Slot is vacant.";
			KickPlayerNameNull = "kick: Error, player has null name.";
			RestartingServer = "Restarting the Server";
			StartingServer = "Starting the Server";
			PurgingProjectiles = "Purging all projectiles.";
			PurgingNPC = "Purging all NPCs.";
			PurgingItems = "Purging all items.";
			LoadedPlugins = "Loaded Plugins: ";
			NoPluginsLoaded = "There are no loaded plugins.";
			IssueFindingPlayer = "There was an issue finding the player.";
			NoOnlinePlayersToSpawnNear = "There is no Online Players to spawn near.";
			NeedsToBeNightTime = "This boss needs to be summoned in night time, Please override with -night";
			HardModeAlreadyEnabled = "Hardmode is already in place.";
			StartingHardMode = "Starting hardmode, This may take a while...";
			RPGMode_Allowed = "RPG Mode is now allowed on this server:";
			RPGMode_Refused = "RPG Mode is now refused on this server:";
			ItemRejection_Added = " was added to the Item Rejection list!";
			ItemRejection_ItemExists = "That item already exists in the list.";
			ItemRejection_Removed = " was removed from the Item Rejection list!";
			ItemRejection_ItemDoesntExist = "That item does not exist in the list.";
			ItemRejection_Cleared = "Item Rejection list has been cleared!";
			Ban_You = "You have been banned from this Server.";
			Ban_Banned = " has been banned";
			Ban_FailedToSave = "BanList Failed to Save due to ";
			Ban_UnBanned = " has been unbanned";
			CommandDescription_Exit = "Stop the save the world then exit program.";
			CommandDescription_SaveAll = "Save all world data and backup.";
			CommandDescription_ReloadPlugins = "Reload plugins.";
			CommandDescription_OnlinePlayers = "List active players (also: who, players, playing, online).";
			CommandDescription_Me = "Broadcast a message in third person.";
			CommandDescription_Say = "Broadcast a message as the Server.";
			CommandDescription_Slots = "Display information about occupied player slots.";
			CommandDescription_Kick = "Kick a player by name or slot.";
			CommandDescription_Ban = "Ban a player by Name or IP";
			CommandDescription_UnBan = "UnBan a player by Name or IP";
			CommandDescription_Whitelist = "Add or remove a player or IP to the whitelist";
			CommandDescription_Rcon = "Manage remote console access.";
			CommandDescription_Status = "Check the server's status";
			CommandDescription_Time = "Change the time of the World.";
			CommandDescription_Help = "Get a printout of active commands.";
			CommandDescription_Give = "Give a player items.";
			CommandDescription_SpawnNpc = "Spawn an NPC near a player.";
			CommandDescription_Teleport = "Teleport a player to another player.";
			CommandDescription_TeleportHere = "Teleport a player to yourself.";
			CommandDescription_SettleLiquids = "Settle Liquids.";
			CommandDescription_Op = "Op a player";
			CommandDescription_DeOp = "De-Op a player";
			CommandDescription_OpLogin = "OP Login System.";
			CommandDescription_OpLogout = "OP Logout System.";
			CommandDescription_NpcSpawns = "Toggle the state of NPC Spawning.";
			CommandDescription_Restart = "Restart the Server.";
			CommandDescription_Purge = "Purge the map of items, NPCs or projectiles.";
			CommandDescription_Plugins = "List currently enabled plugins.";
			CommandDescription_Plugin = "Enable/disable and get details about specific plugins.";
			CommandDescription_SpawnBoss = "Summon a Boss to the world.";
			CommandDescription_ItemRej = "Add or remove an item from the whitelist.";
			CommandDescription_Explosions = "Toggle the allowing of explosions for the server.";
			CommandDescription_MaxPlayers = "Set the maximum number of player slots.";
			CommandDescription_Q = "List connections waiting in queues.";
			CommandDescription_Refresh = "Redownload the area around you from the server.";
			CommandDescription_HardMode = "Toggle hardmode for the current world";
			CommandDescription_LanguageReload = "Reloads the Langauge File";
			CommandDescription_Backups = "Allows backups to be performed or removed.";
			Generation_Terrain = "Generating world terrain";
			Generation_AddingNPCs = "Adding NPC's...";
			Generation_AddingSand = "Adding sand";
			Generation_GeneratingHills = "Generating hills";
			Generation_AddingSnow = "Adding snow";
			Generation_PuttingDirtBehindDirt = "Putting dirt behind dirt";
			Generation_PlacingRocks = "Placing rocks within the dirt";
			Generation_PlaceDirtWithinRocks = "Placing dirt within the rocks";
			Generation_AddingClay = "Adding clay";
			Generation_MakingRandomHoles = "Making random holes";
			Generation_GeneratingSmallCaves = "Generating small caves";
			Generation_GeneratingLargeCaves = "Generating large caves";
			Generation_GeneratingSurfaceCaves = "Generating surface caves";
			Generation_GeneratingJungle = "Generating jungle";
			Generation_GeneratingFloatingIslands = "Generating floating islands";
			Generation_AddingMushroomPatches = "Adding mushroom patches";
			Generation_PlacingMudInDirt = "Placing mud in the dirt";
			Generation_AddingShinies = "Adding shinies";
			Generation_AddingWebs = "Adding webs";
			Generation_CreatingTheUnderworld = "Creating the underworld";
			Generation_AddingWaterBodies = "Adding water bodies";
			Generation_MakingTheWorldEvil = "Making the world evil";
			Generation_GeneratingMoutainCaves = "Generating moutain caves";
			Generation_CreatingBeachesOnEitherSide = "Creating beaches on either side";
			Generation_AddingGems = "Adding gems";
			Generation_GravitatingSand = "Gravitating sand";
			Generation_CleaningUpDirtBackgrounds = "Cleaning up dirt backgrounds";
			Generation_PlacingAltars = "Placing altars";
			Generation_SettlingLiquids = "Settling liquids";
			Generation_PlacingLifeCrystals = "Placing life crystals";
			Generation_HidingTreasure = "Hiding treasure";
			Generation_HidingMoreTreasure = "Hiding more treasure";
			Generation_HidingJungleTreasure = "Hiding jungle treasure";
			Generation_HidingWaterTreasure = "Hiding water treasure";
			Generation_AddingIslandHouses = "Adding island houses";
			Generation_PlacingBreakables = "Placing breakables";
			Generation_PlacingHellForges = "Placing hellforges";
			Generation_SpreadingGrass = "Spreading grass...";
			Generation_GrowingCacti = "Growing cacti...";
			Generation_PlantingSunflowers = "Planting sunflowers";
			Generation_PlantingTrees = "Planting trees...";
			Generation_PlantingHerbs = "Planting herbs...";
			Generation_PlantingWeeds = "Planting weeds...";
			Generation_GrowingVines = "Growing vines...";
			Generation_PlantingFlowers = "Planting flowers...";
			Generation_PlantingMushrooms = "Planting mushrooms...";
			Generation_CreatingDungeons = "Creating dungeons";
			Generation_PlacingTraps = "Placing traps";
			Startup_Initializing = "Initializing";
			Startup_SettingUpPaths = "Setting up paths.";
			Startup_SettingUpProperties = "Setting up Properties.";
			Startup_NoPropertiesFileFound = "New properties file created. Would you like to exit for editing? [Y/n]: ";
			Startup_PropertiesCreationComplete = "Complete, Press any Key to Exit...";
			Startup_IssueDeletingPID = "Issue deleting PID file, Continue? [Y/n]: ";
			Startup_PressAnyKeyToExit = "Press any key to exit...";
			Startup_IssueCreatingPID = "Issue creating PID file, Continue? [Y/n]: ";
			Startup_PIDCreated = "PID File Created, Process ID: ";
			Startup_RestartingIntoNewUpdate = "Restarting into new update!";
			Startup_ErrorUpdating = "Error updating";
			Startup_StartingRCON = "Starting remote console server";
			Startup_StartingPermissions = "Starting permissions manager";
			Startup_PreparingServerData = "Preparing server data";
			Startup_LoadingItemDefinitions = "Loading item definitions";
			Startup_LoadingNPCDefinitions = "Loading NPC definitions";
			Startup_LoadingProjectileDefinitions = "Loading projectile definitions";
			Startup_LanguageFileOOD = "Language file is out of date!";
			Startup_LanguageFileUpdate = "Please update it in order to use new features.";
			Startup_LoadingPlugins = "Loading plugins...";
			Startup_PluginsLoaded = "Plugins loaded: ";
			Startup_GeneratingWorld = "Generating world";
			Startup_GeneratedSeed = "Generated seed:";
			Startup_WorldSizingError = "World dimensions need to be equal or larger than";
			Startup_GeneratingWithCustomSize = "Generating world with custom map size:";
			Startup_StartingTheServer = "Starting the Server";
			Startup_YouCanNowInsertCommands = "You can now insert Commands.";
			Startup_IssueParsingConsoleCommand = "Issue parsing console command.";
			Startup_Exiting = "Exiting...";
			Startup_CrashlogGeneratedBy = "Crash log Generated by";
			Startup_ProgramCrash = "Program crash";
			Startup_PleaseSend = "Please send";
			Startup_LogEnd = "Log end.";
			CreatingTileArrayOf = "Creating tile array of";
			InitializingSlotManagerFor = "Initializing slot manager for";
			Players = "players";
			ServerStartedOn = "Server started on";
			Water_WaterHasBeenSettled = "Water has been settled.";
			Water_PerformingWaterCheck = "Performing water check";
			Water_PreparingLiquids = "Preparing liquids...";
			CheckingTileAlignment = "Checking tile alignment";
			LoadingWorldTiles = "Loading world tiles";
			ResettingGameObjects = "Resetting game objects";
			SavedFile = "Saved file";
			SavedTo = "Saved to";
			Failed = "failed";

			ExtendedLanguages = new Dictionary<String, String>();
		}
#endregion

		public static bool LoadClass(string filePath, bool restore = false, bool error = true, bool autoSave = true)
		{
			if (!File.Exists(filePath) || restore)
			{
				//if (File.Exists(filePath))
				//    File.Delete(filePath);

				/*using (var ctx = Assembly.GetExecutingAssembly().GetManifestResourceStream(Collections.Registries.DEFINITIONS + filePath))
				{
					using (var stream = File.OpenWrite(filePath))
					{
						var buff = new byte[ctx.Length];
						ctx.Read(buff, 0, buff.Length);
						stream.Write(buff, 0, buff.Length);
					}
				}*/
				Save(filePath);
			}

			using (var stream = File.Open(filePath, FileMode.Open))
			{
				var document = new XmlDocument();
				document.Load(stream);

				var type = typeof(Languages);
				var typeProperties = type.GetProperties();

				foreach (XmlNode child in document.ChildNodes)
					foreach (XmlNode node in child.ChildNodes)
					{
						try
						{
							var property = node.Name;
							var value = node.InnerText;

							var properties = from x in typeProperties where x.Name == property select x;

							if (properties.Count() == 0)
							{
								ExtendedLanguages[property] = value;
								continue;
							}

							foreach (var prop in properties)
								prop.SetValue(null, value, null);
						}
						catch (Exception e)
						{
							if (error)
							{
								ProgramLog.Error.Log("Error parsing language file\n{0}", e);
								return false;
							}
						}
					}
			}

			if (autoSave) return Save(filePath);

			return true;
		}

		public static bool Save(string location)
		{
			try
			{
				if (File.Exists(location)) File.Delete(location);

				using (var writer = new XmlTextWriter(location, Encoding.ASCII))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("Languages");

					var totalProperties = typeof(Languages).GetProperties();

					foreach (var property in totalProperties)
					{
						var name = property.Name;
						var value = property.GetValue(null, null).ToString();
						if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value))
						{
							writer.WriteStartElement(name);
							writer.WriteString(value);
							writer.WriteEndElement();
						}
					}

					lock (ExtendedLanguages)
					{
						foreach (var pair in ExtendedLanguages)
						{
							writer.WriteStartElement(pair.Key);
							writer.WriteString(pair.Value);
							writer.WriteEndElement();
						}
					}

					writer.WriteEndElement();
					writer.WriteEndDocument();

					return true;
				}
			}
			catch (Exception e)
			{
				ProgramLog.Error.Log("Error saving language file\n{0}", e);
			}

			return false;
		}

		/*
		 * Not needed now with the new system as it automatically replaces out of date.
		public static bool IsOutOfDate()
		{
			var type = typeof(Languages);
			var properties = from x in type.GetProperties() where x.GetValue(null, null) == null select x;

			return properties.Count() > 0;
		}*/
	}
}