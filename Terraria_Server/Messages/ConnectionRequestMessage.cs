using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class ConnectionRequestMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.CONNECTION_REQUEST;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            ServerSlot slot = Netplay.slots[whoAmI];
            PlayerLoginEvent loginEvent = new PlayerLoginEvent();
            loginEvent.Slot = slot;
            loginEvent.Sender = Main.players[whoAmI];
            Program.server.PluginManager.processHook(Plugin.Hooks.PLAYER_PRELOGIN, loginEvent);
            if ((loginEvent.Cancelled || loginEvent.Action == PlayerLoginAction.REJECT) && (slot.state & SlotState.DISCONNECTING) == 0)
            {
                slot.Kick ("Disconnected by server.");
                return;
            }

            String clientName = slot.remoteAddress.Split(':')[0];
//
//            if (Program.server.BanList.containsException(clientName))
//            {
//                slot.Kick ("You are banned from this Server.");
//                return;
//            }

            if (Program.properties.UseWhiteList && !Program.server.WhiteList.containsException(clientName))
            {
                slot.Kick ("You are not on the WhiteList.");
                return;
            }

            if (slot.state == SlotState.CONNECTED)
            {
                String version = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
                if (!(version == "Terraria" + Statics.CURRENT_TERRARIA_RELEASE))
                {
                    if (version.Length > 30) version = version.Substring (0, 30);
                    ProgramLog.Debug.Log ("Client version string: {0}", version);
                    slot.Kick ("You are not using the same Terraria version as this server.");
                    return;
                }

                if (Netplay.password == null || Netplay.password == "")
                {
                    slot.state = SlotState.ACCEPTED;
                    NetMessage.SendData(3, whoAmI);
                    return;
                }

                slot.state = SlotState.SERVER_AUTH;
                NetMessage.SendData(37, whoAmI);
            }
        }
    }
}
