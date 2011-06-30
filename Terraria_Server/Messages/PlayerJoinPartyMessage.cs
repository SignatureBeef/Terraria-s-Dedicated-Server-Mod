using System;
using Terraria_Server.Plugin;
using Terraria_Server.Events;

namespace Terraria_Server.Messages
{
    public class PlayerJoinPartyMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_JOIN_PARTY;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int teamIndex = (int)readBuffer[num++];
            Player player = Main.player[playerIndex];
            int currentTeam = player.team;

            if (Main.netMode == 2)
            {
                NetMessage.SendData(45, -1, whoAmI, "", playerIndex);
                Party party = Party.NONE;
                string joinMessage = "";
                switch (teamIndex)
                {
                    case 0:
                        joinMessage = " is no longer on a party.";
                        break;
                    case 1:
                        joinMessage = " has joined the red party.";
                        party = Party.RED;
                        break;
                    case 2:
                        joinMessage = " has joined the green party.";
                        party = Party.GREEN;
                        break;
                    case 3:
                        joinMessage = " has joined the blue party.";
                        party = Party.BLUE;
                        break;
                    case 4:
                        joinMessage = " has joined the yellow party.";
                        party = Party.YELLOW;
                        break;
                }

                PartyChangeEvent changeEvent = new PartyChangeEvent();
                changeEvent.PartyType = party;
                changeEvent.Sender = Main.player[whoAmI];
                Program.server.getPluginManager().processHook(Hooks.PLAYER_PARTYCHANGE, changeEvent);
                if (changeEvent.Cancelled)
                {
                    return;
                }

                player.team = teamIndex;
                for (int i = 0; i < 255; i++)
                {
                    if (i == whoAmI
                        || (currentTeam > 0 && player.team == currentTeam)
                        || (teamIndex > 0 && player.team == teamIndex))
                    {
                        NetMessage.SendData(25, i, -1, player.name + joinMessage, 255, (float)Main.teamColor[teamIndex].R, (float)Main.teamColor[teamIndex].G, (float)Main.teamColor[teamIndex].B);
                    }
                }
            }
        }
    }
}
