using System;
using System.Linq;
using Terraria;
using OTA;
using OTA.Command;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TDSM.Core.Misc;
using OTA.Extensions;

namespace TDSM.Core.Command.Commands
{
    public class SpawnBossCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("spawnboss")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Spawn a boss")
                .WithHelpText("<amount> <boss> <player>")
                .WithHelpText("(If no player is entered it will be a random online player)")
                .WithPermissionNode("tdsm.spawnboss")
                .Calls(this.SummonBoss);
        }

        enum WorldZone
        {
            Any,
            ZoneCorrupt,
            ZoneCrimson,
            ZoneDesert,
            ZoneDungeon,
            ZoneGlowshroom,
            ZoneHoly,
            ZoneJungle,
            ZoneMeteor,
            ZonePeaceCandle,
            ZoneSnow,
            ZoneTowerNebula,
            ZoneTowerSolar,
            ZoneTowerStardust,
            ZoneTowerVortex,
            ZoneUndergroundDesert,
            ZoneWaterCandle
        }

        static Player FindPlayerWithOptions(WorldZone options)
        {
            switch (options)
            {
                case WorldZone.ZoneCorrupt:
                    return Main.player.Where(x => x.active && x.ZoneCorrupt).Random();
                case WorldZone.ZoneCrimson:
                    return Main.player.Where(x => x.active && x.ZoneCrimson).Random();
                case WorldZone.ZoneDesert:
                    return Main.player.Where(x => x.active && x.ZoneDesert).Random();
                case WorldZone.ZoneDungeon:
                    return Main.player.Where(x => x.active && x.ZoneDungeon).Random();
                case WorldZone.ZoneGlowshroom:
                    return Main.player.Where(x => x.active && x.ZoneGlowshroom).Random();
                case WorldZone.ZoneHoly:
                    return Main.player.Where(x => x.active && x.ZoneHoly).Random();
                case WorldZone.ZoneJungle:
                    return Main.player.Where(x => x.active && x.ZoneJungle).Random();
                case WorldZone.ZoneMeteor:
                    return Main.player.Where(x => x.active && x.ZoneMeteor).Random();
                case WorldZone.ZonePeaceCandle:
                    return Main.player.Where(x => x.active && x.ZonePeaceCandle).Random();
                case WorldZone.ZoneSnow:
                    return Main.player.Where(x => x.active && x.ZoneSnow).Random();
                case WorldZone.ZoneTowerNebula:
                    return Main.player.Where(x => x.active && x.ZoneTowerNebula).Random();
                case WorldZone.ZoneTowerSolar:
                    return Main.player.Where(x => x.active && x.ZoneTowerSolar).Random();
                case WorldZone.ZoneTowerStardust:
                    return Main.player.Where(x => x.active && x.ZoneTowerStardust).Random();
                case WorldZone.ZoneTowerVortex:
                    return Main.player.Where(x => x.active && x.ZoneTowerVortex).Random();
                case WorldZone.ZoneUndergroundDesert:
                    return Main.player.Where(x => x.active && x.ZoneUndergroundDesert).Random();
                case WorldZone.ZoneWaterCandle:
                    return Main.player.Where(x => x.active && x.ZoneWaterCandle).Random();
                default:
                    return Main.player.Where(x => x.active).Random();
            }
        }

        struct Boss
        {
            /// <summary>
            /// Display name for the notification (null fto use default)
            /// </summary>
            public string name;

            /// <summary>
            /// The typeid of the boss
            /// </summary>
            public int type;

            /// <summary>
            /// Ignore notifying of the spawn
            /// </summary>
            internal bool ignore;
        }

        /// <summary>
        /// Summon a Boss
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void SummonBoss(ISender sender, ArgumentList args)
        {
            var count = args.GetInt(0);
            var bossName = args.GetString(1).ToLower();
            Player target;

            if (!args.TryGetOnlinePlayer(2, out target))
            {
                if (sender is Player)
                {
                    target = sender as Player;
                }
                else
                {
                    target = Main.player.Where(x => x.active).Random();
                    if (null == target)
                    {
                        throw new CommandError("No players online");
                    }
                }
            }

            //            int type = -1, type1 = -1;
            //            string name = null;
            var queue = new Queue<Boss>();

            switch (bossName)
            {
                case "wyvern":
                    //                    type = 87;
                    queue.Enqueue(new Boss()
                        {
                            type = 87
                        });
                    break;

                case "brain":
                case "brain of cthulhu":
                    //                    type = 266;
                    queue.Enqueue(new Boss()
                        {
                            type = 266
                        });
                    break;

            //                case "crimson mimic":
            //                    type = 474;
            //                    break;
                case "corrupt mimic":
                    //                    type = 473;
                    queue.Enqueue(new Boss()
                        {
                            type = 473
                        });
                    break;
            //                case "hallowed mimic":
            //                    type = 475;
            //                    break;

                case "duke fishron":
                case "duke":
                case "fishron":
                    //                    type = 370;
                    queue.Enqueue(new Boss()
                        {
                            type = 370
                        });
                    break;

                case "everscream":
                    World.SetTime(16200.0, false);
                    //                    type = 344;
                    queue.Enqueue(new Boss()
                        {
                            type = 344
                        });
                    break;

                case "eye":
                case "cthulhu":
                case "eye of cthulhu":
                    World.SetTime(16200.0, false);
                    //                    type = 4;
                    queue.Enqueue(new Boss()
                        {
                            type = 4
                        });
                    break;

                case "dutchman":
                case "flying dutchman":
                    //                    type = 491;
                    queue.Enqueue(new Boss()
                        {
                            type = 491
                        });
                    break;

                case "golem":
                    //                    type = 245;
                    queue.Enqueue(new Boss()
                        {
                            type = 245
                        });
                    break;

                case "goblin summoner":
                    //                    type = 471;
                    queue.Enqueue(new Boss()
                        {
                            type = 471
                        });
                    break;

                case "king":
                case "king slime":
                    //                    type = 50;
                    queue.Enqueue(new Boss()
                        {
                            type = 50
                        });
                    break;

                case "ice golem":
                    //                    type = 243;
                    queue.Enqueue(new Boss()
                        {
                            type = 243
                        });
                    break;

                case "ice queen":
                    World.SetTime(16200.0, false);
                    //                    type = 345;
                    queue.Enqueue(new Boss()
                        {
                            type = 345
                        });
                    break;

                case "lunatic":
                case "cultist":
                case "lunatic cultist":
                    //                    type = 439;
                    queue.Enqueue(new Boss()
                        {
                            type = 439
                        });
                    break;

                case "saucer":
                case "martian saucer":
                    //                    type = 395;
                    queue.Enqueue(new Boss()
                        {
                            type = 395
                        });
                    break;

                case "moon":
                case "moon lord":
                    //                    type = 398;
                    queue.Enqueue(new Boss()
                        {
                            type = 398
                        });
                    break;

                case "mothron":
                    if (!Main.eclipse)
                        throw new CommandError("Mothron can only be spawned during a solar eclipse. See the worldevent command.");
                    //                    type = 477;
                    queue.Enqueue(new Boss()
                        {
                            type = 477
                        });
                    break;

                case "wood":
                case "mourning wood":
                    World.SetTime(16200.0, false);
                    //                    type = 325;
                    queue.Enqueue(new Boss()
                        {
                            type = 325
                        });
                    break;

                case "paladin":
                    //                    type = 290;
                    queue.Enqueue(new Boss()
                        {
                            type = 290
                        });
                    break;

                case "captain":
                case "pirate":
                case "pirate captain":
                    World.SetTime(16200.0, false);
                    //                    type = 216;
                    queue.Enqueue(new Boss()
                        {
                            type = 216
                        });
                    break;

                case "plantera":
                    //                    type = 262;
                    queue.Enqueue(new Boss()
                        {
                            type = 262
                        });
                    break;

                case "pumpking":
                    World.SetTime(16200.0, false);
                    //                    type = 327;
                    queue.Enqueue(new Boss()
                        {
                            type = 327
                        });
                    break;

                case "queen":
                case "queen bee":
                    //                    type = 222;
                    queue.Enqueue(new Boss()
                        {
                            type = 222
                        });
                    break;

                case "santa":
                case "santa nk1":
                case "santa-nk1":
                    World.SetTime(16200.0, false);
                    //                    type = 346;
                    queue.Enqueue(new Boss()
                        {
                            type = 346
                        });
                    break;

                case "skeletron":
                    World.SetTime(16200.0, false);
                    //                    type = 35;
                    queue.Enqueue(new Boss()
                        {
                            type = 35
                        });
                    break;

                case "prime":
                case "skeletron prime":
                    //                    type = 127;
                    queue.Enqueue(new Boss()
                        {
                            type = 127
                        });
                    break;

                case "nebula":
                case "nebula pillar":
                    //                    type = 507;
                    queue.Enqueue(new Boss()
                        {
                            type = 507
                        });
                    break;
                case "solar":
                case "solar pillar":
                    //                    type = 517;
                    queue.Enqueue(new Boss()
                        {
                            type = 517
                        });
                    break;
                case "stardust":
                case "stardust pillar":
                    //                    type = 493;
                    queue.Enqueue(new Boss()
                        {
                            type = 493
                        });
                    break;
                case "vortex":
                case "vortex pillar":
                    //                    type = 422;
                    queue.Enqueue(new Boss()
                        {
                            type = 422
                        });
                    break;

                case "destroyer":
                case "the destroyer":
                    World.SetTime(16200.0, false);
                    //                    type = 134;
                    queue.Enqueue(new Boss()
                        {
                            type = 134
                        });
                    break;

                case "twins":
                case "the twins":
                    World.SetTime(16200.0, false);
                    //                    type = 125;
                    //                    type1 = 126;
                    queue.Enqueue(new Boss()
                        {
                            type = 125,
                            name = "The Twins"
                        });
                    queue.Enqueue(new Boss()
                        {
                            type = 126,
                            ignore = true
                        });
                    break;

                case "eater":
                case "eater of worlds":
                    //                    type = 13;
                    queue.Enqueue(new Boss()
                        {
                            type = 13
                        });
                    break;

                case "wall":
                case "flesh":
                case "wall of flesh":
                    if (Main.wof > 0 && Main.npc[Main.wof].active)
                        throw new CommandError("The Wall Of Flesh is already active");

                    if (target.position.Y / 16 < (float)(Main.maxTilesY - 205)) //As per NPC.SpawnWOF
                        throw new CommandError("Player must be in The Underworld to spawn the Eater Of Worlds");

                    //                    type = 113;
                    queue.Enqueue(new Boss()
                        {
                            type = 113
                        });
                    break;

                case "deathcradle":
                    count = 1;
                    var items = new int[]
                    {
                        87, 266, 473, 
                        370, 344, 4, 
                        491, 245, 471, 
                        50, 243, 345, 
                        439, 395, 398, 
                        477, 325, 290, 
                        216, 262, 327, 
                        222, 346, 35, 
                        127, 507, 517,
                        493, 422, 134,
                        125, 126, 13
                    };
                    World.SetTime(16200.0, false);

                    foreach (var item in items)
                    {
                        queue.Enqueue(new Boss()
                            {
                                type = item,
                                ignore = true
                            });
                    }

                    Core._likeABoss = true;
                    Tools.NotifyAllPlayers("Easter egg found: Like a boss mini game!", Color.Purple, true);

                    break;
                default:
                    throw new CommandError("Unknown boss: " + bossName);
            }

            while (count-- > 0)
            {
                while (queue.Count > 0)
                {
                    var boss = queue.Dequeue();
                    var position = World.GetRandomClearTile(target.position.X / 16f, target.position.Y / 16f);
                    var id = NPC.NewNPC((int)(position.X * 16f), (int)(position.Y * 16f), boss.type);

                    Main.npc[id].SetDefaults(boss.type);
                    Main.npc[id].SetDefaults(Main.npc[id].name);

                    if (count == 0 && !boss.ignore)
                    {
                        var tms = String.Empty;
                        if (count > 1)
                            tms = " " + count + " times";
                        Tools.NotifyAllPlayers((boss.name ?? Main.npc[id].name) + " [" + boss.type + "]" + " summoned by " + sender.SenderName + tms, Color.Purple, true);
                    }
                }
            }
        }
    }
}

