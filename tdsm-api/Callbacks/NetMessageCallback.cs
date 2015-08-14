using TDSM.API.Plugin;
using Microsoft.Xna.Framework;
using System.IO;

#if Full_API
using Terraria;
using Terraria.Net.Sockets;
#endif

namespace TDSM.API.Callbacks
{
    public static class NetMessageCallback
    {
        public static bool SendData(Packet msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            return SendData((int)msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5);
        }

        public static bool SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            #if Full_API
            if (Main.netMode == 0)
            {
                return false;
            }

            int bufferId = 256;
            if (Main.netMode == 2 && remoteClient >= 0)
            {
                bufferId = remoteClient;
            }

            lock (NetMessage.buffer [bufferId])
            {
                var writer = NetMessage.buffer[bufferId].writer;
                if (writer == null)
                {
                    NetMessage.buffer[bufferId].ResetWriter();
                    writer = NetMessage.buffer[bufferId].writer;
                }

                writer.BaseStream.Position = 0;
                var position = writer.BaseStream.Position;
                writer.BaseStream.Position += 2;
                writer.Write((byte)msgType);

                var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.SendNetMessage()
                {
                    MsgType = msgType,
                    BufferId = bufferId,
                    RemoteClient = remoteClient,
                    IgnoreClient = ignoreClient,
                    Text = text,
                    Number = number,
                    Number2 = number2,
                    Number3 = number3,
                    Number4 = number4,
                    Number5 = number5
                };
                HookPoints.SendNetMessage.Invoke(ref ctx, ref args);

                if (ctx.Result != HookResult.DEFAULT)
                {
                    var endOfMessage = (int)writer.BaseStream.Position;
                    writer.BaseStream.Position = position;
                    writer.Write((short)endOfMessage);
                    writer.BaseStream.Position = (long)endOfMessage;

                    if (remoteClient == -1)
                    {
                        switch ((Packet)msgType)
                        {
                            case Packet.CHEST:
                            case Packet.CHEST_NAME_UPDATE:
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.Clients[i].IsConnected())
                                    {
                                        try
                                        {
                                            NetMessage.buffer[i].spamCount++;
                                            Main.txMsg++;
                                            Main.txData += endOfMessage;
                                            Main.txMsgType[msgType]++;
                                            Main.txDataType[msgType] += endOfMessage;
                                            Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                break;

                            case Packet.TILE_SQUARE:
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.Clients[i].IsConnected() && Netplay.Clients[i].SectionRange(number, (int)number2, (int)number3))
                                    {
                                        try
                                        {
                                            NetMessage.buffer[i].spamCount++;
                                            Main.txMsg++;
                                            Main.txData += endOfMessage;
                                            Main.txMsgType[msgType]++;
                                            Main.txDataType[msgType] += endOfMessage;
                                            Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                break;

                            case Packet.NPC_INFO:
                                NPC npcInfo = Main.npc[number];
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.Clients[i].IsConnected())
                                    {
                                        var sendData = false;
                                        if (npcInfo.boss || npcInfo.netAlways || npcInfo.townNPC || !npcInfo.active)
                                            sendData = true;
                                        else if (npcInfo.netSkip <= 0)
                                        {
                                            Rectangle rect = Main.player[i].getRect();
                                            Rectangle rect2 = npcInfo.getRect();
                                            rect2.X -= 2500;
                                            rect2.Y -= 2500;
                                            rect2.Width += 5000;
                                            rect2.Height += 5000;
                                            if (rect.Intersects(rect2))
                                                sendData = true; 
                                        }
                                        else
                                            sendData = true; 

                                        if (sendData)
                                        {
                                            try
                                            {
                                                NetMessage.buffer[i].spamCount++;
                                                Main.txMsg++;
                                                Main.txData += endOfMessage;
                                                Main.txMsgType[msgType]++;
                                                Main.txDataType[msgType] += endOfMessage;
                                                Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                                npcInfo.netSkip++;
                                if (npcInfo.netSkip > 4)
                                {
                                    npcInfo.netSkip = 0;
                                }
                                break;

                            case Packet.DAMAGE_NPC:
                                NPC npcDmg = Main.npc[number];
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.Clients[i].IsConnected())
                                    {
                                        var sendData = false;
                                        if (npcDmg.life <= 0)
                                            sendData = true;
                                        else
                                        {
                                            Rectangle rect3 = Main.player[i].getRect();
                                            Rectangle rect4 = npcDmg.getRect();
                                            rect4.X -= 3000;
                                            rect4.Y -= 3000;
                                            rect4.Width += 6000;
                                            rect4.Height += 6000;
                                            if (rect3.Intersects(rect4))
                                                sendData = true; 
                                        }

                                        if (sendData)
                                        {
                                            try
                                            {
                                                NetMessage.buffer[i].spamCount++;
                                                Main.txMsg++;
                                                Main.txData += endOfMessage;
                                                Main.txMsgType[msgType]++;
                                                Main.txDataType[msgType] += endOfMessage;
                                                Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                                break;

                            case Packet.PLAYER_STATE_UPDATE:
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.Clients[i].IsConnected())
                                    {
                                        try
                                        {
                                            NetMessage.buffer[i].spamCount++;
                                            Main.txMsg++;
                                            Main.txData += endOfMessage;
                                            Main.txMsgType[msgType]++;
                                            Main.txDataType[msgType] += endOfMessage;
                                            Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                Main.player[number].netSkip++;
                                if (Main.player[number].netSkip > 2)
                                {
                                    Main.player[number].netSkip = 0;
                                }
                                break;

                            case Packet.PROJECTILE:
                                Projectile projectile2 = Main.projectile[number];
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && NetMessage.buffer[i].broadcast && Netplay.Clients[i].IsConnected())
                                    {
                                        var sendData = false;
                                        if (projectile2.type == 12 || Main.projPet[projectile2.type] || projectile2.aiStyle == 11 || projectile2.netImportant)
                                            sendData = true;
                                        else
                                        {
                                            Rectangle rect5 = Main.player[i].getRect();
                                            Rectangle rect6 = projectile2.getRect();
                                            //Is this some sort of in range check? o.O
                                            rect6.X -= 5000;
                                            rect6.Y -= 5000;
                                            rect6.Width += 10000;
                                            rect6.Height += 10000;
                                            if (rect5.Intersects(rect6))
                                                sendData = true;
                                        }

                                        if (sendData)
                                        {
                                            try
                                            {
                                                NetMessage.buffer[i].spamCount++;
                                                Main.txMsg++;
                                                Main.txData += endOfMessage;
                                                Main.txMsgType[msgType]++;
                                                Main.txDataType[msgType] += endOfMessage;
                                                Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                                break;

                            default:
                                for (int i = 0; i < 256; i++)
                                {
                                    if (i != ignoreClient && (NetMessage.buffer[i].broadcast || (Netplay.Clients[i].State >= 3 && msgType == 10)) && Netplay.Clients[i].IsConnected())
                                    {
                                        try
                                        {
                                            NetMessage.buffer[i].spamCount++;
                                            Main.txMsg++;
                                            Main.txData += endOfMessage;
                                            Main.txMsgType[msgType]++;
                                            Main.txDataType[msgType] += endOfMessage;
                                            Netplay.Clients[i].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[i].ServerWriteCallBack), null);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (Netplay.Clients[remoteClient].IsConnected())
                        {
                            try
                            {
                                NetMessage.buffer[remoteClient].spamCount++;
                                Main.txMsg++;
                                Main.txData += endOfMessage;
                                Main.txMsgType[msgType]++;
                                Main.txDataType[msgType] += endOfMessage;
                                Netplay.Clients[remoteClient].Socket.AsyncSend(NetMessage.buffer[bufferId].writeBuffer, 0, endOfMessage, new SocketSendCallback(Netplay.Clients[remoteClient].ServerWriteCallBack), null);
                            }
                            catch
                            {
                            }
                        }
                    }

                }
                else
                    return true; //Continue with vanilla code
            }
            #endif
            return false;
        }
    }
}
