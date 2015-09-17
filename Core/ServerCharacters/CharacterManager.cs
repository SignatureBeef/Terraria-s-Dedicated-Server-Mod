using System;
using System.IO;
using OTA;
using Terraria;
using OTA.Data;
using OTA.Logging;
using OTA.Plugin;
using System.Linq;

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
            Dye,
            Equipment,
            MiscDyes,
            Bank,
            Bank2
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
                new SlotItem(-15, 1, 0, false, 0),
                new SlotItem(-13, 1, 0, false, 1),
                new SlotItem(-16, 1, 0, false, 2)
            }
        };

        public static void Init()
        {
//            if (Storage.IsAvailable)
//            {
//                if (!Tables.CharacterTable.Exists())
//                {
//                    ProgramLog.Admin.Log("SSC table does not exist and will now be created");
//                    Tables.CharacterTable.Create();
//                }
//                if (!Tables.ItemTable.Exists())
//                {
//                    ProgramLog.Admin.Log("SSC item table does not exist and will now be created");
//                    Tables.ItemTable.Create();
//                }
//                if (!Tables.PlayerBuffTable.Exists())
//                {
//                    ProgramLog.Admin.Log("SSC player buff table does not exist and will now be created");
//                    Tables.PlayerBuffTable.Create();
//                }
//                if (!Tables.DefaultLoadoutTable.Exists())
//                {
//                    ProgramLog.Admin.Log("SSC loadout table does not exist and will now be created");
//                    Tables.DefaultLoadoutTable.Create();
//                    Tables.DefaultLoadoutTable.PopulateDefaults(StartingOutInfo);
//                }
//            }

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
//                ProgramLog.Log("Loading SSC for " + authName);

                if (Storage.IsAvailable)
                {
                    var dbSSC = Tables.CharacterTable.GetCharacter(Mode, player.AuthenticatedAs, player.ClientUUId);

                    if (dbSSC != null)
                    {
                        var ssc = dbSSC.ToServerCharacter();
//                        ProgramLog.Log("Loading SSC loadout");

                        var inv = Tables.ItemTable.GetItemsForCharacter(ItemType.Inventory, ssc.Id);
                        if (null != inv) ssc.Inventory = inv.ToList();

                        var amr = Tables.ItemTable.GetItemsForCharacter(ItemType.Armor, ssc.Id);
                        if (null != amr) ssc.Armor = amr.ToList();

                        var dye = Tables.ItemTable.GetItemsForCharacter(ItemType.Dye, ssc.Id);
                        if (null != dye) ssc.Dye = dye.ToList();

                        var equipment = Tables.ItemTable.GetItemsForCharacter(ItemType.Equipment, ssc.Id);
                        if (null != equipment) ssc.Equipment = equipment.ToList();

                        var miscdye = Tables.ItemTable.GetItemsForCharacter(ItemType.MiscDyes, ssc.Id);
                        if (null != miscdye) ssc.MiscDyes = miscdye.ToList();

                        var bank = Tables.ItemTable.GetItemsForCharacter(ItemType.Bank, ssc.Id);
                        if (null != bank) ssc.Bank = bank.ToList();

                        var bank2 = Tables.ItemTable.GetItemsForCharacter(ItemType.Bank2, ssc.Id);
                        if (null != bank2) ssc.Bank2 = bank2.ToList();

                        return ssc;
                    }
                    else
                    {
                        if (returnNewInfo)
                        {
//                            ProgramLog.Log("Issuing new loadout");
                            //                        player.SetPluginData(Key_NewCharacter, true);
                            EnsureSave = true; //Save is now required
                            return new ServerCharacter(StartingOutInfo, player);
                        }
//                        else ProgramLog.Log("New loadout not specified");
                    }
                }
                else
                {
                    var dir = Path.Combine(Globals.CharacterDataPath, Mode.ToString());
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var file = Path.Combine(dir, authName + ".ssc");
                    if (System.IO.File.Exists(file))
                    {
                        var json = System.IO.File.ReadAllText(file);
                        if (json.Length > 0)
                        {
//                            ProgramLog.Log("Loading existing loadout");
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<ServerCharacter>(json);
                        }
                        else
                        {
                            ProgramLog.Log("Player data was empty");
                        }
                    }

                    if (returnNewInfo)
                    {
//                        ProgramLog.Log("Issuing new loadout");
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
                    var character = Tables.CharacterTable.GetCharacter(Mode, player.AuthenticatedAs, player.ClientUUId);
                    if (character == null)
                    {
//                        if (player.ClearPluginData(Key_NewCharacter))
//                        {
                        character = Tables.CharacterTable.NewCharacter
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
//                        }
//                        else
//                        {
//                            ProgramLog.Error.Log("Failed to save SSC for player: {0}", player.Name);
//                            return false;
//                        }
                    }
                    else
                    {
                        character = Tables.CharacterTable.UpdateCharacter
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
                    }

                    if (character != null)
                    {
                        if (!SaveCharacterItems(player, character.Id, player.inventory, ItemType.Inventory)) return false;
                        if (!SaveCharacterItems(player, character.Id, player.armor, ItemType.Armor)) return false;
                        if (!SaveCharacterItems(player, character.Id, player.dye, ItemType.Dye)) return false;
                        if (!SaveCharacterItems(player, character.Id, player.miscEquips, ItemType.Equipment)) return false;
                        if (!SaveCharacterItems(player, character.Id, player.miscDyes, ItemType.MiscDyes)) return false;
//                        for (var i = 0; i < player.inventory.Length; i++)
//                        {
//                            var item = player.inventory[i];
//                            var netId = 0;
//                            var prefix = 0;
//                            var stack = 0;
//                            var favorite = false;
//
//                            if (item != null)
//                            {
//                                netId = item.netID;
//                                prefix = item.prefix;
//                                stack = item.stack;
//                                favorite = item.favorited;
//                            }
//
//                            var itemId = Tables.ItemTable.GetItem(ItemType.Inventory, i, characterId);
//                            if (itemId > 0)
//                            {
//                                if (!Tables.ItemTable.UpdateItem(ItemType.Inventory, netId, prefix, stack, favorite, i, characterId))
//                                {
//                                    ProgramLog.Error.Log("Failed to save Inventory for player: {0}", player.Name);
//                                    return false;
//                                }
//                            }
//                            else
//                            {
//                                itemId = Tables.ItemTable.NewItem(ItemType.Inventory, netId, prefix, stack, favorite, i, characterId);
//                            }
//                        }
//                        for (var i = 0; i < player.armor.Length; i++)
//                        {
//                            var item = player.armor[i];
//                            var netId = 0;
//                            var prefix = 0;
//                            var stack = 0;
//                            var favorite = false;
//
//                            if (item != null)
//                            {
//                                netId = item.netID;
//                                prefix = item.prefix;
//                                stack = item.stack;
//                                favorite = item.favorited;
//                            }
//
//                            var itemId = Tables.ItemTable.GetItem(ItemType.Armor, i, characterId);
//                            if (itemId > 0)
//                            {
//                                if (!Tables.ItemTable.UpdateItem(ItemType.Armor, netId, prefix, stack, favorite, i, characterId))
//                                {
//                                    ProgramLog.Error.Log("Failed to save Armor for player: {0}", player.Name);
//                                    return false;
//                                }
//                            }
//                            else
//                            {
//                                itemId = Tables.ItemTable.NewItem(ItemType.Armor, netId, prefix, stack, favorite, i, characterId);
//                            }
//                        }
//                        for (var i = 0; i < player.dye.Length; i++)
//                        {
//                            var item = player.dye[i];
//                            var netId = 0;
//                            var prefix = 0;
//                            var stack = 0;
//                            var favorite = false;
//
//                            if (item != null)
//                            {
//                                netId = item.netID;
//                                prefix = item.prefix;
//                                stack = item.stack;
//                                favorite = item.favorited;
//                            }
//
//                            var itemId = Tables.ItemTable.GetItem(ItemType.Dye, i, characterId);
//                            if (itemId > 0)
//                            {
//                                if (!Tables.ItemTable.UpdateItem(ItemType.Dye, netId, prefix, stack, favorite, i, characterId))
//                                {
//                                    ProgramLog.Error.Log("Failed to save Dye for player: {0}", player.Name);
//                                    return false;
//                                }
//                            }
//                            else
//                            {
//                                itemId = Tables.ItemTable.NewItem(ItemType.Dye, netId, prefix, stack, favorite, i, characterId);
//                            }
//                        }
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

        private static bool SaveCharacterItems(Player player, int characterId, Item[] items, ItemType type)
        {
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var netId = 0;
                var prefix = 0;
                var stack = 0;
                var favorite = false;

                if (item != null)
                {
                    netId = item.netID;
                    prefix = item.prefix;
                    stack = item.stack;
                    favorite = item.favorited;
                }

                var slotItem = Tables.ItemTable.GetItem(type, i, characterId);
                if (slotItem != null)
                {
                    if (!Tables.ItemTable.UpdateItem(type, netId, prefix, stack, favorite, i, characterId))
                    {
                        ProgramLog.Error.Log("Failed to save {1} for player: {0}", player.Name, type.ToString());
                        return false;
                    }
                }
                else
                {
                    slotItem = Tables.ItemTable.NewItem(type, netId, prefix, stack, favorite, i, characterId);
                }
            }

            return true;
        }

        public static void LoadForAuthenticated(Player player, bool createIfNone = true)
        {
            var ssc = LoadPlayerData(player, createIfNone);

            if (ssc != null)
            {
                //Check to make sure the player is the same player (ie skin, clothes)
                //Add hooks for pre and post apply

                var ctx = new HookContext()
                {
                    Player = player,
                    Sender = player
                };

                var args = new TDSM.Core.Events.HookArgs.PreApplyServerSideCharacter()
                {
                    Character = ssc
                };

                TDSM.Core.Events.HookPoints.PreApplyServerSideCharacter.Invoke(ref ctx, ref args);

                args.Character.ApplyToPlayer(player);

                var args1 = new TDSM.Core.Events.HookArgs.PostApplyServerSideCharacter();
                TDSM.Core.Events.HookPoints.PostApplyServerSideCharacter.Invoke(ref ctx, ref args1);
            }
            else
            {
                ProgramLog.Log("No SSC data");
            }
        }

        public static void LoadForGuest(Player player)
        {
            var ssc = new ServerCharacter(StartingOutInfo, player);
            ssc.ApplyToPlayer(player);
            //TODO add guest events
            //Check to make sure the player is the same player (ie skin, clothes)
            //Add hooks for pre and post apply

//            var ctx = new HookContext()
//            {
//                Player = player,
//                Sender = player
//            };
//
//            var args = new TDSM.Core.Events.HookArgs.PreApplyServerSideCharacter()
//            {
//                Character = ssc
//            };
//
//            TDSM.Core.Events.HookPoints.PreApplyServerSideCharacter.Invoke(ref ctx, ref args);

//            args.Character.ApplyToPlayer(player);

//            var args1 = new TDSM.Core.Events.HookArgs.PostApplyServerSideCharacter();
//            TDSM.Core.Events.HookPoints.PostApplyServerSideCharacter.Invoke(ref ctx, ref args1);
        }
    }
}
