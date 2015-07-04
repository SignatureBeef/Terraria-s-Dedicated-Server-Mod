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
                case Packet.TILE_BREAK:
                    ProcessTileBreak(bufferId);
                    return 0;
                case Packet.PROJECTILE:
                    ProcessProjectile(bufferId);
                    return 0;
            }

            return packetId;
        }

        private static void ProcessProjectile(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];

            int identity = (int)buffer.reader.ReadInt16();
            Vector2 position = buffer.reader.ReadVector2();
            Vector2 velocity = buffer.reader.ReadVector2();
            float knockBack = buffer.reader.ReadSingle();
            int damage = (int)buffer.reader.ReadInt16();
            int owner = (int)buffer.reader.ReadByte();
            int type = (int)buffer.reader.ReadInt16();
            BitsByte flags = buffer.reader.ReadByte();
            float[] ai = new float[Projectile.maxAI];

            for (int i = 0; i < Projectile.maxAI; i++)
            {
                if (flags[i])
                {
                    ai[i] = buffer.reader.ReadSingle();
                }
                else
                {
                    ai[i] = 0;
                }
            }
            if (Main.netMode == 2)
            {
                owner = bufferId;
                if (Main.projHostile[type])
                {
                    return;
                }
            }
            int index = 1000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
            }
            if (index == 1000)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (!Main.projectile[i].active)
                    {
                        index = i;
                        break;
                    }
                }
            }

            var player = Main.player[bufferId];
            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.ProjectileReceived
            {
                Position = position,
                Velocity = velocity,
                Id = identity,
                Owner = bufferId,
                Knockback = knockBack,
                Damage = damage,
                Type = type,
                AI = ai,
                ExistingIndex = index < 1000 ? index : -1
            };

            HookPoints.ProjectileReceived.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                if (index > -1 && index < 1000)
                {

                    Projectile projectile = Main.projectile[index];
                    if (!projectile.active || projectile.type != type)
                    {
                        projectile.SetDefaults(type);
                        if (Main.netMode == 2)
                        {
                            Netplay.Clients[bufferId].SpamProjectile += 1;
                        }
                    }
                    projectile.identity = identity;
                    projectile.position = position;
                    projectile.velocity = velocity;
                    projectile.type = type;
                    projectile.damage = damage;
                    projectile.knockBack = knockBack;
                    projectile.owner = owner;
                    for (int num85 = 0; num85 < Projectile.maxAI; num85++)
                    {
                        projectile.ai[num85] = ai[num85];
                    }
                    projectile.ProjectileFixDesperation(owner);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(27, -1, bufferId, "", index, 0, 0, 0, 0, 0, 0);
                    }
                }
            }
        }

        private static void ProcessTileBreak(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];

            byte action = buffer.reader.ReadByte();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            short type = buffer.reader.ReadInt16();
            int style = (int)buffer.reader.ReadByte();
            bool fail = type == 1;

            if (!WorldGen.InWorld(x, y, 3))
            {
                return;
            }

            var player = Main.player[bufferId];

            //TODO implement the old methods
            var ctx = new HookContext
            {
                Connection = player.Connection,
                Sender = player,
                Player = player,
            };

            var args = new HookArgs.PlayerWorldAlteration
            {
                X = x,
                Y = y,
                Action = action,
                Type = type,
                Style = style
            };

            HookPoints.PlayerWorldAlteration.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return;

            if (ctx.Result == HookResult.IGNORE)
                return;

            if (ctx.Result == HookResult.RECTIFY)
            {
                //Terraria.WorldGen.SquareTileFrame (x, y, true);
                NetMessage.SendTileSquare(bufferId, x, y, 1);
                return;
            }

            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }

            if (Main.netMode == 2)
            {
                if (!fail)
                {
                    if (action == 0 || action == 2 || action == 4)
                    {
                        Netplay.Clients[bufferId].SpamDeleteBlock += 1;
                    }
                    if (action == 1 || action == 3)
                    {
                        Netplay.Clients[bufferId].SpamAddBlock += 1;
                    }
                }
                if (!Netplay.Clients[bufferId].TileSections[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
                {
                    fail = true;
                }
            }
            if (action == 0)
            {
                WorldGen.KillTile(x, y, fail, false, false);
            }
            if (action == 1)
            {
                WorldGen.PlaceTile(x, y, (int)type, false, true, -1, style);
            }
            if (action == 2)
            {
                WorldGen.KillWall(x, y, fail);
            }
            if (action == 3)
            {
                WorldGen.PlaceWall(x, y, (int)type, false);
            }
            if (action == 4)
            {
                WorldGen.KillTile(x, y, fail, false, true);
            }
            if (action == 5)
            {
                WorldGen.PlaceWire(x, y);
            }
            if (action == 6)
            {
                WorldGen.KillWire(x, y);
            }
            if (action == 7)
            {
                WorldGen.PoundTile(x, y);
            }
            if (action == 8)
            {
                WorldGen.PlaceActuator(x, y);
            }
            if (action == 9)
            {
                WorldGen.KillActuator(x, y);
            }
            if (action == 10)
            {
                WorldGen.PlaceWire2(x, y);
            }
            if (action == 11)
            {
                WorldGen.KillWire2(x, y);
            }
            if (action == 12)
            {
                WorldGen.PlaceWire3(x, y);
            }
            if (action == 13)
            {
                WorldGen.KillWire3(x, y);
            }
            if (action == 14)
            {
                WorldGen.SlopeTile(x, y, (int)type);
            }
            if (action == 15)
            {
                Minecart.FrameTrack(x, y, true, false);
            }
            if (Main.netMode != 2)
            {
                return;
            }
            NetMessage.SendData(17, -1, bufferId, "", (int)action, (float)x, (float)y, (float)type, style, 0, 0);
            if (action == 1 && type == 53)
            {
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
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
                    Tools.WriteLine("<" + Main.player[bufferId].name + "> " + chatText);
                }
            }
        }
    }
}
