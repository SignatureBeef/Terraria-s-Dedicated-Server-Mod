using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Chat;
using tdsm.api.Plugin;

namespace tdsm.api.Callbacks
{
    public static class MessageBufferCallback
    {
        //        public static string PlayerNameFormatForChat = "<{0}> {1}";
        //        public static string PlayerNameFormatForConsole = PlayerNameFormatForChat;

        public static byte ProcessPacket(int bufferId, byte packetId)
        {
            switch ((Packet)packetId)
            {
                case Packet.PLAYER_CHAT:
                    ProcessChat(bufferId);
                    return 0;
            }

            return packetId;
        }

        private static void ProcessChat(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];

            //Discard 
            buffer.reader.ReadByte();
            buffer.reader.ReadRGB();

            var chatText = buffer.reader.ReadString();

            var player = Main.player[bufferId];
            var color = Color.White;

            if (Main.netMode != 2)
                return;

            var lowered = chatText.ToLower();
            if (lowered == Lang.mp[6] || lowered == Lang.mp[21])
            {
                var players = "";
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (players.Length > 0)
                            players += ", ";
                        players += Main.player[i].name;
                    }
                }
                NetMessage.SendData(25, bufferId, -1, Lang.mp[7] + " " + players + ".", 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else if (lowered.StartsWith("/me "))
            {
                NetMessage.SendData(25, -1, -1, "*" + Main.player[bufferId].name + " " + chatText.Substring(4), 255, 200, 100, 0, 0, 0, 0);
                return;
            }
            else if (lowered == Lang.mp[8])
            {
                NetMessage.SendData(25, -1, -1, string.Concat(new object[]
                        {
                            "*",
                            Main.player[bufferId].name,
                            " ",
                            Lang.mp[9],
                            " ",
                            Main.rand.Next(1, 101)
                        }), 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else if (lowered.StartsWith("/p "))
            {
                int team = Main.player[bufferId].team;
                color = Main.teamColor[team];
                if (team != 0)
                {
                    for (int num74 = 0; num74 < 255; num74++)
                    {
                        if (Main.player[num74].team == team)
                        {
                            NetMessage.SendData(25, num74, -1, chatText.Substring(3), bufferId, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
                        }
                    }
                    return;
                }
                NetMessage.SendData(25, bufferId, -1, Lang.mp[10], 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else
            {
                if (Main.player[bufferId].difficulty == 2)
                    color = Main.hcColor;
                else if (Main.player[bufferId].difficulty == 1)
                    color = Main.mcColor;

                var ctx = new HookContext
                {
                    Connection = player.Connection,
                    Sender = player,
                    Player = player
                };

                var args = new HookArgs.PlayerChat
                {
                    Message = chatText,
                    Color = color
                };

                HookPoints.PlayerChat.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;
                
                NetMessage.SendData(25, -1, -1, chatText, bufferId, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
                if (Main.dedServ)
                {
                    Tools.WriteLine("[TDSM] <" + Main.player[bufferId].name + "> " + chatText);
                }
            }
        }
    }
}
