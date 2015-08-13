
using System;
using System.IO;
using TDSM.API;
using Terraria;
using TDSM.API.Data;
using TDSM.API.Logging;

namespace TDSM.Core.ServerCharacters
{
    public static class CharacterManager
    {
        internal const String SQLSafeName = "tdsm";
        internal const String Key_NewCharacter = "tdsm_NewCharacter";

        public enum ItemType
        {
            Inventory = 1,
            Armor,
            Dye
        }

        public static CharacterMode Mode { get; set; }

        public static NewPlayerInfo StartingOutInfo = new NewPlayerInfo()
        {
            Health = 200,
            MaxHealth = 200,
            Mana = 20,
            MaxMana = 20,

            Inventory = new SlotItem[]
            {
                new SlotItem(-15, 1, 0, 0),
                new SlotItem(-13, 1, 0, 1),
                new SlotItem(-16, 1, 0, 2)
            }
        };

        public static void Init()
        {
            if (Storage.IsAvailable)
            {
                if (!Tables.CharacterTable.Exists())
                {
                    ProgramLog.Admin.Log("SSC table does not exist and will now be created");
                    Tables.CharacterTable.Create();
                }
                if (!Tables.ItemTable.Exists())
                {
                    ProgramLog.Admin.Log("SSC item table does not exist and will now be created");
                    Tables.ItemTable.Create();
                }
                if (!Tables.PlayerBuffTable.Exists())
                {
                    ProgramLog.Admin.Log("SSC player buff table does not exist and will now be created");
                    Tables.PlayerBuffTable.Create();
                }
                if (!Tables.DefaultLoadoutTable.Exists())
                {
                    ProgramLog.Admin.Log("SSC loadout table does not exist and will now be created");
                    Tables.DefaultLoadoutTable.Create();
                    Tables.DefaultLoadoutTable.PopulateDefaults(StartingOutInfo);
                }
            }

            //Player inventory,armor,dye common table

            //Default loadout table
            LoadConfig();
        }

        /// <summary>
        /// Load the default start gear
        /// </summary>
        public static void LoadConfig()
        {

        }

        static bool _hadPlayers;
        static DateTime _lastSave;

        public static bool EnsureSave { get; set; }


