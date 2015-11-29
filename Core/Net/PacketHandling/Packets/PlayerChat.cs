using System;
using Terraria;
using Microsoft.Xna.Framework;
using OTA;
using OTA.Plugin;
using OTA.Logging;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class PlayerChat : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.PLAYER_CHAT; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            
            //Discard
            buffer.reader.ReadByte();
            buffer.reader.ReadRGB();
            
            var chatText = buffer.reader.ReadString();
            
            ProcessQueuedPlayerCommand(new Tuple<Int32,String>(bufferId, chatText));

            return true;
        }

        internal static void ProcessQueuedPlayerCommand(Tuple<Int32, String> message)
        {
            var bufferId = message.Item1;
            var chat = message.Item2;
        
            var player = Main.player[bufferId];
            var color = Color.White;
        
            if (Main.netMode != 2)
                return;
        
            var lowered = message.Item2.ToLower();
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
                NetMessage.SendData((int)Packet.PLAYER_CHAT, bufferId, -1, Lang.mp[7] + " " + players + ".", 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else if (lowered.StartsWith("/me "))
            {
                NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, "*" + Main.player[bufferId].name + " " + chat.Substring(4), 255, 200, 100, 0, 0, 0, 0);
                return;
            }
            else if (lowered == Lang.mp[8])
            {
                NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, string.Concat(new object[]
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
                            NetMessage.SendData((int)Packet.PLAYER_CHAT, num74, -1, chat.Substring(3), bufferId, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
                        }
                    }
                    return;
                }
                NetMessage.SendData((int)Packet.PLAYER_CHAT, bufferId, -1, Lang.mp[10], 255, 255, 240, 20, 0, 0, 0);
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
                    Connection = player.Connection.Socket,
                    Sender = player,
                    Player = player
                };
        
                var args = new TDSMHookArgs.PlayerChat
                {
                    Message = chat,
                    Color = color
                };
        
                TDSMHookPoints.PlayerChat.Invoke(ref ctx, ref args);
        
                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;
        
                if (ctx.Result == HookResult.RECTIFY)
                {
                    //The a plugin is enforcing the format
                    NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, args.Message, 255, (float)args.Color.R, (float)args.Color.G, (float)args.Color.B, 0, 0, 0);
                }
                else
                {
                    //Default <Player> ...
                    NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, args.Message, bufferId, (float)args.Color.R, (float)args.Color.G, (float)args.Color.B, 0, 0, 0);
                }
        
                if (Main.dedServ)
                {
                    Loggers.Chat.Log("<" + Main.player[bufferId].name + "> " + args.Message);
                }
            }
        
        }
    }
}

