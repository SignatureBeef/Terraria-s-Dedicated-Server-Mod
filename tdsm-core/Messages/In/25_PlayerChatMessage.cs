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
            var whoAmI = conn.SlotIndex;

            ReadByte(readBuffer);
            ReadRGB(readBuffer);

            var color = Color.White;

            string text;
            if (!ReadString(readBuffer, out text)) return;

            var lowered = text.ToLower();
            if (lowered == Lang.mp[6] || lowered == Lang.mp[21])
            {
                var players = String.Empty;
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (players.Length > 0) players += ", ";
                        players += Main.player[i].name;
                    }
                }
                NewNetMessage.SendData(25, whoAmI, -1, Lang.mp[7] + " " + players + ".", 255, 255f, 240f, 20f, 0);
                return;
            }
            if (lowered.StartsWith("/me "))
            {
                NewNetMessage.SendData(25, -1, -1, "*" + Main.player[whoAmI].name + " " + text.Substring(4), 255, 200f, 100f, 0f, 0);
                return;
            }
            if (lowered == Lang.mp[8])
            {
                NewNetMessage.SendData(25, -1, -1, "*" + Main.player[whoAmI].name + " " + Lang.mp[9] + " " + Main.rand.Next(1, 101), 255, 255f, 240f, 20f, 0);
                return;
            }
            if (lowered.StartsWith("/p "))
            {
                var team = Main.player[whoAmI].team;
                var msg = text.Substring(3);

                if (team != 0)
                {
                    color = Main.teamColor[team];
                    for (int i = 0; i < 255; i++)
                    {
                        if (Main.player[i].team == team)
                            NewNetMessage.SendData(25, i, -1, msg, whoAmI, (float)color.R, (float)color.G, (float)color.B, 0);
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

                if (player.Op) color = Color.DeepSkyBlue;
                else if (player.difficulty == 1) color = Main.mcColor;
                else if (player.difficulty == 2) color = Main.hcColor;
                else if (player.team > 0 && player.team < Main.teamColor.Length) color = Main.teamColor[player.team];

                var ctx = new HookContext
                {
                    Connection = player.Connection,
                    Sender = player,
                    Player = player
                };

                var args = new HookArgs.PlayerChat
                {
                    Message = text,
                    Color = color
                };

                HookPoints.PlayerChat.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;

                NewNetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, text, whoAmI, args.Color.R, args.Color.G, args.Color.B);
                ProgramLog.Log("<" + Main.player[whoAmI].name + "> " + text);
            }
        }
    }
}
