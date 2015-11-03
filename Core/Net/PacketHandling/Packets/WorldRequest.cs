using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;
using TDSM.Core.Net.PacketHandling.Misc;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class WorldRequest : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.WORLD_REQUEST; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            //Check to see if the client is to send the player password
            //If they must, then we can ignore the world request until they respond.
            if (Netplay.Clients[bufferId].State == (int)ConnectionState.WaitingForUserPassword)
                return true;
            
            return false;
        }
    }
}

