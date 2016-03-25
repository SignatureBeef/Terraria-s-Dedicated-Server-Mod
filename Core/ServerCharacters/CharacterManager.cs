using System;
using System.IO;
using OTA;
using Terraria;
using OTA.Data;
using OTA.Logging;
using OTA.Plugin;
using System.Linq;
using TDSM.Core.Data;
using TDSM.Core.Data.Extensions;
using System.Data;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.ServerCharacters
{
    public static class CharacterManager
    {
        internal const String SQLSafeName = "tdsm";
        internal const String Key_NewCharacter = "tdsm_NewCharacter";

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
        
        [TDSMComponent(ComponentEvent.Initialise)]
        public static void Init(Entry plugin)
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


            CharacterMode characterMode;
            if (CharacterMode.TryParse(plugin.Config.SSC_CharacterMode, out characterMode))
            {
                Terraria.Main.ServerSideCharacter = characterMode != CharacterMode.NONE;
                CharacterManager.Mode = characterMode;
                ProgramLog.Admin.Log("SSC mode: " + characterMode);

                plugin.Hook(HookPoints.ReceiveNetMessage, OnNetMessageReceived);
                //                        Hook(HookPoints.PlayerDataReceived, OnPlayerDataReceived);
            }
//            else
//                ProgramLog.Error.Log("Failed to parse line server-side-characters. No SSC will be used.");

            AllowGuestInfo = plugin.Config.SSC_AllowGuestInfo;
            SaveInterval = plugin.Config.SSC_SaveInterval;

            //Default loadout table
            LoadConfig();
        }

        static bool AllowGuestInfo;
        static void OnNetMessageReceived(ref HookContext ctx, ref HookArgs.ReceiveNetMessage args)
        {
            if (Terraria.Main.ServerSideCharacter)
            {
                switch ((Packet)args.PacketId)
                {
                    case Packet.INVENTORY_DATA:
                        if (!AllowGuestInfo && !ctx.Player.IsAuthenticated())
                            ctx.SetResult(HookResult.IGNORE);
                        break;

                    case Packet.PLAYER_MANA_UPDATE:
                        if (!AllowGuestInfo && !ctx.Player.IsAuthenticated())
                            ctx.SetResult(HookResult.IGNORE);
                        break;

                    case Packet.PLAYER_HEALTH_UPDATE:
                        if (!AllowGuestInfo && !ctx.Player.IsAuthenticated())
                            ctx.SetResult(HookResult.IGNORE);
                        break;
                }
            }
        }

        /// <summary>
        /// Load the default start gear and settings
        /// </summary>
        public static void LoadConfig()
        {

        }

        static bool _hadPlayers;
        static DateTime _lastSave;

        public static bool EnsureSave { get; set; }

        /// <summary>
        /// Gets or sets the save interval, in seconds
        /// </summary>
        /// <value>The save interval.</value>
        public static int SaveInterval { get; set; } = 5;

        public static void SaveAll()
        {
            if ((DateTime.Now - _lastSave).TotalSeconds >= SaveInterval)
            {
                //Don't perform any unnecessary writes
                var hasPlayers = Netplay.anyClients;
                if (!hasPlayers && !_hadPlayers && !EnsureSave)
                    return;

                EnsureSave = false;
                try
                {
#if ENTITY_FRAMEWORK_7
                    using (var ctx = new TContext())
#elif DAPPER
                    using(var ctx = DatabaseFactory.CreateConnection())
#endif
                    {
                        using (var txn = ctx.BeginTransaction())
                        {
                            foreach (var ply in Terraria.Main.player)
                            {
                                if (ply != null && ply.active && ply.GetSSCReadyForSave())
                                {
                                    SavePlayerData(ctx, txn, false, ply);
                                }
                            }
                            txn.Commit();
                        }

#if ENTITY_FRAMEWORK_7
                        ctx.SaveChanges();
#endif
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

        public static ServerCharacter LoadPlayerData(IDbConnection ctx, IDbTransaction txn, Player player, bool returnNewInfo = false)
        {
            //If using a flat based system ensure the MODE is stored
            string authName = null;
            if (Mode == CharacterMode.AUTH)
            {
                var auth = player.GetAuthenticatedAs();
                if (!String.IsNullOrEmpty(auth))
                    authName = auth;
            }
            else if (Mode == CharacterMode.UUID)
            {
                if (!String.IsNullOrEmpty(player.ClientUUId))
                    authName = player.ClientUUId + '@' + player.name;
            }

//            ProgramLog.Admin.Log("SSC is: " + Storage.IsAvailable);
//            ProgramLog.Admin.Log("Finding SSC for: " + (authName ?? "NULL"));

            if (!String.IsNullOrEmpty(authName))
            {
//                ProgramLog.Log("Loading SSC for " + authName);

                if (Storage.IsAvailable)
                {
                    var auth = player.GetAuthenticatedAs();
#if ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
                    var dbSSC = Tables.CharacterTable.GetCharacter(ctx, Mode, auth, player.ClientUUId);

//                    ProgramLog.Admin.Log("Found SCC: " + (dbSSC != null));

                    if (dbSSC != null)
                    {
                        var ssc = dbSSC.ToServerCharacter();
#elif DATA_CONNECTOR
                    var ssc = Tables.CharacterTable.GetCharacter(Mode, auth, player.ClientUUId);
                    if (ssc != null)
                    { 
#elif DAPPER
                    var dbSSC = Tables.CharacterTable.GetCharacter(ctx, txn, Mode, auth, player.ClientUUId);
                    if (dbSSC != null)
                    {
                        var ssc = dbSSC.ToServerCharacter();
#endif
                        //                        ProgramLog.Log("Loading SSC loadout");

                        //                        ProgramLog.Admin.Log("Loading SSC loadout: " + dbSSC.Id);
                        //                        ProgramLog.Admin.Log("Translated SCC: " + (ssc != null));
                        //
                        var inv = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Inventory, ssc.Id);
                        if (null != inv) ssc.Inventory = inv.ToList();

                        var amr = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Armor, ssc.Id);
                        if (null != amr) ssc.Armor = amr.ToList();

                        var dye = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Dye, ssc.Id);
                        if (null != dye) ssc.Dye = dye.ToList();

                        var equipment = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Equipment, ssc.Id);
                        if (null != equipment) ssc.Equipment = equipment.ToList();

                        var miscdye = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.MiscDyes, ssc.Id);
                        if (null != miscdye) ssc.MiscDyes = miscdye.ToList();

                        var bank = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Bank, ssc.Id);
                        if (null != bank) ssc.Bank = bank.ToList();

                        var bank2 = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Bank2, ssc.Id);
                        if (null != bank2) ssc.Bank2 = bank2.ToList();

                        var trash = Tables.ItemTable.GetItemsForCharacter(ctx, txn, ItemType.Trash, ssc.Id);
#if ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
                        if (null != trash && trash.Count > 0) ssc.Trash = trash.First();
#elif DATA_CONNECTOR
                        if (null != trash && trash.Length > 0) ssc.Trash = trash.First();
#endif

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

#if ENTITY_FRAMEWORK_7
        public static bool SavePlayerData(TContext ctx, bool save, Player player)
#elif DAPPER
        public static bool SavePlayerData(IDbConnection ctx, IDbTransaction txn, bool save, Player player)
#endif
        {
            if (!player.IsAuthenticated()) return false;

            //If using a flat based system ensure the MODE is stored
            string authName = null;
            if (Mode == CharacterMode.AUTH)
            {
                var auth = player.GetAuthenticatedAs();
                if (!String.IsNullOrEmpty(auth))
                    authName = auth;
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
                    var auth = player.GetAuthenticatedAs();

#if ENTITY_FRAMEWORK_6
                    int userId = 0;
                    if (Mode == CharacterMode.AUTH)
                    {
                        var user = AuthenticatedUsers.GetUser(auth);

                        if (user == null)
                        {
                            OTA.Logging.ProgramLog.Error.Log("No user found ");
                            return false;
                        }

                        userId = user.Id;
                    }
                    else if (Mode != CharacterMode.UUID)
                        return false;

                    //Sync the character
                    var character = ctx.GetCharacter(Mode, userId, player.ClientUUId);
                    if (null == character)
                    {
                        character = ctx.AddCharacter(userId, player);
                    }
                    else character.UpdateCharacter(userId, player);

                    //Sync items
                    var items = ctx.Items.Where(x => x.CharacterId == character.Id).ToArray();

                    if (!SaveCharacterItems(ctx, save, player, character.Id, player.inventory, ItemType.Inventory, items)) return false;
                    if (!SaveCharacterItems(ctx, save, player, character.Id, player.armor, ItemType.Armor, items)) return false;
                    if (!SaveCharacterItems(ctx, save, player, character.Id, player.dye, ItemType.Dye, items)) return false;
                    if (!SaveCharacterItems(ctx, save, player, character.Id, player.miscEquips, ItemType.Equipment, items)) return false;
                    if (!SaveCharacterItems(ctx, save, player, character.Id, player.miscDyes, ItemType.MiscDyes, items)) return false;
                    if (!SaveCharacterItem(ctx, save, player, character.Id, ItemType.Trash, player.trashItem, 0, items)) return false;


#elif ENTITY_FRAMEWORK_7
#else
                    var character = Tables.CharacterTable.GetCharacter(ctx, txn, Mode, auth, player.ClientUUId);
                    if (character == null)
                    {
//                        if (player.ClearPluginData(Key_NewCharacter))
//                        {
                        character = Tables.CharacterTable.NewCharacter
                            (
                            ctx,
                            txn,
                            Mode,
                            auth,
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
//                            ProgramLog.Error.Log("Failed to save SSC for player: {0}", player.name);
//                            return false;
//                        }
                    }
                    else
                    {
                        character = Tables.CharacterTable.UpdateCharacter
                        (
                            ctx,
                            txn,
                            Mode,
                            auth,
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
                        if (!SaveCharacterItems(ctx, txn, save, player, character.Id, player.inventory, ItemType.Inventory)) return false;
                        if (!SaveCharacterItems(ctx, txn, save, player, character.Id, player.armor, ItemType.Armor)) return false;
                        if (!SaveCharacterItems(ctx, txn, save, player, character.Id, player.dye, ItemType.Dye)) return false;
                        if (!SaveCharacterItems(ctx, txn, save, player, character.Id, player.miscEquips, ItemType.Equipment)) return false;
                        if (!SaveCharacterItems(ctx, txn, save, player, character.Id, player.miscDyes, ItemType.MiscDyes)) return false;
                        if (!SaveCharacterItem(ctx, txn, save, player, character.Id, ItemType.Trash, player.trashItem, 0)) return false;

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
//                                    ProgramLog.Error.Log("Failed to save Inventory for player: {0}", player.name);
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
//                                    ProgramLog.Error.Log("Failed to save Armor for player: {0}", player.name);
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
//                                    ProgramLog.Error.Log("Failed to save Dye for player: {0}", player.name);
//                                    return false;
//                                }
//                            }
//                            else
//                            {
//                                itemId = Tables.ItemTable.NewItem(ItemType.Dye, netId, prefix, stack, favorite, i, characterId);
//                            }
//                        }
                    }
#endif
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

#if ENTITY_FRAMEWORK_6
        private static bool SaveCharacterItem(TContext ctx, bool save, Player player, int characterId, ItemType type, Item item, int slot, SlotItem[] existing)
        {
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

            var slotItem = existing.SingleOrDefault(x => x.Slot == slot && x.Type == type && x.NetId == netId);
            if (slotItem != null)
            {
                slotItem.Favorite = favorite;
                slotItem.Stack = stack;
                slotItem.Prefix = prefix;
            }
            else
            {
                slotItem = new SlotItem()
                {
                    CharacterId = characterId,
                    Slot = slot,
                    NetId = netId,
                    Stack = stack,
                    Prefix = prefix,
                    Favorite = favorite,
                    Type = type
                };
                ctx.Items.Add(slotItem);
            }

            return true;
        }

        private static bool SaveCharacterItems(TContext ctx, bool save, Player player, int characterId, Item[] items, ItemType type, SlotItem[] existing)
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (!SaveCharacterItem(ctx, save, player, characterId, type, items[i], i, existing)) return false;
            }

            return true;
        }
#else
#if ENTITY_FRAMEWORK_7
        private static bool SaveCharacterItem(TContext ctx, bool save, Player player, int characterId, ItemType type, Item item, int slot)
#elif DAPPER
        private static bool SaveCharacterItem(IDbConnection ctx, IDbTransaction txn, bool save, Player player, long characterId, ItemType type, Item item, int slot)
#endif
        {
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

            var slotItem = Tables.ItemTable.GetItem(ctx, txn, type, slot, characterId);
            if (slotItem != null)
            {
                if (!Tables.ItemTable.UpdateItem(ctx, txn, save, type, netId, prefix, stack, favorite, slot, characterId))
                {
                    ProgramLog.Error.Log("Failed to save {1} for player: {0}", player.name, type.ToString());
                    return false;
                }
            }
            else
            {
                slotItem = Tables.ItemTable.NewItem(ctx, txn, save, type, netId, prefix, stack, favorite, slot, characterId);
            }

            return true;
        }

#if ENTITY_FRAMEWORK_7
        private static bool SaveCharacterItems(TContext ctx, bool save, Player player, int characterId, Item[] items, ItemType type)
#elif DAPPER
        private static bool SaveCharacterItems(IDbConnection ctx, IDbTransaction transaction, bool save, Player player, long characterId, Item[] items, ItemType type)
#endif
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (!SaveCharacterItem(ctx, transaction, save, player, characterId, type, items[i], i)) return false;
            }

            return true;
        }
#endif

#if ENTITY_FRAMEWORK_7
        public static void LoadForAuthenticated(TContext ctx, Player player, bool createIfNone = true)
#elif DAPPER
        public static void LoadForAuthenticated(IDbConnection ctx, IDbTransaction txn, Player player, bool createIfNone = true)
#endif
        {
            var ssc = LoadPlayerData(ctx, txn, player, createIfNone);

            if (ssc != null)
            {
//                var loaded = String.Join(",", ssc.Inventory.Select(x => x.NetId).Where(x => x > 0).ToArray());
//                ProgramLog.Admin.Log("Loaded items: " + loaded);

                //Check to make sure the player is the same player (ie skin, clothes)
                //Add hooks for pre and post apply

                var hctx = new HookContext()
                {
                    Player = player,
                    Sender = player
                };

                var args = new TDSM.Core.Events.HookArgs.PreApplyServerSideCharacter()
                {
                    Character = ssc
                };

                TDSM.Core.Events.HookPoints.PreApplyServerSideCharacter.Invoke(ref hctx, ref args);

                args.Character.ApplyToPlayer(player);

                var args1 = new TDSM.Core.Events.HookArgs.PostApplyServerSideCharacter();
                TDSM.Core.Events.HookPoints.PostApplyServerSideCharacter.Invoke(ref hctx, ref args1);
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
