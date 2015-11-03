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
    public class PasswordReceived : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.PASSWORD_RESPONSE; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            var conn = Netplay.Clients[bufferId];
            
            if (Main.netMode != 2)
            {
                return true;
            }
            var clientPassword = buffer.reader.ReadString();
            if (conn.State == -1)
            {
                var ctx = new HookContext
                {
                    Connection = conn.Socket,
                    Player = player,
                    Sender = player
                };
            
                var args = new TDSMHookArgs.ServerPassReceived
                {
                    Password = clientPassword,
                };
            
                TDSMHookPoints.ServerPassReceived.Invoke(ref ctx, ref args);
            
                if (ctx.CheckForKick())
                    return true;
            
                if (ctx.Result == HookResult.ASK_PASS)
                {
                    NetMessage.SendData((int)Packet.PASSWORD_REQUEST, bufferId);
                    return true;
                }
                else if (ctx.Result == HookResult.CONTINUE || clientPassword == Netplay.ServerPassword)
                {
                    Netplay.Clients[bufferId].State = 1;
                    NetMessage.SendData((int)Packet.CONNECTION_RESPONSE, bufferId, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
                    return true;
                }
            
                conn.Kick("Incorrect server password.");
            }
            else if (conn.State == (int)ConnectionState.WaitingForUserPassword)
            {
                //var name = player.name ?? "";
            
                var ctx = new HookContext
                {
                    Connection = conn.Socket,
                    Player = player,
                    Sender = player
                };
            
                var args = new TDSMHookArgs.PlayerPassReceived
                {
                    Password = clientPassword
                };
            
                TDSMHookPoints.PlayerPassReceived.Invoke(ref ctx, ref args);
            
                if (ctx.CheckForKick())
                    return true;
            
                if (ctx.Result == HookResult.ASK_PASS)
                {
                    NetMessage.SendData((int)Packet.PASSWORD_REQUEST, bufferId);
                    return true;
                }
                else // HookResult.DEFAULT
                {
                    //ProgramLog.Error.Log("Accepted player: " + player.name + "/" + (player.AuthenticatedAs ?? "null"));
            
                    //Continue with world request
                    Netplay.Clients[bufferId].State = 2;
                    Netplay.Clients[bufferId].ResetSections();
                    NetMessage.SendData(7, bufferId, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
                    Main.SyncAnInvasion(bufferId);

                    return true;
                }
            }

            return true;
        }
    }
}

