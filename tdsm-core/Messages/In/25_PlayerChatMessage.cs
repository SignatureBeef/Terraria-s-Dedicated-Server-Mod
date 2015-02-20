using Microsoft.Xna.Framework;
using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Logging;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerChatMessage : MessageHandler
    {
        public PlayerChatMessage()
        {
            ValidStates = SlotState.CONNECTED | SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.PLAYER_CHAT;
        }

        public override void Process(ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            //TODO string validation

            ReadByte(readBuffer);

            var whoAmI = conn.SlotIndex;

            Color color = ReadRGB(readBuffer);
            if (Main.netMode == 2)
            {
                color = new Color(255, 255, 255);
            }
            string text = ReadString(readBuffer);
            string text2 = text.ToLower();
            if (text2 == Lang.mp[6] || text2 == Lang.mp[21])
            {
                string text3 = String.Empty;
                for (int num62 = 0; num62 < 255; num62++)
                {
                    if (Main.player[num62].active)
                    {
                        if (text3 == String.Empty)
                        {
                            text3 = Main.player[num62].name;
                        }
                        else
                        {
                            text3 = text3 + ", " + Main.player[num62].name;
                        }
                    }
                }
                NewNetMessage.SendData(25, whoAmI, -1, Lang.mp[7] + " " + text3 + ".", 255, 255f, 240f, 20f, 0);
                return;
            }
            if (text2.StartsWith("/me "))
            {
                NewNetMessage.SendData(25, -1, -1, "*" + Main.player[whoAmI].name + " " + text.Substring(4), 255, 200f, 100f, 0f, 0);
                return;
            }
            if (text2 == Lang.mp[8])
            {
                NewNetMessage.SendData(25, -1, -1, string.Concat(new object[]
			{
				"*",
				Main.player[whoAmI].name,
				" ",
				Lang.mp[9],
				" ",
				Main.rand.Next(1, 101)
			}), 255, 255f, 240f, 20f, 0);
                return;
            }
            if (text2.StartsWith("/p "))
            {
                int team = Main.player[whoAmI].team;
                color = Main.teamColor[team];
                if (team != 0)
                {
                    for (int num63 = 0; num63 < 255; num63++)
                    {
                        if (Main.player[num63].team == team)
                        {
                            NewNetMessage.SendData(25, num63, -1, text.Substring(3), whoAmI, (float)color.R, (float)color.G, (float)color.B, 0);
                        }
                    }
                    return;
                }
                NewNetMessage.SendData(25, whoAmI, -1, Lang.mp[10], 255, 255f, 240f, 20f, 0);
                return;
            }
            else
            {
                var player = Main.player[whoAmI];
                color = Color.White;
                if (player.Op)
                {
                    color = Color.DeepSkyBlue;
                }
                else if (player.difficulty == 1)
                {
                    color = Main.mcColor;
                }
                else if (player.difficulty == 2)
                {
                    color = Main.hcColor;
                }
                else if (player.team > 0 && player.team < Main.teamColor.Length)
                {
                    color = Main.teamColor[player.team];
                }

                var ctx = new HookContext
                {
                    Connection = player.Connection,
                    Sender = player,
                    Player = player,
                };

                var args = new HookArgs.PlayerChat
                {
                    Message = text,
                    Color = color,
                };

                HookPoints.PlayerChat.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;

                NewNetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, text, whoAmI, args.Color.R, args.Color.G, args.Color.B);
                if (Main.dedServ)
                {
                    ProgramLog.Log("<" + Main.player[whoAmI].name + "> " + text);
                    return;
                }
                return;
            }
        }
    }
}
