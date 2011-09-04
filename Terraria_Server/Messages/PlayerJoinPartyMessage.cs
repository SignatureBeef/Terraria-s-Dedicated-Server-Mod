using System;
using Terraria_Server.Plugin;
using Terraria_Server.Events;

namespace Terraria_Server.Messages
{
    public class PlayerJoinPartyMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_JOIN_PARTY;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (PLAYER_JOIN_PARTY forgery).");
                return;
            }
            
            playerIndex = whoAmI;

            int teamIndex = (int)readBuffer[num + 1];
            Player player = Main.players[playerIndex];
            int currentTeam = player.team;
            
            Party party = Party.NONE;
            string joinMessage = "";
            switch (teamIndex)
            {
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
                default:
                    joinMessage = " is no longer in a party.";
                    break;
            }

            PartyChangeEvent changeEvent = new PartyChangeEvent();
            changeEvent.PartyType = party;
            changeEvent.Sender = Main.players[whoAmI];
            Program.server.PluginManager.processHook(Hooks.PLAYER_PARTYCHANGE, changeEvent);
            if (changeEvent.Cancelled)
            {
                return;
            }

            player.team = teamIndex;
            
            NetMessage.SendData(45, -1, whoAmI, "", playerIndex);
            
            for (int i = 0; i < 255; i++)
            {
                if (i == whoAmI
                    || (currentTeam > 0 && player.team == currentTeam)
                    || (teamIndex > 0 && player.team == teamIndex))
                {
                    NetMessage.SendData(25, i, -1, player.Name + joinMessage, 255, (float)Main.teamColor[teamIndex].R, (float)Main.teamColor[teamIndex].G, (float)Main.teamColor[teamIndex].B);
                }
            }
        }
    }
}
