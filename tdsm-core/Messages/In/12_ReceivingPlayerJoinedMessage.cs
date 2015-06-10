using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ReceivingPlayerJoinedMessage : SlotMessageHandler
    {
        public ReceivingPlayerJoinedMessage()
        {
            ValidStates = SlotState.SENDING_TILES | SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.RECEIVING_PLAYER_JOINED;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.serverSock[whoAmI].Kick("Cheating detected (RECEIVING_PLAYER_JOINED forgery).");
                return;
            }

            playerIndex = whoAmI;
            num++;

            Player player = Main.player[playerIndex];

            //if (player.SpawnX >= 0 && player.SpawnY >= 0)
            //{
            //    player.OldSpawnX = player.SpawnX;
            //    player.OldSpawnY = player.SpawnY;
            //}

            player.SpawnX = ReadInt16(readBuffer);
            player.SpawnY = ReadInt16(readBuffer);
            player.Spawn();

            //ProgramLog.Debug.Log ("sx: {0}, sy: {1}, tx: {2}, ty: {3}", player.SpawnX, player.SpawnY, player.TeleSpawnX, player.TeleSpawnY);

            player.respawnTimer = Int32.MaxValue;

            if (Terraria.Netplay.serverSock[whoAmI].State() >= SlotState.SENDING_TILES)
            {
                if (Terraria.Netplay.serverSock[whoAmI].State() == SlotState.SENDING_TILES)
                {
                    Terraria.Netplay.serverSock[whoAmI].SetState(SlotState.PLAYING);
                    NewNetMessage.OnPlayerJoined(whoAmI); // this also forwards the message
                }
                else
                {
                    NewNetMessage.SendData(12, -1, whoAmI, String.Empty, whoAmI, 0f, 0f, 0f, 0);
                }
                NewNetMessage.SendData(74, whoAmI, -1, Main.player[whoAmI].name, Main.anglerQuest, 0f, 0f, 0f, 0);
            }

            Server.AddUniqueConnection(player.Name, player.IPAddress.Split(':')[0]);
        }
    }
}
