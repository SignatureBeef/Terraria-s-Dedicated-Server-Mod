using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class ClientUUId : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.CLIENT_UUID; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            //Not sure why Re-Logic doesn't just do this in the first place.
            if (Main.netMode != 2)
                return true;
            
            if (String.IsNullOrEmpty(Main.player[bufferId].ClientUUId))
            {
                var buffer = NetMessage.buffer[bufferId];
                Main.player[bufferId].ClientUUId = buffer.reader.ReadString();
            }
            return true;
        }
    }
}

