using System;
using System.Collections.Generic;
using System.Linq;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public struct SentMessage
    {
        public DateTime Time { get; set; }
        public int Type { get; set; }
    }

    public class SpawnNPCs : SpammableMessage<Int32, SentMessage>
    {
        public const Int32 MIN_WAIT = 10;

        public override Packet GetPacket()
        {
            return Packet.SPAWN_NPCS;
        }

        public override IEnumerable<Record<Int32, SentMessage>> GetRemovable
        {
            get
            {
                return from x in Register where (DateTime.Now - x.Val.Time).TotalSeconds > MIN_WAIT select x;
            }
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //int plr = BitConverter.ToInt32(readBuffer, num);
            //num += 4;
            //int typeOrInvasion = BitConverter.ToInt32(readBuffer, num);

            //var player = Main.player[whoAmI];

            //if (plr != whoAmI)
            //{
            //    player.Kick("SpawnNPC Player Forgery!");
            //    return;
            //}

            //SentMessage[] records;
            //if (Register.GetAllResults(plr, out records))
            //    foreach (var record in records)
            //    {
            //        if (record.Type == typeOrInvasion)
            //        {
            //            /* Checks if out of time, While also, if npc's are purged, check if are summoned, or is a Invasion. */

            //            if ((DateTime.Now - record.Time).TotalSeconds <= MIN_WAIT && (NPC.IsNPCSummoned(typeOrInvasion) ||
            //                (typeOrInvasion == -1 || typeOrInvasion == -2 && Main.IsInvasionOccurring(typeOrInvasion, true))))
            //            {
            //                player.Kick("SpawnNPC packet abuse!");
            //                return;
            //            }
            //        }
            //    }

            //Purge();
            //Add(plr, new SentMessage()
            //{
            //    Time = DateTime.Now,
            //    Type = typeOrInvasion
            //});

            //if (typeOrInvasion == (int)NPCType.N04_EYE_OF_CTHULHU ||
            //    typeOrInvasion == (int)NPCType.N13_EATER_OF_WORLDS_HEAD ||
            //    typeOrInvasion == (int)NPCType.N50_KING_SLIME ||
            //    typeOrInvasion == (int)NPCType.N125_RETINAZER ||
            //    typeOrInvasion == (int)NPCType.N126_SPAZMATISM ||
            //    typeOrInvasion == (int)NPCType.N134_THE_DESTROYER ||
            //    typeOrInvasion == (int)NPCType.N127_SKELETRON_PRIME ||
            //    typeOrInvasion == (int)NPCType.N128_PRIME_CANNON)
            //{
            //    if (!NPC.IsNPCSummoned(typeOrInvasion))
            //        NPC.SpawnOnPlayer(plr, typeOrInvasion);
            //}
            //else
            //{
            //    if (typeOrInvasion >= 0)
            //    {
            //        player.Kick("Attempt to summon an unsupported NPC.");
            //        return;
            //    }

            //    int invasionType = typeOrInvasion;

            //    if (typeOrInvasion == -1 || typeOrInvasion == -2)
            //        invasionType *= -1;
            //    else
            //    {
            //        player.Kick("Attempt to invoke an unknown invasion.");
            //        return;
            //    }

            //    if (Main.invasionType == 0)
            //    {
            //        if (invasionType > 0)
            //        {
            //            Main.invasionDelay = 0;
            //            Main.StartInvasion((InvasionType)invasionType);
            //        }
            //    }
            //    else
            //        player.sendMessage("Please wait until the current invasion has been defeated.", ChatColor.Purple);
            //}







            int plr = ReadInt32(readBuffer);

			if (plr != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.serverSock[whoAmI].Kick("SpawnNPC Player Forgery!");
                return;
            }

            int num140 = ReadInt32(readBuffer);
            if (Main.netMode != 2)
            {
                return;
            }
            if (num140 == 4 || num140 == 13 || num140 == 50 || num140 == 125 || num140 == 126 || num140 == 134 || num140 == 127 || num140 == 128 || num140 == 222 || num140 == 245 || num140 == 266 || num140 == 370)
            {
                bool flag12 = !NPC.AnyNPCs(num140);
                if (flag12)
                {
                    NPC.SpawnOnPlayer(plr, num140);
                    return;
                }
                return;
            }
            else
            {
                if (num140 == -4)
                {
                    if (!Main.dayTime)
                    {
                        NewNetMessage.SendData(25, -1, -1, Lang.misc[31], 255, 50f, 255f, 130f, 0);
                        Main.startPumpkinMoon();
                        NewNetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);
                        return;
                    }
                    return;
                }
                else
                {
                    if (num140 == -5)
                    {
                        if (!Main.dayTime)
                        {
                            NewNetMessage.SendData(25, -1, -1, Lang.misc[34], 255, 50f, 255f, 130f, 0);
                            Main.startSnowMoon();
                            NewNetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);
                            return;
                        }
                        return;
                    }
                    else
                    {
                        if (num140 >= 0)
                        {
                            return;
                        }
                        int num141 = 1;
                        if (num140 > -4)
                        {
                            num141 = -num140;
                        }
                        if (num141 > 0 && Main.invasionType == 0)
                        {
                            Main.invasionDelay = 0;
                            Main.StartInvasion(num141);
                            return;
                        }
                        return;
                    }
                }
            }
        }
    }
}
