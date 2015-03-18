using System;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;
using Terraria_Server.Messages;

namespace Terraria_Server.TDCM.Packets
{
    /*
     * TDCM Will be using authentication upon release.
     *      Such credentials 'should' be saved and needed to be delt with once unless the users is to change them.
     *      When they first download it will require them to sign up, registration will be sotred within a SQL server,
     *      which will do all functionality locally, sending byte sized packets (preferably an enumerated value) as results (As in beta).
     *      TDSM will then need to check whether they are in session to allow them RPG.
     *      
     *      ** The session detection/hack prevention will need to be thought out more thoroughly.
     * 
     */

    public class ClientModMessage : SlotMessageHandler
    {
		public ClientModMessage ()
		{
			ValidStates = SlotState.SENDING_TILES | SlotState.PLAYING;
		}
		
        public override Packet GetPacket()
        {
            return Packet.CLIENT_MOD;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            var player = Main.players[whoAmI];
            player.HasClientMod = true;
            ProgramLog.Log(player.Name + " has logged in with the TDCM Client");
            
            //Update Client with server data.
            NetMessage.SendData(Packet.CLIENT_MOD, whoAmI);
        }
    }
}
