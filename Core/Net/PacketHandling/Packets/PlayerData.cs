using OTA;
using Terraria;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class PlayerData : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.PLAYER_DATA; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            var isConnection = player == null || player.Connection == null || !player.active;
            if (isConnection)
            {
                //player = new Player();
            }
            player.whoAmI = bufferId;
            player.IPAddress = Netplay.Clients[bufferId].Socket.GetRemoteAddress().GetIdentifier();
            
            if (bufferId == Main.myPlayer && !Main.ServerSideCharacter)
            {
                return true;
            }

            var data = new TDSMHookArgs.PlayerDataReceived()
            {
                IsConnecting = isConnection
            };
            
            data.Parse(buffer.reader, start, length);
            //            Skip(read);
            //
            //            if (data.Hair >= MAX_HAIR_ID)
            //            {
            //                data.Hair = 0;
            //            }
            //
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player,
            };
            //
            TDSMHookPoints.PlayerDataReceived.Invoke(ref ctx, ref data);
            //
            if (ctx.CheckForKick())
                return true;
            
            if (!data.NameChecked)
            {
                string error;
                if (!data.CheckName(out error))
                {
                    player.Connection.Kick(error);
                    return true;
                }
            }
            
            //            string address = player.IPAddress.Split(':')[0];
            //            if (!data.BansChecked)
            //            {
            //                if (Server.Bans.Contains(address) || Server.Bans.Contains(data.Name))
            //                {
            //                    ProgramLog.Admin.Log("Prevented banned user {0} from accessing the server.", data.Name);
            //                    conn.Kick("You are banned from this server.");
            //                    return;
            //                }
            //            }
            //
            //            if (!data.WhitelistChecked && Server.WhitelistEnabled)
            //            {
            //                if (!Server.Whitelist.Contains(address) && !Server.Whitelist.Contains(data.Name))
            //                {
            //                    ProgramLog.Admin.Log("Prevented non whitelisted user {0} from accessing the server.", data.Name);
            //                    conn.Kick("You do not have access to this server.");
            //                    return;
            //                }
            //            }
            
            data.Apply(player);
            
            {
                var lname = player.name.ToLower();
            
                foreach (var otherPlayer in Main.player)
                {
                    //                            var otherSlot = Terraria.Netplay.Clients[otherPlayer.whoAmI];
                    if (otherPlayer.name != null && lname == otherPlayer.name.ToLower() && otherPlayer.whoAmI != bufferId) // && otherSlot.State >= SlotState.CONNECTED)
                    {
                        player.Kick("A \"" + otherPlayer.name + "\" is already on this server.");
                        return true;
                    }
                }
            }
            
            if (isConnection)
            {
                if (ctx.Result == HookResult.ASK_PASS)
                {
                    Netplay.Clients[bufferId].State = -2;
                    //                    conn.State = SlotState.PLAYER_AUTH;
            
            
                    //                    var msg = NewNetMessage.PrepareThreadInstance();
                    //                    msg.PasswordRequest();
                    //                    conn.Send(msg.Output);
                    NetMessage.SendData((int)Packet.PASSWORD_REQUEST, bufferId);

                    return true;
                }
                else // HookResult.DEFAULT
                {
                    // don't allow replacing connections for guests, but do for registered users
                    //                    if (conn.State < SlotState.PLAYING)
            
                    //conn.Queue = (int)loginEvent.Priority; // actual queueing done on world request message
            
                    // and now decide whether to queue the connection
                    //SlotManager.Schedule (conn, (int)loginEvent.Priority);
            
                    //if (Netplay.Clients[bufferId].State == -2)
                    //{
                    //    //Netplay.Clients[bufferId].State = 1;
                    //    ProgramLog.Error.Log("User password accepted, send world");
                    //}
                    //else
                    //{
                    //Netplay.Clients[bufferId].State = 1;
                    //NetMessage.SendData((int)Packet.CONNECTION_RESPONSE, bufferId, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
                    //}
                }
            }
            
            if (player.name.Length > Player.nameLen)
            {
                NetMessage.SendData(2, bufferId, -1, "Name is too long.");
                return true;
            }
            if (player.name == "")
            {
                NetMessage.SendData(2, bufferId, -1, "Empty name.");
                return true;
            }
            
            Netplay.Clients[bufferId].Name = player.name;

            OTA.Callbacks.VanillaHooks.OnPlayerEntering(player);

            return true;
        }
    }
}

