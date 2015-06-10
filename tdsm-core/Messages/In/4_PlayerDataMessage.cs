using Microsoft.Xna.Framework;
using System;
using System.Text;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Logging;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    class PlayerDataMessage : MessageHandler
    {
        public PlayerDataMessage()
        {
            //IgnoredStates |= SlotState.ASSIGNING_SLOT;
            ValidStates = SlotState.ACCEPTED | SlotState.ASSIGNING_SLOT |
                /* this so that we can have a custom error message */
                SlotState.SENDING_WORLD | SlotState.SENDING_TILES | SlotState.PLAYING;
        }

        private const int MAX_HAIR_ID = 36;

        public override Packet GetPacket()
        {
            return Packet.PLAYER_DATA;
        }

        static string GetName(ClientConnection conn, byte[] readBuffer, int num, int len)
        {
            string name;

            try
            {
                name = Encoding.ASCII.GetString(readBuffer, num, len).Trim();
            }
            catch (ArgumentException)
            {
                conn.Kick("Invalid name: contains non-ASCII characters.");
                return null;
            }

            if (name.Length > 20)
            {
                conn.Kick("Invalid name: longer than 20 characters.");
                return null;
            }

            if (name == String.Empty)
            {
                conn.Kick("Invalid name: whitespace or empty.");
                return null;
            }

            foreach (char c in name)
            {
                if (c < 32 || c > 126)
                {
                    conn.Kick("Invalid name: contains non-printable characters.");
                    return null;
                }
            }

            if (name.Contains(" " + " "))
            {
                conn.Kick("Invalid name: contains double spaces.");
                return null;
            }

            return name;
        }

        public override void Process(ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;

            if (conn.State == SlotState.ASSIGNING_SLOT)
            {
                // TODO: verify that data didn't change.
                int who = conn.SlotIndex;
                NewNetMessage.SendData(4, -1, who, conn.Player.Name, who);
                return;
            }

            //if (conn.Player != null)
            //{
            //    conn.Kick("Player data sent twice.");
            //    return;
            //}

            //var player = new Player();
            var player = conn.Player;
            var connecting = conn.Player == null;
            if (connecting)
            {
                player = new Player();
                conn.Player = player;
                player.Connection = conn;
                player.IPAddress = conn.RemoteAddress;
                player.whoAmi = conn.SlotIndex;
            }

            var data = new HookArgs.PlayerDataReceived()
            {
                IsConnecting = connecting
            };

            var read = data.Parse(readBuffer, num + 1 /* PlayerId */, length);
            Skip(read);

            if (data.Hair >= MAX_HAIR_ID)
            {
                data.Hair = 0;
            }

            var ctx = new HookContext
            {
                Connection = conn,
                Player = player,
                Sender = player,
            };

            HookPoints.PlayerDataReceived.Invoke(ref ctx, ref data);

            if (ctx.CheckForKick())
                return;

            if (!data.NameChecked)
            {
                string error;
                if (!data.CheckName(out error))
                {
                    conn.Kick(error);
                    return;
                }
            }

            string address = conn.RemoteAddress.Split(':')[0];
            if (!data.BansChecked)
            {
                if (Server.Bans.Contains(address) || Server.Bans.Contains(data.Name))
                {
                    ProgramLog.Admin.Log("Prevented banned user {0} from accessing the server.", data.Name);
                    conn.Kick("You are banned from this server.");
                    return;
                }
            }

            if (!data.WhitelistChecked && Server.WhitelistEnabled)
            {
                if (!Server.Whitelist.Contains(address) && !Server.Whitelist.Contains(data.Name))
                {
                    ProgramLog.Admin.Log("Prevented non whitelisted user {0} from accessing the server.", data.Name);
                    conn.Kick("You do not have access to this server.");
                    return;
                }
            }

            data.Apply(player);

            if (connecting)
                if (ctx.Result == HookResult.ASK_PASS)
                {
                    conn.State = SlotState.PLAYER_AUTH;

                    var msg = NewNetMessage.PrepareThreadInstance();
                    msg.PasswordRequest();
                    conn.Send(msg.Output);

                    return;
                }
                else // HookResult.DEFAULT
                {
                    // don't allow replacing connections for guests, but do for registered users
                    if (conn.State < SlotState.PLAYING)
                    {
                        var lname = player.Name.ToLower();

                        foreach (var otherPlayer in Main.player)
                        {
                            var otherSlot = Terraria.Netplay.serverSock[otherPlayer.whoAmi];
                            if (otherPlayer.Name != null && lname == otherPlayer.Name.ToLower() && otherSlot.State() >= SlotState.CONNECTED)
                            {
                                conn.Kick("A \"" + otherPlayer.Name + "\" is already on this server.");
                                return;
                            }
                        }
                    }

                    //conn.Queue = (int)loginEvent.Priority; // actual queueing done on world request message

                    // and now decide whether to queue the connection
                    //SlotManager.Schedule (conn, (int)loginEvent.Priority);

                    //NewNetMessage.SendData (4, -1, -1, player.Name, whoAmI);
                }




            //int num3 = (int)ReadByte(readBuffer);
            //if (Main.netMode == 2)
            //{
            //    num3 = this.whoAmI;
            //}
            //if (num3 == Main.myPlayer && !Main.ServerSideCharacter)
            //{
            //    return;
            //}
            //Player player2 = Main.player[num3];
            //player2.whoAmi = num3;
            //if (ReadByte() == 0)
            //{
            //    player2.male = true;
            //}
            //else
            //{
            //    player2.male = false;
            //}
            //player2.hair = (int)ReadByte(readBuffer);
            //if (player2.hair >= 123)
            //{
            //    player2.hair = 0;
            //}
            //player2.name = ReadString().Trim();
            //player2.hairDye = ReadByte(readBuffer);
            //player2.hideVisual = ReadByte(readBuffer);
            //player2.hairColor = ReadRGB();
            //player2.skinColor = ReadRGB();
            //player2.eyeColor = ReadRGB();
            //player2.shirtColor = ReadRGB();
            //player2.underShirtColor = ReadRGB();
            //player2.pantsColor = ReadRGB();
            //player2.shoeColor = ReadRGB();
            //player2.difficulty = ReadByte(readBuffer);
            //if (Main.netMode != 2)
            //{
            //    return;
            //}
            //bool flag = false;
            //if (tdsm.api.Callbacks.Netplay.slots[this.whoAmI].state < 10)
            //{
            //    for (int n = 0; n < 255; n++)
            //    {
            //        if (n != num3 && player2.name == Main.player[n].name && tdsm.api.Callbacks.Netplay.slots[n].active)
            //        {
            //            flag = true;
            //        }
            //    }
            //}
            //if (flag)
            //{
            //    NewNetMessage.SendData(2, this.whoAmI, -1, player2.name + " " + Lang.mp[5], 0, 0f, 0f, 0f, 0);
            //    return;
            //}
            //if (player2.name.Length > Player.nameLen)
            //{
            //    NewNetMessage.SendData(2, this.whoAmI, -1, "Name is too long.", 0, 0f, 0f, 0f, 0);
            //    return;
            //}
            //if (player2.name == String.Empty)
            //{
            //    NewNetMessage.SendData(2, this.whoAmI, -1, "Empty name.", 0, 0f, 0f, 0f, 0);
            //    return;
            //}
            //tdsm.api.Callbacks.Netplay.slots[this.whoAmI].oldName = player2.name;
            //tdsm.api.Callbacks.Netplay.slots[this.whoAmI].name = player2.name;
            //NewNetMessage.SendData(4, -1, this.whoAmI, player2.name, num3, 0f, 0f, 0f, 0);
            //return;
        }


        private int setColor(Color color, int bufferPos, byte[] readBuffer)
        {
            color.R = readBuffer[bufferPos++];
            color.G = readBuffer[bufferPos++];
            color.B = readBuffer[bufferPos++];
            return bufferPos;
        }

    }
}
