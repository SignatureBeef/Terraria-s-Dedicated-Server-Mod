using System;
using Terraria_Server.Plugin;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class ClientModMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.CLIENT_MOD;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            var player = Main.players[whoAmI];
            player.HasClientMod = true;
            ProgramLog.Log(player.Name + " has logged in with the TDCM Client");
        }
    }
}
