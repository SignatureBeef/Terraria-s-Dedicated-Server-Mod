using System;
using OTA.Command;
using TDSM.Core.Definitions;
using OTA;
using Terraria;
using Microsoft.Xna.Framework;
using TDSM.Core.Misc;
using System.Linq;

namespace TDSM.Core.Command.Commands
{
    public class SpawnNpcCommand : CoreCommand
    {
        public override void Initialise()
        {
            AddCommand("spawnnpc")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawns NPCs")
                .WithHelpText("<amount> \"<name:id>\" \"<player>\"")
                .WithHelpText("<amount> \"<name:id>\" \"<player>\" -item <item>")
                .WithPermissionNode("tdsm.spawnnpc")
                .Calls(this.SpawnNPC);
        }

        /// <summary>
        /// Spawns specified NPC type.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        /// <remarks>This function also allows NPC custom health.</remarks>
        public void SpawnNPC(ISender sender, ArgumentList args)
        {
            //if (Main.stopSpawns && !Program.properties.NPCSpawnsOverride)
            //    throw new CommandError("NPC Spawing is disabled.");

            //var health = -1;
            //var customHealth = args.TryPopAny<Int32>("-health", out health);

            Player player = sender as Player;
            int amount, offset = -1;
            if (args.Count > 5)
                throw new CommandError("Too many arguments");
            else if (sender is ConsoleSender && args.Count <= 2)
            {
                if (!Netplay.anyClients || !Tools.TryGetFirstOnlinePlayer(out player))
                    throw new CommandError("No players online.");
            }
            else if (args.Count == 3)
                player = args.GetOnlinePlayer(2);
            else if (args.Count >= 4)
            {
                player = args.GetOnlinePlayer(2);
                args.TryPopAny<Int32>("-item", out offset);
            }

            var npcName = args.GetString(1).ToLower().Trim();

            // Get the class id of the npc
            var npcs = DefinitionManager.FindNPC(npcName);
            if (npcs == null || npcs.Length == 0)
            {
                int npcId;
                if (Int32.TryParse(npcName, out npcId))
                {
                    npcs = DefinitionManager.FindNPC(npcId);
                    if (npcs == null || npcs.Length == 0)
                    {
                        throw new CommandError("No npc exists by type {0}", npcId);
                    }
                }
                else throw new CommandError("No npc exists {0}", npcName);
            }

            npcs = npcs.OrderBy(x => x.Name).ToArray();
            if (npcs.Length > 1)
            {
                if (offset == -1)
                {
                    sender.SendMessage("Npcs matching " + npcName + ':');
                    for (var x = 0; x < npcs.Length; x++)
                    {
                        if (sender is ConsoleSender)
                        {
                            sender.SendMessage($"\t{x}\t- {npcs[x].Name}");
                        }
                        else
                        {
                            sender.SendMessage($"{x} - {npcs[x].Name}");
                        }
                    }
                    return;
                }
            }
            else offset = 0;

            var npc = npcs[offset];
            if (npc.Boss.HasValue && npc.Boss == true)
                throw new CommandError("This NPC can only be summoned by the SPAWNBOSS command.");
            try
            {
                amount = args.GetInt(0);
                //if (NPCAmount > Program.properties.SpawnNPCMax && sender is Player)
                //{
                //    (sender as Player).Kick("Don't spawn that many.");
                //    return;
                //}
            }
            catch
            {
                throw new CommandError("Expected amount to spawn");
            }

            var max = Tools.AvailableNPCSlots; //Perhaps remove a few incase of spawns
            if (amount > max)
                throw new CommandError("Cannot spawn that many, available slots: {0}", max);

            string realNPCName = String.Empty;
            for (int i = 0; i < amount; i++)
            {
                Vector2 location = World.GetRandomClearTile(((int)player.position.X / 16), ((int)player.position.Y / 16), 100, 100, 50);
                int npcIndex = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), npc.Id, 0);

                //if (customHealth)
                //{
                //    Main.npc[npcIndex].life = health;
                //    Main.npc[npcIndex].lifeMax = health;
                //}
                Main.npc[npcIndex].netDefaults(npc.NetId);

                realNPCName = Main.npc[npcIndex].name;
            }
            Utils.NotifyAllOps("Spawned " + amount.ToString() + " of " + realNPCName + " [" + player.name + "]", true);
        }
    }
}