        public static void SaveAll()
        {
            if ((DateTime.Now - _lastSave).TotalSeconds >= 5)
            {
                //Don't perform any unnecessary writes
                var hasPlayers = Netplay.anyClients;
                if (!hasPlayers && !_hadPlayers && !EnsureSave)
                    return;

                EnsureSave = false;
                try
                {
                    foreach (var ply in Terraria.Main.player)
                    {
                        if (ply != null && ply.active)
                        {
                            SavePlayerData(ply);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e);
                }

                _hadPlayers = hasPlayers;
                _lastSave = DateTime.Now;
            }
        }

        public static ServerCharacter LoadPlayerData(Player player, bool returnNewInfo = false)
        {
            //If using a flat based system ensure the MODE is stored
            string authName = null;
            if (Mode == CharacterMode.AUTH)
            {
                if (!String.IsNullOrEmpty(player.AuthenticatedAs))
                    authName = player.AuthenticatedAs;
            }
            else if (Mode == CharacterMode.UUID)
            {
                if (!String.IsNullOrEmpty(player.ClientUUId))
                    authName = player.ClientUUId + '@' + player.name;
            }

            if (!String.IsNullOrEmpty(authName))
            {
                ProgramLog.Log("Loading SSC for " + authName);
                if (false == true && Storage.IsAvailable)
                {

                }
                else
                {
                    var dir = Path.Combine(Globals.CharacterDataPath, Mode.ToString());
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var file = Path.Combine(dir, authName + ".ssc");
                    if (true == false && System.IO.File.Exists(file))
                    {
                        var json = System.IO.File.ReadAllText(file);
                        if (json.Length > 0)
                        {
                            ProgramLog.Log("Loading existing loadout");
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<ServerCharacter>(json);
                        }
                        else
                        {
                            ProgramLog.Log("Player data was empty");
                        }
                    }

                    if (returnNewInfo)
                    {
                        ProgramLog.Log("Issuing new loadout");
//                        player.SetPluginData(Key_NewCharacter, true);
                        EnsureSave = true; //Save is now required
                        return new ServerCharacter(StartingOutInfo, player);
                    }
                }
            }

            return null;
        }

        public static bool SavePlayerData(Player player)
        {
            //If using a flat based system ensure the MODE is stored
            string authName = null;
            if (Mode == CharacterMode.AUTH)
            {
                if (!String.IsNullOrEmpty(player.AuthenticatedAs))
                    authName = player.AuthenticatedAs;
            }
            else if (Mode == CharacterMode.UUID)
            {
                if (!String.IsNullOrEmpty(player.ClientUUId))
                    authName = player.ClientUUId + '@' + player.name;
            }

            if (!String.IsNullOrEmpty(authName))
            {
                if (Storage.IsAvailable)
                {
//                    var create = player.GetPluginData<Boolean>(Key_NewCharacter, false);
                    var existingId = Tables.CharacterTable.GetCharacterId(Mode, player.AuthenticatedAs, player.ClientUUId);
                    if (existingId <= 0)
                    {
                        if (player.ClearPluginData(Key_NewCharacter))
                        {
                            var characterId = Tables.CharacterTable.NewCharacter
                            (
                                Mode,
                                player.AuthenticatedAs,
                                player.ClientUUId,
                                player.statLife,
                                player.statLifeMax,
                                player.statMana,
                                player.statManaMax,
                                player.SpawnX,
                                player.SpawnY,
                                player.hair,
                                player.hairDye,
                                player.hideVisual,
                                player.difficulty,
                                player.hairColor,
                                player.skinColor,
                                player.eyeColor,
                                player.shirtColor,
                                player.underShirtColor,
                                player.pantsColor,
                                player.shoeColor,
                                player.anglerQuestsFinished
                            );

                            if (characterId > 0)
                            {

                            }
                        }
                        else
                        {
                            ProgramLog.Error.Log("Failed to save SSC for player: {0}", player.Name);
                        }
                    }
                    else
                    {
                        Tables.CharacterTable.UpdateCharacter
                        (
                            Mode,
                            player.AuthenticatedAs,
                            player.ClientUUId,
                            player.statLife,
                            player.statLifeMax,
                            player.statMana,
                            player.statManaMax,
                            player.SpawnX,
                            player.SpawnY,
                            player.hair,
                            player.hairDye,
                            player.hideVisual,
                            player.difficulty,
                            player.hairColor,
                            player.skinColor,
                            player.eyeColor,
                            player.shirtColor,
                            player.underShirtColor,
                            player.pantsColor,
                            player.shoeColor,
                            player.anglerQuestsFinished
                        );

                        var characterId = Tables.CharacterTable.GetCharacterId(Mode, player.AuthenticatedAs, player.ClientUUId);
                        if (characterId > 0)
                        {

                        }
                    }
                }
                else
                {
                    var dir = Path.Combine(Globals.CharacterDataPath, Mode.ToString());
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var file = Path.Combine(dir, authName + ".ssc");
                    var data = new ServerCharacter(player);

//                    if (data.Buffs != null && data.BuffTime != null)
//                    {
//                        var max = Math.Min(data.Buffs.Length, data.BuffTime.Length);
//                        for (var x = 0; x < max; x++)
//                        {
//                            if (data.Buffs[x] > 0)
//                            {
//                                var time = data.BuffTime[x] * 60;
//
//                                ProgramLog.Plugin.Log("Saving buff {0} for {1}/{2}", data.Buffs[x], time, data.BuffTime[x]);
//                            }
//                        }
//                    }

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    System.IO.File.WriteAllText(file, json);
                    return true;
                }
            }
            return false;
        }

        public static void LoadForAuthenticated(Player player, bool createIfNone = true)
        {
            var ssc = LoadPlayerData(player, createIfNone);

            if (ssc != null)
            {
                //Check to make sure the player is the same player (ie skin, clothes)
                //Add hooks for pre and post apply

                ssc.ApplyToPlayer(player);
            }
            else
            {
                ProgramLog.Log("No SSC data");
            }
        }
    }
}
