using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;

namespace Terraria_Server.TDCM.Packets.Quests
{
    public static class QuestActions
    {
        /// <summary>
        /// Aimed to increase the spawn rate of NPC's when a player is in a certain Quest
        /// </summary>
        /// <param name="npcId"></param>
        public static void NPC_NPCSpawnHandler(int npcId)
        {
			return;
            /*var npc = Main.npcs[npcId];

            if (Main.rand == null)
                Main.rand = new Random((int)DateTime.Now.Ticks);

            CheckAndSpawn(QuestType.SLIME_QUEST, "Green Slime", npc);
            CheckAndSpawn(QuestType.ALCHEMIST_2, "Demon Eye", npc);*/
        }

        public static void CheckAndSpawn(QuestType Type, string Name, NPC Npc)
        {
            if (IsAnyUsingQuest(Type) && Main.rand.Next(0, 3) == 1)
                SpawnNPCByName(Name, Npc);
        }

        public static void SpawnNPCByName(string Name, NPC Npc)
        {
            Vector2 randLoc = World.GetRandomClearTile(
                ((int)Npc.Position.X / 16), ((int)Npc.Position.Y / 16), 100, true, 100, 10);

            NPC.NewNPC((int)randLoc.X * 16, (int)randLoc.Y * 16, Name, 0);
        }

        public static bool IsAnyUsingQuest(QuestType Type)
        {
            foreach (var player in Main.players.Where(x => x.CurrentQuest == (int)Type))
                return true;

            return false;
        }

        public static void Dark_Mage_3(Player player)
        {
            //int id;
            //if (NPC.TryFindNPCByName("Skeletron", out id))
            //    player.sendMessage("Skeletron already has been summoned, ");

            int reduceBy = 100;

            int players = Networking.ClientConnection.All.Count;
            if (players > 0)
                reduceBy -= players / 100;

            if (players >= 15) //If there are alot of players on, They can have the full health.
                reduceBy = 1;

            if (reduceBy < 1)
                reduceBy = 1;

            /*Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 75, 50);
            int SkeletronId = NPC.NewNPC(((int)location.X), ((int)location.Y * 16), (int)NPCType.N35_SKELETRON_HEAD);

            NPC npc = Main.npcs[SkeletronId];
            npc.life = npc.life / reduceBy;
            npc.lifeMax = npc.life;
            npc.damage = npc.damage / reduceBy;

            npc.target = player.whoAmi;
            npc.ai[3] = 1f;

            if (npc.life < 0)
                npc.life = 1;

            if (npc.lifeMax < 0)
                npc.lifeMax = 1;

            if (npc.damage < 0)
                npc.damage = 1;

            npc.netUpdate = true;
            Main.npcs[SkeletronId] = npc; */


            Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
            int SkeletronId = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), (int)NPCType.N35_SKELETRON_HEAD);

            NPC npc = Main.npcs[SkeletronId];
            npc.life = npc.life / reduceBy;
            npc.lifeMax = npc.life;
            npc.damage = npc.damage / reduceBy;

            npc.target = player.whoAmi;
            npc.ai[3] = 1f;

            if (npc.life < 1)
                npc.life = 1;

            if (npc.lifeMax < 1)
                npc.lifeMax = 1;

            if (npc.damage < 1)
                npc.damage = 1;

            npc.netUpdate = true;
            Main.npcs[SkeletronId] = npc;

            //Tell the client which NPC to look for.
            NetMessage.SendData(Packet.CLIENT_MOD_SPAWN_NPC, player.whoAmi, -1, "", SkeletronId);
        }

        public static void Dark_Mage_4(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 location = World.GetRandomClearTile(((int)player.Position.X / 16), ((int)player.Position.Y / 16), 100, true, 100, 50);
                int mHeadId = NPC.NewNPC(((int)location.X * 16), ((int)location.Y * 16), (int)NPCType.N23_METEOR_HEAD);

                //Main.npcs[mHeadId].damage = Main.npcs[mHeadId].damage / 2;
                //Main.npcs[mHeadId].target = player.whoAmi;

                NetMessage.SendData(Packet.CLIENT_MOD_SPAWN_NPC, player.whoAmi, -1, "", mHeadId);
            }
        }

        public static void Paladin_3(Player player)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 spawnPos = World.GetRandomClearTile((int)(player.Position.X / 16), (int)(player.Position.Y / 16), 100, true, 75, 50);

                int eId = NPC.NewNPC((int)spawnPos.X * 16, (int)spawnPos.Y * 16, "Little Eater");
                Main.npcs[eId].target = player.whoAmi;

                NetMessage.SendData(Packet.CLIENT_MOD_SPAWN_NPC, player.whoAmi, -1, "", eId);
            }
        }

        public static void Tinkerer_2(Player player)
        {
            Vector2 spawnPos = World.GetRandomClearTile((int)(player.Position.X / 16), (int)(player.Position.Y / 16), 100, true, 75, 50);

            int ZombieId = NPC.NewNPC((int)spawnPos.X * 16, (int)spawnPos.Y * 16, 3);

            Main.npcs[ZombieId].target = player.whoAmi;
            NetMessage.SendData(Packet.CLIENT_MOD_SPAWN_NPC, player.whoAmi, -1, "", ZombieId);
        }

        public static void Tinkerer_3(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnPos = World.GetRandomClearTile((int)(player.Position.X / 16), (int)(player.Position.Y / 16), 100, true, 75, 50);

                int zId = NPC.NewNPC((int)spawnPos.X * 16, (int)spawnPos.Y * 16, 3);
                Main.npcs[zId].target = player.whoAmi;

                NetMessage.SendData(Packet.CLIENT_MOD_SPAWN_NPC, player.whoAmi, -1, "", zId);
            }
        }

        public static void Tinkerer_4(Player player)
        {
            for (int i = 0; i < 35; i++)
            {
                Vector2 spawnPos = World.GetRandomClearTile((int)(player.Position.X / 16), (int)(player.Position.Y / 16), 100, true, 75, 50);

                int zId = NPC.NewNPC((int)spawnPos.X * 16, (int)spawnPos.Y * 16, 3);
                Main.npcs[zId].target = player.whoAmi;

                Main.npcs[zId].life = Main.npcs[zId].life / 2;
                Main.npcs[zId].lifeMax = Main.npcs[zId].life;

                NetMessage.SendData(Packet.CLIENT_MOD_SPAWN_NPC, player.whoAmi, -1, "", zId);
            }
        }
    }
}
