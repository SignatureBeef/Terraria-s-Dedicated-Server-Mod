using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class NetMessage
    {
        public static messageBuffer[] buffer = new messageBuffer[257];

        public static void SendData(int msgType, World world, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f)
        {
            int num = 9;
            if (Statics.netMode == 2 && remoteClient >= 0)
            {
                num = remoteClient;
            }
            lock (NetMessage.buffer[num])
            {
                int num2 = 5;
                int num3 = num2;
                if (msgType == 1)
                {
                    byte[] bytes = BitConverter.GetBytes(msgType);
                    byte[] bytes2 = Encoding.ASCII.GetBytes("Terraria" + Statics.currentRelease);
                    num2 += bytes2.Length;
                    byte[] bytes3 = BitConverter.GetBytes(num2 - 4);
                    Buffer.BlockCopy(bytes3, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                    Buffer.BlockCopy(bytes, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                    Buffer.BlockCopy(bytes2, 0, NetMessage.buffer[num].writeBuffer, 5, bytes2.Length);
                }
                else
                {
                    if (msgType == 2)
                    {
                        byte[] bytes4 = BitConverter.GetBytes(msgType);
                        byte[] bytes5 = Encoding.ASCII.GetBytes(text);
                        num2 += bytes5.Length;
                        byte[] bytes6 = BitConverter.GetBytes(num2 - 4);
                        Buffer.BlockCopy(bytes6, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                        Buffer.BlockCopy(bytes4, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                        Buffer.BlockCopy(bytes5, 0, NetMessage.buffer[num].writeBuffer, 5, bytes5.Length);
                    }
                    else
                    {
                        if (msgType == 3)
                        {
                            byte[] bytes7 = BitConverter.GetBytes(msgType);
                            byte[] bytes8 = BitConverter.GetBytes(remoteClient);
                            num2 += bytes8.Length;
                            byte[] bytes9 = BitConverter.GetBytes(num2 - 4);
                            Buffer.BlockCopy(bytes9, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                            Buffer.BlockCopy(bytes7, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                            Buffer.BlockCopy(bytes8, 0, NetMessage.buffer[num].writeBuffer, 5, bytes8.Length);
                        }
                        else
                        {
                            if (msgType == 4)
                            {
                                byte[] bytes10 = BitConverter.GetBytes(msgType);
                                byte b = (byte)number;
                                byte b2 = (byte)world.getPlayerList()[(int)b].hair;
                                byte[] bytes11 = Encoding.ASCII.GetBytes(text);
                                num2 += 23 + bytes11.Length;
                                byte[] bytes12 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes12, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes10, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[6] = b2;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].hairColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].hairColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].hairColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].skinColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].skinColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].skinColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].eyeColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].eyeColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].eyeColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].shirtColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].shirtColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].shirtColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].underShirtColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].underShirtColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].underShirtColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].pantsColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].pantsColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].pantsColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].shoeColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].shoeColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getPlayerList()[(int)b].shoeColor.B;
                                num3++;
                                Buffer.BlockCopy(bytes11, 0, NetMessage.buffer[num].writeBuffer, num3, bytes11.Length);
                            }
                            else
                            {
                                if (msgType == 5)
                                {
                                    byte[] bytes13 = BitConverter.GetBytes(msgType);
                                    byte b3 = (byte)number;
                                    byte b4 = (byte)number2;
                                    byte b5;
                                    if (number2 < 44f)
                                    {
                                        b5 = (byte)world.getPlayerList()[number].inventory[(int)number2].stack;
                                        if (world.getPlayerList()[number].inventory[(int)number2].stack < 0)
                                        {
                                            b5 = 0;
                                        }
                                    }
                                    else
                                    {
                                        b5 = (byte)world.getPlayerList()[number].armor[(int)number2 - 44].stack;
                                        if (world.getPlayerList()[number].armor[(int)number2 - 44].stack < 0)
                                        {
                                            b5 = 0;
                                        }
                                    }
                                    string text2 = text;
                                    if (text2 == null)
                                    {
                                        text2 = "";
                                    }
                                    byte[] bytes14 = Encoding.ASCII.GetBytes(text2);
                                    num2 += 3 + bytes14.Length;
                                    byte[] bytes15 = BitConverter.GetBytes(num2 - 4);
                                    Buffer.BlockCopy(bytes15, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                    Buffer.BlockCopy(bytes13, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                    NetMessage.buffer[num].writeBuffer[5] = b3;
                                    num3++;
                                    NetMessage.buffer[num].writeBuffer[6] = b4;
                                    num3++;
                                    NetMessage.buffer[num].writeBuffer[7] = b5;
                                    num3++;
                                    Buffer.BlockCopy(bytes14, 0, NetMessage.buffer[num].writeBuffer, num3, bytes14.Length);
                                }
                                else
                                {
                                    if (msgType == 6)
                                    {
                                        byte[] bytes16 = BitConverter.GetBytes(msgType);
                                        byte[] bytes17 = BitConverter.GetBytes(num2 - 4);
                                        Buffer.BlockCopy(bytes17, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                        Buffer.BlockCopy(bytes16, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                    }
                                    else
                                    {
                                        if (msgType == 7)
                                        {
                                            byte[] bytes18 = BitConverter.GetBytes(msgType);
                                            byte[] bytes19 = BitConverter.GetBytes((int)world.getTime());
                                            byte b6 = 0;
                                            if (world.isDayTime())
                                            {
                                                b6 = 1;
                                            }
                                            byte b7 = (byte)world.getMoonPhase();
                                            byte b8 = 0;
                                            if (world.isBloodMoon())
                                            {
                                                b8 = 1;
                                            }
                                            byte[] bytes20 = BitConverter.GetBytes(Statics.maxTilesX);
                                            byte[] bytes21 = BitConverter.GetBytes(Statics.maxTilesY);
                                            byte[] bytes22 = BitConverter.GetBytes(Statics.spawnTileX);
                                            byte[] bytes23 = BitConverter.GetBytes(Statics.spawnTileY);
                                            byte[] bytes24 = BitConverter.GetBytes((int)world.getWorldSurface());
                                            byte[] bytes25 = BitConverter.GetBytes((int)world.getRockLayer());
                                            byte[] bytes26 = BitConverter.GetBytes(world.getId());
                                            byte[] bytes27 = Encoding.ASCII.GetBytes(world.getName());
                                            num2 += bytes19.Length + 1 + 1 + 1 + bytes20.Length + bytes21.Length + bytes22.Length + bytes23.Length + bytes24.Length + bytes25.Length + bytes26.Length + bytes27.Length;
                                            byte[] bytes28 = BitConverter.GetBytes(num2 - 4);
                                            Buffer.BlockCopy(bytes28, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                            Buffer.BlockCopy(bytes18, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                            Buffer.BlockCopy(bytes19, 0, NetMessage.buffer[num].writeBuffer, 5, bytes19.Length);
                                            num3 += bytes19.Length;
                                            NetMessage.buffer[num].writeBuffer[num3] = b6;
                                            num3++;
                                            NetMessage.buffer[num].writeBuffer[num3] = b7;
                                            num3++;
                                            NetMessage.buffer[num].writeBuffer[num3] = b8;
                                            num3++;
                                            Buffer.BlockCopy(bytes20, 0, NetMessage.buffer[num].writeBuffer, num3, bytes20.Length);
                                            num3 += bytes20.Length;
                                            Buffer.BlockCopy(bytes21, 0, NetMessage.buffer[num].writeBuffer, num3, bytes21.Length);
                                            num3 += bytes21.Length;
                                            Buffer.BlockCopy(bytes22, 0, NetMessage.buffer[num].writeBuffer, num3, bytes22.Length);
                                            num3 += bytes22.Length;
                                            Buffer.BlockCopy(bytes23, 0, NetMessage.buffer[num].writeBuffer, num3, bytes23.Length);
                                            num3 += bytes23.Length;
                                            Buffer.BlockCopy(bytes24, 0, NetMessage.buffer[num].writeBuffer, num3, bytes24.Length);
                                            num3 += bytes24.Length;
                                            Buffer.BlockCopy(bytes25, 0, NetMessage.buffer[num].writeBuffer, num3, bytes25.Length);
                                            num3 += bytes25.Length;
                                            Buffer.BlockCopy(bytes26, 0, NetMessage.buffer[num].writeBuffer, num3, bytes26.Length);
                                            num3 += bytes26.Length;
                                            Buffer.BlockCopy(bytes27, 0, NetMessage.buffer[num].writeBuffer, num3, bytes27.Length);
                                            num3 += bytes27.Length;
                                        }
                                        else
                                        {
                                            if (msgType == 8)
                                            {
                                                byte[] bytes29 = BitConverter.GetBytes(msgType);
                                                byte[] bytes30 = BitConverter.GetBytes(number);
                                                byte[] bytes31 = BitConverter.GetBytes((int)number2);
                                                num2 += bytes30.Length + bytes31.Length;
                                                byte[] bytes32 = BitConverter.GetBytes(num2 - 4);
                                                Buffer.BlockCopy(bytes32, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                Buffer.BlockCopy(bytes29, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                Buffer.BlockCopy(bytes30, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                num3 += 4;
                                                Buffer.BlockCopy(bytes31, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                            }
                                            else
                                            {
                                                if (msgType == 9)
                                                {
                                                    byte[] bytes33 = BitConverter.GetBytes(msgType);
                                                    byte[] bytes34 = BitConverter.GetBytes(number);
                                                    byte[] bytes35 = Encoding.ASCII.GetBytes(text);
                                                    num2 += bytes34.Length + bytes35.Length;
                                                    byte[] bytes36 = BitConverter.GetBytes(num2 - 4);
                                                    Buffer.BlockCopy(bytes36, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                    Buffer.BlockCopy(bytes33, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                    Buffer.BlockCopy(bytes34, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                    num3 += 4;
                                                    Buffer.BlockCopy(bytes35, 0, NetMessage.buffer[num].writeBuffer, num3, bytes35.Length);
                                                }
                                                else
                                                {
                                                    if (msgType == 10)
                                                    {
                                                        short num4 = (short)number;
                                                        int num5 = (int)number2;
                                                        int num6 = (int)number3;
                                                        byte[] bytes37 = BitConverter.GetBytes(msgType);
                                                        Buffer.BlockCopy(bytes37, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                        byte[] bytes38 = BitConverter.GetBytes(num4);
                                                        Buffer.BlockCopy(bytes38, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                        num3 += 2;
                                                        byte[] bytes39 = BitConverter.GetBytes(num5);
                                                        Buffer.BlockCopy(bytes39, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                        num3 += 4;
                                                        byte[] bytes40 = BitConverter.GetBytes(num6);
                                                        Buffer.BlockCopy(bytes40, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                        num3 += 4;
                                                        for (int i = num5; i < num5 + (int)num4; i++)
                                                        {
                                                            byte b9 = 0;
                                                            if (world.getTile()[i, num6].active)
                                                            {
                                                                b9 += 1;
                                                            }
                                                            if (world.getTile()[i, num6].lighted)
                                                            {
                                                                b9 += 2;
                                                            }
                                                            if (world.getTile()[i, num6].wall > 0)
                                                            {
                                                                b9 += 4;
                                                            }
                                                            if (world.getTile()[i, num6].liquid > 0)
                                                            {
                                                                b9 += 8;
                                                            }
                                                            NetMessage.buffer[num].writeBuffer[num3] = b9;
                                                            num3++;
                                                            byte[] bytes41 = BitConverter.GetBytes(world.getTile()[i, num6].frameX);
                                                            byte[] bytes42 = BitConverter.GetBytes(world.getTile()[i, num6].frameY);
                                                            byte wall = world.getTile()[i, num6].wall;
                                                            if (world.getTile()[i, num6].active)
                                                            {
                                                                NetMessage.buffer[num].writeBuffer[num3] = world.getTile()[i, num6].type;
                                                                num3++;
                                                                if (Statics.tileFrameImportant[(int)world.getTile()[i, num6].type])
                                                                {
                                                                    Buffer.BlockCopy(bytes41, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                    num3 += 2;
                                                                    Buffer.BlockCopy(bytes42, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                    num3 += 2;
                                                                }
                                                            }
                                                            if (wall > 0)
                                                            {
                                                                NetMessage.buffer[num].writeBuffer[num3] = wall;
                                                                num3++;
                                                            }
                                                            if (world.getTile()[i, num6].liquid > 0)
                                                            {
                                                                NetMessage.buffer[num].writeBuffer[num3] = world.getTile()[i, num6].liquid;
                                                                num3++;
                                                                byte b10 = 0;
                                                                if (world.getTile()[i, num6].lava)
                                                                {
                                                                    b10 = 1;
                                                                }
                                                                NetMessage.buffer[num].writeBuffer[num3] = b10;
                                                                num3++;
                                                            }
                                                        }
                                                        byte[] bytes43 = BitConverter.GetBytes(num3 - 4);
                                                        Buffer.BlockCopy(bytes43, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                        num2 = num3;
                                                    }
                                                    else
                                                    {
                                                        if (msgType == 11)
                                                        {
                                                            byte[] bytes44 = BitConverter.GetBytes(msgType);
                                                            byte[] bytes45 = BitConverter.GetBytes(number);
                                                            byte[] bytes46 = BitConverter.GetBytes((int)number2);
                                                            byte[] bytes47 = BitConverter.GetBytes((int)number3);
                                                            byte[] bytes48 = BitConverter.GetBytes((int)number4);
                                                            num2 += bytes45.Length + bytes46.Length + bytes47.Length + bytes48.Length;
                                                            byte[] bytes49 = BitConverter.GetBytes(num2 - 4);
                                                            Buffer.BlockCopy(bytes49, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                            Buffer.BlockCopy(bytes44, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                            Buffer.BlockCopy(bytes45, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                            num3 += 4;
                                                            Buffer.BlockCopy(bytes46, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                            num3 += 4;
                                                            Buffer.BlockCopy(bytes47, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                            num3 += 4;
                                                            Buffer.BlockCopy(bytes48, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                            num3 += 4;
                                                        }
                                                        else
                                                        {
                                                            if (msgType == 12)
                                                            {
                                                                byte[] bytes50 = BitConverter.GetBytes(msgType);
                                                                byte b11 = (byte)number;
                                                                byte[] bytes51 = BitConverter.GetBytes(world.getPlayerList()[(int)b11].SpawnX);
                                                                byte[] bytes52 = BitConverter.GetBytes(world.getPlayerList()[(int)b11].SpawnY);
                                                                num2 += 1 + bytes51.Length + bytes52.Length;
                                                                byte[] bytes53 = BitConverter.GetBytes(num2 - 4);
                                                                Buffer.BlockCopy(bytes53, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                Buffer.BlockCopy(bytes50, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                NetMessage.buffer[num].writeBuffer[num3] = b11;
                                                                num3++;
                                                                Buffer.BlockCopy(bytes51, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                num3 += 4;
                                                                Buffer.BlockCopy(bytes52, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                num3 += 4;
                                                            }
                                                            else
                                                            {
                                                                if (msgType == 13)
                                                                {
                                                                    byte[] bytes54 = BitConverter.GetBytes(msgType);
                                                                    byte b12 = (byte)number;
                                                                    byte b13 = 0;
                                                                    if (world.getPlayerList()[(int)b12].controlUp)
                                                                    {
                                                                        b13 += 1;
                                                                    }
                                                                    if (world.getPlayerList()[(int)b12].controlDown)
                                                                    {
                                                                        b13 += 2;
                                                                    }
                                                                    if (world.getPlayerList()[(int)b12].controlLeft)
                                                                    {
                                                                        b13 += 4;
                                                                    }
                                                                    if (world.getPlayerList()[(int)b12].controlRight)
                                                                    {
                                                                        b13 += 8;
                                                                    }
                                                                    if (world.getPlayerList()[(int)b12].controlJump)
                                                                    {
                                                                        b13 += 16;
                                                                    }
                                                                    if (world.getPlayerList()[(int)b12].controlUseItem)
                                                                    {
                                                                        b13 += 32;
                                                                    }
                                                                    if (world.getPlayerList()[(int)b12].direction == 1)
                                                                    {
                                                                        b13 += 64;
                                                                    }
                                                                    byte b14 = (byte)world.getPlayerList()[(int)b12].selectedItem;
                                                                    byte[] bytes55 = BitConverter.GetBytes(world.getPlayerList()[number].position.X);
                                                                    byte[] bytes56 = BitConverter.GetBytes(world.getPlayerList()[number].position.Y);
                                                                    byte[] bytes57 = BitConverter.GetBytes(world.getPlayerList()[number].velocity.X);
                                                                    byte[] bytes58 = BitConverter.GetBytes(world.getPlayerList()[number].velocity.Y);
                                                                    num2 += 3 + bytes55.Length + bytes56.Length + bytes57.Length + bytes58.Length;
                                                                    byte[] bytes59 = BitConverter.GetBytes(num2 - 4);
                                                                    Buffer.BlockCopy(bytes59, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                    Buffer.BlockCopy(bytes54, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                    NetMessage.buffer[num].writeBuffer[5] = b12;
                                                                    num3++;
                                                                    NetMessage.buffer[num].writeBuffer[6] = b13;
                                                                    num3++;
                                                                    NetMessage.buffer[num].writeBuffer[7] = b14;
                                                                    num3++;
                                                                    Buffer.BlockCopy(bytes55, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                    num3 += 4;
                                                                    Buffer.BlockCopy(bytes56, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                    num3 += 4;
                                                                    Buffer.BlockCopy(bytes57, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                    num3 += 4;
                                                                    Buffer.BlockCopy(bytes58, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                }
                                                                else
                                                                {
                                                                    if (msgType == 14)
                                                                    {
                                                                        byte[] bytes60 = BitConverter.GetBytes(msgType);
                                                                        byte b15 = (byte)number;
                                                                        byte b16 = (byte)number2;
                                                                        num2 += 2;
                                                                        byte[] bytes61 = BitConverter.GetBytes(num2 - 4);
                                                                        Buffer.BlockCopy(bytes61, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                        Buffer.BlockCopy(bytes60, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                        NetMessage.buffer[num].writeBuffer[5] = b15;
                                                                        NetMessage.buffer[num].writeBuffer[6] = b16;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (msgType == 15)
                                                                        {
                                                                            byte[] bytes62 = BitConverter.GetBytes(msgType);
                                                                            byte[] bytes63 = BitConverter.GetBytes(num2 - 4);
                                                                            Buffer.BlockCopy(bytes63, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                            Buffer.BlockCopy(bytes62, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (msgType == 16)
                                                                            {
                                                                                byte[] bytes64 = BitConverter.GetBytes(msgType);
                                                                                byte b17 = (byte)number;
                                                                                byte[] bytes65 = BitConverter.GetBytes((short)world.getPlayerList()[(int)b17].statLife);
                                                                                byte[] bytes66 = BitConverter.GetBytes((short)world.getPlayerList()[(int)b17].statLifeMax);
                                                                                num2 += 1 + bytes65.Length + bytes66.Length;
                                                                                byte[] bytes67 = BitConverter.GetBytes(num2 - 4);
                                                                                Buffer.BlockCopy(bytes67, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                Buffer.BlockCopy(bytes64, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                NetMessage.buffer[num].writeBuffer[5] = b17;
                                                                                num3++;
                                                                                Buffer.BlockCopy(bytes65, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                num3 += 2;
                                                                                Buffer.BlockCopy(bytes66, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                            }
                                                                            else
                                                                            {
                                                                                if (msgType == 17)
                                                                                {
                                                                                    byte[] bytes68 = BitConverter.GetBytes(msgType);
                                                                                    byte b18 = (byte)number;
                                                                                    byte[] bytes69 = BitConverter.GetBytes((int)number2);
                                                                                    byte[] bytes70 = BitConverter.GetBytes((int)number3);
                                                                                    byte b19 = (byte)number4;
                                                                                    num2 += 1 + bytes69.Length + bytes70.Length + 1;
                                                                                    byte[] bytes71 = BitConverter.GetBytes(num2 - 4);
                                                                                    Buffer.BlockCopy(bytes71, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                    Buffer.BlockCopy(bytes68, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b18;
                                                                                    num3++;
                                                                                    Buffer.BlockCopy(bytes69, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                    num3 += 4;
                                                                                    Buffer.BlockCopy(bytes70, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                    num3 += 4;
                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b19;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (msgType == 18)
                                                                                    {
                                                                                        byte[] bytes72 = BitConverter.GetBytes(msgType);
                                                                                        BitConverter.GetBytes((int)world.getTime());
                                                                                        byte b20 = 0;
                                                                                        if (world.isDayTime())
                                                                                        {
                                                                                            b20 = 1;
                                                                                        }
                                                                                        byte[] bytes73 = BitConverter.GetBytes((int)world.getTime());
                                                                                        byte[] bytes74 = BitConverter.GetBytes(world.getSunModY());
                                                                                        byte[] bytes75 = BitConverter.GetBytes(world.getMoonModY());
                                                                                        num2 += 1 + bytes73.Length + bytes74.Length + bytes75.Length;
                                                                                        byte[] bytes76 = BitConverter.GetBytes(num2 - 4);
                                                                                        Buffer.BlockCopy(bytes76, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                        Buffer.BlockCopy(bytes72, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b20;
                                                                                        num3++;
                                                                                        Buffer.BlockCopy(bytes73, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                        num3 += 4;
                                                                                        Buffer.BlockCopy(bytes74, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                        num3 += 2;
                                                                                        Buffer.BlockCopy(bytes75, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                        num3 += 2;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (msgType == 19)
                                                                                        {
                                                                                            byte[] bytes77 = BitConverter.GetBytes(msgType);
                                                                                            byte b21 = (byte)number;
                                                                                            byte[] bytes78 = BitConverter.GetBytes((int)number2);
                                                                                            byte[] bytes79 = BitConverter.GetBytes((int)number3);
                                                                                            byte b22 = 0;
                                                                                            if (number4 == 1f)
                                                                                            {
                                                                                                b22 = 1;
                                                                                            }
                                                                                            num2 += 1 + bytes78.Length + bytes79.Length + 1;
                                                                                            byte[] bytes80 = BitConverter.GetBytes(num2 - 4);
                                                                                            Buffer.BlockCopy(bytes80, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                            Buffer.BlockCopy(bytes77, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                            NetMessage.buffer[num].writeBuffer[num3] = b21;
                                                                                            num3++;
                                                                                            Buffer.BlockCopy(bytes78, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                            num3 += 4;
                                                                                            Buffer.BlockCopy(bytes79, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                            num3 += 4;
                                                                                            NetMessage.buffer[num].writeBuffer[num3] = b22;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (msgType == 20)
                                                                                            {
                                                                                                short num7 = (short)number;
                                                                                                int num8 = (int)number2;
                                                                                                int num9 = (int)number3;
                                                                                                byte[] bytes81 = BitConverter.GetBytes(msgType);
                                                                                                Buffer.BlockCopy(bytes81, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                byte[] bytes82 = BitConverter.GetBytes(num7);
                                                                                                Buffer.BlockCopy(bytes82, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                num3 += 2;
                                                                                                byte[] bytes83 = BitConverter.GetBytes(num8);
                                                                                                Buffer.BlockCopy(bytes83, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                                num3 += 4;
                                                                                                byte[] bytes84 = BitConverter.GetBytes(num9);
                                                                                                Buffer.BlockCopy(bytes84, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                                num3 += 4;
                                                                                                for (int j = num8; j < num8 + (int)num7; j++)
                                                                                                {
                                                                                                    for (int k = num9; k < num9 + (int)num7; k++)
                                                                                                    {
                                                                                                        byte b23 = 0;
                                                                                                        if (world.getTile()[j, k].active)
                                                                                                        {
                                                                                                            b23 += 1;
                                                                                                        }
                                                                                                        if (world.getTile()[j, k].lighted)
                                                                                                        {
                                                                                                            b23 += 2;
                                                                                                        }
                                                                                                        if (world.getTile()[j, k].wall > 0)
                                                                                                        {
                                                                                                            b23 += 4;
                                                                                                        }
                                                                                                        if (world.getTile()[j, k].liquid > 0 && Statics.netMode == 2)
                                                                                                        {
                                                                                                            b23 += 8;
                                                                                                        }
                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b23;
                                                                                                        num3++;
                                                                                                        byte[] bytes85 = BitConverter.GetBytes(world.getTile()[j, k].frameX);
                                                                                                        byte[] bytes86 = BitConverter.GetBytes(world.getTile()[j, k].frameY);
                                                                                                        byte wall2 = world.getTile()[j, k].wall;
                                                                                                        if (world.getTile()[j, k].active)
                                                                                                        {
                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = world.getTile()[j, k].type;
                                                                                                            num3++;
                                                                                                            if (Statics.tileFrameImportant[(int)world.getTile()[j, k].type])
                                                                                                            {
                                                                                                                Buffer.BlockCopy(bytes85, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                                num3 += 2;
                                                                                                                Buffer.BlockCopy(bytes86, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                                num3 += 2;
                                                                                                            }
                                                                                                        }
                                                                                                        if (wall2 > 0)
                                                                                                        {
                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = wall2;
                                                                                                            num3++;
                                                                                                        }
                                                                                                        if (world.getTile()[j, k].liquid > 0 && Statics.netMode == 2)
                                                                                                        {
                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = world.getTile()[j, k].liquid;
                                                                                                            num3++;
                                                                                                            byte b24 = 0;
                                                                                                            if (world.getTile()[j, k].lava)
                                                                                                            {
                                                                                                                b24 = 1;
                                                                                                            }
                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = b24;
                                                                                                            num3++;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                byte[] bytes87 = BitConverter.GetBytes(num3 - 4);
                                                                                                Buffer.BlockCopy(bytes87, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                num2 = num3;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (msgType == 21)
                                                                                                {
                                                                                                    byte[] bytes88 = BitConverter.GetBytes(msgType);
                                                                                                    byte[] bytes89 = BitConverter.GetBytes((short)number);
                                                                                                    byte[] bytes90 = BitConverter.GetBytes(world.getItemList()[number].position.X);
                                                                                                    byte[] bytes91 = BitConverter.GetBytes(world.getItemList()[number].position.Y);
                                                                                                    byte[] bytes92 = BitConverter.GetBytes(world.getItemList()[number].velocity.X);
                                                                                                    byte[] bytes93 = BitConverter.GetBytes(world.getItemList()[number].velocity.Y);
                                                                                                    byte b25 = (byte)world.getItemList()[number].stack;
                                                                                                    string text3 = "0";
                                                                                                    if (world.getItemList()[number].active && world.getItemList()[number].stack > 0)
                                                                                                    {
                                                                                                        text3 = world.getItemList()[number].name;
                                                                                                    }
                                                                                                    if (text3 == null)
                                                                                                    {
                                                                                                        text3 = "0";
                                                                                                    }
                                                                                                    byte[] bytes94 = Encoding.ASCII.GetBytes(text3);
                                                                                                    num2 += bytes89.Length + bytes90.Length + bytes91.Length + bytes92.Length + bytes93.Length + 1 + bytes94.Length;
                                                                                                    byte[] bytes95 = BitConverter.GetBytes(num2 - 4);
                                                                                                    Buffer.BlockCopy(bytes95, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                    Buffer.BlockCopy(bytes88, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                    Buffer.BlockCopy(bytes89, 0, NetMessage.buffer[num].writeBuffer, num3, bytes89.Length);
                                                                                                    num3 += 2;
                                                                                                    Buffer.BlockCopy(bytes90, 0, NetMessage.buffer[num].writeBuffer, num3, bytes90.Length);
                                                                                                    num3 += 4;
                                                                                                    Buffer.BlockCopy(bytes91, 0, NetMessage.buffer[num].writeBuffer, num3, bytes91.Length);
                                                                                                    num3 += 4;
                                                                                                    Buffer.BlockCopy(bytes92, 0, NetMessage.buffer[num].writeBuffer, num3, bytes92.Length);
                                                                                                    num3 += 4;
                                                                                                    Buffer.BlockCopy(bytes93, 0, NetMessage.buffer[num].writeBuffer, num3, bytes93.Length);
                                                                                                    num3 += 4;
                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b25;
                                                                                                    num3++;
                                                                                                    Buffer.BlockCopy(bytes94, 0, NetMessage.buffer[num].writeBuffer, num3, bytes94.Length);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (msgType == 22)
                                                                                                    {
                                                                                                        byte[] bytes96 = BitConverter.GetBytes(msgType);
                                                                                                        byte[] bytes97 = BitConverter.GetBytes((short)number);
                                                                                                        byte b26 = (byte)world.getItemList()[number].owner;
                                                                                                        num2 += bytes97.Length + 1;
                                                                                                        byte[] bytes98 = BitConverter.GetBytes(num2 - 4);
                                                                                                        Buffer.BlockCopy(bytes98, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                        Buffer.BlockCopy(bytes96, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                        Buffer.BlockCopy(bytes97, 0, NetMessage.buffer[num].writeBuffer, num3, bytes97.Length);
                                                                                                        num3 += 2;
                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b26;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (msgType == 23)
                                                                                                        {
                                                                                                            byte[] bytes99 = BitConverter.GetBytes(msgType);
                                                                                                            byte[] bytes100 = BitConverter.GetBytes((short)number);
                                                                                                            byte[] bytes101 = BitConverter.GetBytes(world.getNPCs()[number].position.X);
                                                                                                            byte[] bytes102 = BitConverter.GetBytes(world.getNPCs()[number].position.Y);
                                                                                                            byte[] bytes103 = BitConverter.GetBytes(world.getNPCs()[number].velocity.X);
                                                                                                            byte[] bytes104 = BitConverter.GetBytes(world.getNPCs()[number].velocity.Y);
                                                                                                            byte[] bytes105 = BitConverter.GetBytes((short)world.getNPCs()[number].target);
                                                                                                            byte[] bytes106 = BitConverter.GetBytes((short)world.getNPCs()[number].life);
                                                                                                            if (!world.getNPCs()[number].active)
                                                                                                            {
                                                                                                                bytes106 = BitConverter.GetBytes(0);
                                                                                                            }
                                                                                                            byte[] bytes107 = Encoding.ASCII.GetBytes(world.getNPCs()[number].name);
                                                                                                            num2 += bytes100.Length + bytes101.Length + bytes102.Length + bytes103.Length + bytes104.Length + bytes105.Length + bytes106.Length + NPC.maxAI * 4 + bytes107.Length + 1 + 1;
                                                                                                            byte[] bytes108 = BitConverter.GetBytes(num2 - 4);
                                                                                                            Buffer.BlockCopy(bytes108, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                            Buffer.BlockCopy(bytes99, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                            Buffer.BlockCopy(bytes100, 0, NetMessage.buffer[num].writeBuffer, num3, bytes100.Length);
                                                                                                            num3 += 2;
                                                                                                            Buffer.BlockCopy(bytes101, 0, NetMessage.buffer[num].writeBuffer, num3, bytes101.Length);
                                                                                                            num3 += 4;
                                                                                                            Buffer.BlockCopy(bytes102, 0, NetMessage.buffer[num].writeBuffer, num3, bytes102.Length);
                                                                                                            num3 += 4;
                                                                                                            Buffer.BlockCopy(bytes103, 0, NetMessage.buffer[num].writeBuffer, num3, bytes103.Length);
                                                                                                            num3 += 4;
                                                                                                            Buffer.BlockCopy(bytes104, 0, NetMessage.buffer[num].writeBuffer, num3, bytes104.Length);
                                                                                                            num3 += 4;
                                                                                                            Buffer.BlockCopy(bytes105, 0, NetMessage.buffer[num].writeBuffer, num3, bytes105.Length);
                                                                                                            num3 += 2;
                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = (byte)(world.getNPCs()[number].direction + 1);
                                                                                                            num3++;
                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = (byte)(world.getNPCs()[number].directionY + 1);
                                                                                                            num3++;
                                                                                                            Buffer.BlockCopy(bytes106, 0, NetMessage.buffer[num].writeBuffer, num3, bytes106.Length);
                                                                                                            num3 += 2;
                                                                                                            for (int l = 0; l < NPC.maxAI; l++)
                                                                                                            {
                                                                                                                byte[] bytes109 = BitConverter.GetBytes(world.getNPCs()[number].ai[l]);
                                                                                                                Buffer.BlockCopy(bytes109, 0, NetMessage.buffer[num].writeBuffer, num3, bytes109.Length);
                                                                                                                num3 += 4;
                                                                                                            }
                                                                                                            Buffer.BlockCopy(bytes107, 0, NetMessage.buffer[num].writeBuffer, num3, bytes107.Length);
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (msgType == 24)
                                                                                                            {
                                                                                                                byte[] bytes110 = BitConverter.GetBytes(msgType);
                                                                                                                byte[] bytes111 = BitConverter.GetBytes((short)number);
                                                                                                                byte b27 = (byte)number2;
                                                                                                                num2 += bytes111.Length + 1;
                                                                                                                byte[] bytes112 = BitConverter.GetBytes(num2 - 4);
                                                                                                                Buffer.BlockCopy(bytes112, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                Buffer.BlockCopy(bytes110, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                Buffer.BlockCopy(bytes111, 0, NetMessage.buffer[num].writeBuffer, num3, bytes111.Length);
                                                                                                                num3 += 2;
                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b27;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (msgType == 25)
                                                                                                                {
                                                                                                                    byte[] bytes113 = BitConverter.GetBytes(msgType);
                                                                                                                    byte b28 = (byte)number;
                                                                                                                    byte[] bytes114 = Encoding.ASCII.GetBytes(text);
                                                                                                                    byte b29 = (byte)number2;
                                                                                                                    byte b30 = (byte)number3;
                                                                                                                    byte b31 = (byte)number4;
                                                                                                                    num2 += 1 + bytes114.Length + 3;
                                                                                                                    byte[] bytes115 = BitConverter.GetBytes(num2 - 4);
                                                                                                                    Buffer.BlockCopy(bytes115, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                    Buffer.BlockCopy(bytes113, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b28;
                                                                                                                    num3++;
                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b29;
                                                                                                                    num3++;
                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b30;
                                                                                                                    num3++;
                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b31;
                                                                                                                    num3++;
                                                                                                                    Buffer.BlockCopy(bytes114, 0, NetMessage.buffer[num].writeBuffer, num3, bytes114.Length);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (msgType == 26)
                                                                                                                    {
                                                                                                                        byte[] bytes116 = BitConverter.GetBytes(msgType);
                                                                                                                        byte b32 = (byte)number;
                                                                                                                        byte b33 = (byte)(number2 + 1f);
                                                                                                                        byte[] bytes117 = BitConverter.GetBytes((short)number3);
                                                                                                                        byte b34 = (byte)number4;
                                                                                                                        num2 += 2 + bytes117.Length + 1;
                                                                                                                        byte[] bytes118 = BitConverter.GetBytes(num2 - 4);
                                                                                                                        Buffer.BlockCopy(bytes118, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                        Buffer.BlockCopy(bytes116, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b32;
                                                                                                                        num3++;
                                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b33;
                                                                                                                        num3++;
                                                                                                                        Buffer.BlockCopy(bytes117, 0, NetMessage.buffer[num].writeBuffer, num3, bytes117.Length);
                                                                                                                        num3 += 2;
                                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b34;
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (msgType == 27)
                                                                                                                        {
                                                                                                                            byte[] bytes119 = BitConverter.GetBytes(msgType);
                                                                                                                            byte[] bytes120 = BitConverter.GetBytes((short)world.getProjectile()[number].identity);
                                                                                                                            byte[] bytes121 = BitConverter.GetBytes(world.getProjectile()[number].position.X);
                                                                                                                            byte[] bytes122 = BitConverter.GetBytes(world.getProjectile()[number].position.Y);
                                                                                                                            byte[] bytes123 = BitConverter.GetBytes(world.getProjectile()[number].velocity.X);
                                                                                                                            byte[] bytes124 = BitConverter.GetBytes(world.getProjectile()[number].velocity.Y);
                                                                                                                            byte[] bytes125 = BitConverter.GetBytes(world.getProjectile()[number].knockBack);
                                                                                                                            byte[] bytes126 = BitConverter.GetBytes((short)world.getProjectile()[number].damage);
                                                                                                                            Buffer.BlockCopy(bytes119, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                            Buffer.BlockCopy(bytes120, 0, NetMessage.buffer[num].writeBuffer, num3, bytes120.Length);
                                                                                                                            num3 += 2;
                                                                                                                            Buffer.BlockCopy(bytes121, 0, NetMessage.buffer[num].writeBuffer, num3, bytes121.Length);
                                                                                                                            num3 += 4;
                                                                                                                            Buffer.BlockCopy(bytes122, 0, NetMessage.buffer[num].writeBuffer, num3, bytes122.Length);
                                                                                                                            num3 += 4;
                                                                                                                            Buffer.BlockCopy(bytes123, 0, NetMessage.buffer[num].writeBuffer, num3, bytes123.Length);
                                                                                                                            num3 += 4;
                                                                                                                            Buffer.BlockCopy(bytes124, 0, NetMessage.buffer[num].writeBuffer, num3, bytes124.Length);
                                                                                                                            num3 += 4;
                                                                                                                            Buffer.BlockCopy(bytes125, 0, NetMessage.buffer[num].writeBuffer, num3, bytes125.Length);
                                                                                                                            num3 += 4;
                                                                                                                            Buffer.BlockCopy(bytes126, 0, NetMessage.buffer[num].writeBuffer, num3, bytes126.Length);
                                                                                                                            num3 += 2;
                                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getProjectile()[number].owner;
                                                                                                                            num3++;
                                                                                                                            NetMessage.buffer[num].writeBuffer[num3] = (byte)world.getProjectile()[number].type;
                                                                                                                            num3++;
                                                                                                                            for (int m = 0; m < Projectile.maxAI; m++)
                                                                                                                            {
                                                                                                                                byte[] bytes127 = BitConverter.GetBytes(world.getProjectile()[number].ai[m]);
                                                                                                                                Buffer.BlockCopy(bytes127, 0, NetMessage.buffer[num].writeBuffer, num3, bytes127.Length);
                                                                                                                                num3 += 4;
                                                                                                                            }
                                                                                                                            num2 += num3;
                                                                                                                            byte[] bytes128 = BitConverter.GetBytes(num2 - 4);
                                                                                                                            Buffer.BlockCopy(bytes128, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (msgType == 28)
                                                                                                                            {
                                                                                                                                byte[] bytes129 = BitConverter.GetBytes(msgType);
                                                                                                                                byte[] bytes130 = BitConverter.GetBytes((short)number);
                                                                                                                                byte[] bytes131 = BitConverter.GetBytes((short)number2);
                                                                                                                                byte[] bytes132 = BitConverter.GetBytes(number3);
                                                                                                                                byte b35 = (byte)(number4 + 1f);
                                                                                                                                num2 += bytes130.Length + bytes131.Length + bytes132.Length + 1;
                                                                                                                                byte[] bytes133 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                Buffer.BlockCopy(bytes133, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                Buffer.BlockCopy(bytes129, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                Buffer.BlockCopy(bytes130, 0, NetMessage.buffer[num].writeBuffer, num3, bytes130.Length);
                                                                                                                                num3 += 2;
                                                                                                                                Buffer.BlockCopy(bytes131, 0, NetMessage.buffer[num].writeBuffer, num3, bytes131.Length);
                                                                                                                                num3 += 2;
                                                                                                                                Buffer.BlockCopy(bytes132, 0, NetMessage.buffer[num].writeBuffer, num3, bytes132.Length);
                                                                                                                                num3 += 4;
                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b35;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (msgType == 29)
                                                                                                                                {
                                                                                                                                    byte[] bytes134 = BitConverter.GetBytes(msgType);
                                                                                                                                    byte[] bytes135 = BitConverter.GetBytes((short)number);
                                                                                                                                    byte b36 = (byte)number2;
                                                                                                                                    num2 += bytes135.Length + 1;
                                                                                                                                    byte[] bytes136 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                    Buffer.BlockCopy(bytes136, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                    Buffer.BlockCopy(bytes134, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                    Buffer.BlockCopy(bytes135, 0, NetMessage.buffer[num].writeBuffer, num3, bytes135.Length);
                                                                                                                                    num3 += 2;
                                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b36;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (msgType == 30)
                                                                                                                                    {
                                                                                                                                        byte[] bytes137 = BitConverter.GetBytes(msgType);
                                                                                                                                        byte b37 = (byte)number;
                                                                                                                                        byte b38 = 0;
                                                                                                                                        if (world.getPlayerList()[(int)b37].hostile)
                                                                                                                                        {
                                                                                                                                            b38 = 1;
                                                                                                                                        }
                                                                                                                                        num2 += 2;
                                                                                                                                        byte[] bytes138 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                        Buffer.BlockCopy(bytes138, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                        Buffer.BlockCopy(bytes137, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b37;
                                                                                                                                        num3++;
                                                                                                                                        NetMessage.buffer[num].writeBuffer[num3] = b38;
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (msgType == 31)
                                                                                                                                        {
                                                                                                                                            byte[] bytes139 = BitConverter.GetBytes(msgType);
                                                                                                                                            byte[] bytes140 = BitConverter.GetBytes(number);
                                                                                                                                            byte[] bytes141 = BitConverter.GetBytes((int)number2);
                                                                                                                                            num2 += bytes140.Length + bytes141.Length;
                                                                                                                                            byte[] bytes142 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                            Buffer.BlockCopy(bytes142, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                            Buffer.BlockCopy(bytes139, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                            Buffer.BlockCopy(bytes140, 0, NetMessage.buffer[num].writeBuffer, num3, bytes140.Length);
                                                                                                                                            num3 += 4;
                                                                                                                                            Buffer.BlockCopy(bytes141, 0, NetMessage.buffer[num].writeBuffer, num3, bytes141.Length);
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (msgType == 32)
                                                                                                                                            {
                                                                                                                                                byte[] bytes143 = BitConverter.GetBytes(msgType);
                                                                                                                                                byte[] bytes144 = BitConverter.GetBytes((short)number);
                                                                                                                                                byte b39 = (byte)number2;
                                                                                                                                                byte b40 = (byte)world.getChests()[number].item[(int)number2].stack;
                                                                                                                                                byte[] bytes145;
                                                                                                                                                if (world.getChests()[number].item[(int)number2].name == null)
                                                                                                                                                {
                                                                                                                                                    bytes145 = Encoding.ASCII.GetBytes("");
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    bytes145 = Encoding.ASCII.GetBytes(world.getChests()[number].item[(int)number2].name);
                                                                                                                                                }
                                                                                                                                                num2 += bytes144.Length + 1 + 1 + bytes145.Length;
                                                                                                                                                byte[] bytes146 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                Buffer.BlockCopy(bytes146, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                Buffer.BlockCopy(bytes143, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                Buffer.BlockCopy(bytes144, 0, NetMessage.buffer[num].writeBuffer, num3, bytes144.Length);
                                                                                                                                                num3 += 2;
                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b39;
                                                                                                                                                num3++;
                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b40;
                                                                                                                                                num3++;
                                                                                                                                                Buffer.BlockCopy(bytes145, 0, NetMessage.buffer[num].writeBuffer, num3, bytes145.Length);
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (msgType == 33)
                                                                                                                                                {
                                                                                                                                                    byte[] bytes147 = BitConverter.GetBytes(msgType);
                                                                                                                                                    byte[] bytes148 = BitConverter.GetBytes((short)number);
                                                                                                                                                    byte[] bytes149;
                                                                                                                                                    byte[] bytes150;
                                                                                                                                                    if (number > -1)
                                                                                                                                                    {
                                                                                                                                                        bytes149 = BitConverter.GetBytes(world.getChests()[number].x);
                                                                                                                                                        bytes150 = BitConverter.GetBytes(world.getChests()[number].y);
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        bytes149 = BitConverter.GetBytes(0);
                                                                                                                                                        bytes150 = BitConverter.GetBytes(0);
                                                                                                                                                    }
                                                                                                                                                    num2 += bytes148.Length + bytes149.Length + bytes150.Length;
                                                                                                                                                    byte[] bytes151 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                    Buffer.BlockCopy(bytes151, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                    Buffer.BlockCopy(bytes147, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                    Buffer.BlockCopy(bytes148, 0, NetMessage.buffer[num].writeBuffer, num3, bytes148.Length);
                                                                                                                                                    num3 += 2;
                                                                                                                                                    Buffer.BlockCopy(bytes149, 0, NetMessage.buffer[num].writeBuffer, num3, bytes149.Length);
                                                                                                                                                    num3 += 4;
                                                                                                                                                    Buffer.BlockCopy(bytes150, 0, NetMessage.buffer[num].writeBuffer, num3, bytes150.Length);
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (msgType == 34)
                                                                                                                                                    {
                                                                                                                                                        byte[] bytes152 = BitConverter.GetBytes(msgType);
                                                                                                                                                        byte[] bytes153 = BitConverter.GetBytes(number);
                                                                                                                                                        byte[] bytes154 = BitConverter.GetBytes((int)number2);
                                                                                                                                                        num2 += bytes153.Length + bytes154.Length;
                                                                                                                                                        byte[] bytes155 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                        Buffer.BlockCopy(bytes155, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                        Buffer.BlockCopy(bytes152, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                        Buffer.BlockCopy(bytes153, 0, NetMessage.buffer[num].writeBuffer, num3, bytes153.Length);
                                                                                                                                                        num3 += 4;
                                                                                                                                                        Buffer.BlockCopy(bytes154, 0, NetMessage.buffer[num].writeBuffer, num3, bytes154.Length);
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (msgType == 35)
                                                                                                                                                        {
                                                                                                                                                            byte[] bytes156 = BitConverter.GetBytes(msgType);
                                                                                                                                                            byte b41 = (byte)number;
                                                                                                                                                            byte[] bytes157 = BitConverter.GetBytes((short)number2);
                                                                                                                                                            num2 += 1 + bytes157.Length;
                                                                                                                                                            byte[] bytes158 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                            Buffer.BlockCopy(bytes158, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                            Buffer.BlockCopy(bytes156, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                            NetMessage.buffer[num].writeBuffer[5] = b41;
                                                                                                                                                            num3++;
                                                                                                                                                            Buffer.BlockCopy(bytes157, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (msgType == 36)
                                                                                                                                                            {
                                                                                                                                                                byte[] bytes159 = BitConverter.GetBytes(msgType);
                                                                                                                                                                byte b42 = (byte)number;
                                                                                                                                                                byte b43 = 0;
                                                                                                                                                                if (world.getPlayerList()[(int)b42].zoneEvil)
                                                                                                                                                                {
                                                                                                                                                                    b43 = 1;
                                                                                                                                                                }
                                                                                                                                                                byte b44 = 0;
                                                                                                                                                                if (world.getPlayerList()[(int)b42].zoneMeteor)
                                                                                                                                                                {
                                                                                                                                                                    b44 = 1;
                                                                                                                                                                }
                                                                                                                                                                byte b45 = 0;
                                                                                                                                                                if (world.getPlayerList()[(int)b42].zoneDungeon)
                                                                                                                                                                {
                                                                                                                                                                    b45 = 1;
                                                                                                                                                                }
                                                                                                                                                                byte b46 = 0;
                                                                                                                                                                if (world.getPlayerList()[(int)b42].zoneJungle)
                                                                                                                                                                {
                                                                                                                                                                    b46 = 1;
                                                                                                                                                                }
                                                                                                                                                                num2 += 4;
                                                                                                                                                                byte[] bytes160 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                Buffer.BlockCopy(bytes160, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                Buffer.BlockCopy(bytes159, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b42;
                                                                                                                                                                num3++;
                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b43;
                                                                                                                                                                num3++;
                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b44;
                                                                                                                                                                num3++;
                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b45;
                                                                                                                                                                num3++;
                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b46;
                                                                                                                                                                num3++;
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (msgType == 37)
                                                                                                                                                                {
                                                                                                                                                                    byte[] bytes161 = BitConverter.GetBytes(msgType);
                                                                                                                                                                    byte[] bytes162 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                    Buffer.BlockCopy(bytes162, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                    Buffer.BlockCopy(bytes161, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (msgType == 38)
                                                                                                                                                                    {
                                                                                                                                                                        byte[] bytes163 = BitConverter.GetBytes(msgType);
                                                                                                                                                                        byte[] bytes164 = Encoding.ASCII.GetBytes(text);
                                                                                                                                                                        num2 += bytes164.Length;
                                                                                                                                                                        byte[] bytes165 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                        Buffer.BlockCopy(bytes165, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                        Buffer.BlockCopy(bytes163, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                        Buffer.BlockCopy(bytes164, 0, NetMessage.buffer[num].writeBuffer, num3, bytes164.Length);
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (msgType == 39)
                                                                                                                                                                        {
                                                                                                                                                                            byte[] bytes166 = BitConverter.GetBytes(msgType);
                                                                                                                                                                            byte[] bytes167 = BitConverter.GetBytes((short)number);
                                                                                                                                                                            num2 += bytes167.Length;
                                                                                                                                                                            byte[] bytes168 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                            Buffer.BlockCopy(bytes168, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                            Buffer.BlockCopy(bytes166, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                            Buffer.BlockCopy(bytes167, 0, NetMessage.buffer[num].writeBuffer, num3, bytes167.Length);
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (msgType == 40)
                                                                                                                                                                            {
                                                                                                                                                                                byte[] bytes169 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                byte b47 = (byte)number;
                                                                                                                                                                                byte[] bytes170 = BitConverter.GetBytes((short)world.getPlayerList()[(int)b47].talkNPC);
                                                                                                                                                                                num2 += 1 + bytes170.Length;
                                                                                                                                                                                byte[] bytes171 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                Buffer.BlockCopy(bytes171, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                Buffer.BlockCopy(bytes169, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b47;
                                                                                                                                                                                num3++;
                                                                                                                                                                                Buffer.BlockCopy(bytes170, 0, NetMessage.buffer[num].writeBuffer, num3, bytes170.Length);
                                                                                                                                                                                num3 += 2;
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (msgType == 41)
                                                                                                                                                                                {
                                                                                                                                                                                    byte[] bytes172 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                    byte b48 = (byte)number;
                                                                                                                                                                                    byte[] bytes173 = BitConverter.GetBytes(world.getPlayerList()[(int)b48].itemRotation);
                                                                                                                                                                                    byte[] bytes174 = BitConverter.GetBytes((short)world.getPlayerList()[(int)b48].itemAnimation);
                                                                                                                                                                                    num2 += 1 + bytes173.Length + bytes174.Length;
                                                                                                                                                                                    byte[] bytes175 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                    Buffer.BlockCopy(bytes175, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                    Buffer.BlockCopy(bytes172, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b48;
                                                                                                                                                                                    num3++;
                                                                                                                                                                                    Buffer.BlockCopy(bytes173, 0, NetMessage.buffer[num].writeBuffer, num3, bytes173.Length);
                                                                                                                                                                                    num3 += 4;
                                                                                                                                                                                    Buffer.BlockCopy(bytes174, 0, NetMessage.buffer[num].writeBuffer, num3, bytes174.Length);
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (msgType == 42)
                                                                                                                                                                                    {
                                                                                                                                                                                        byte[] bytes176 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                        byte b49 = (byte)number;
                                                                                                                                                                                        byte[] bytes177 = BitConverter.GetBytes((short)world.getPlayerList()[(int)b49].statMana);
                                                                                                                                                                                        byte[] bytes178 = BitConverter.GetBytes((short)world.getPlayerList()[(int)b49].statManaMax);
                                                                                                                                                                                        num2 += 1 + bytes177.Length + bytes178.Length;
                                                                                                                                                                                        byte[] bytes179 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                        Buffer.BlockCopy(bytes179, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                        Buffer.BlockCopy(bytes176, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                        NetMessage.buffer[num].writeBuffer[5] = b49;
                                                                                                                                                                                        num3++;
                                                                                                                                                                                        Buffer.BlockCopy(bytes177, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                                                                                                        num3 += 2;
                                                                                                                                                                                        Buffer.BlockCopy(bytes178, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (msgType == 43)
                                                                                                                                                                                        {
                                                                                                                                                                                            byte[] bytes180 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                            byte b50 = (byte)number;
                                                                                                                                                                                            byte[] bytes181 = BitConverter.GetBytes((short)number2);
                                                                                                                                                                                            num2 += 1 + bytes181.Length;
                                                                                                                                                                                            byte[] bytes182 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                            Buffer.BlockCopy(bytes182, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                            Buffer.BlockCopy(bytes180, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                            NetMessage.buffer[num].writeBuffer[5] = b50;
                                                                                                                                                                                            num3++;
                                                                                                                                                                                            Buffer.BlockCopy(bytes181, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (msgType == 44)
                                                                                                                                                                                            {
                                                                                                                                                                                                byte[] bytes183 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                                byte b51 = (byte)number;
                                                                                                                                                                                                byte b52 = (byte)(number2 + 1f);
                                                                                                                                                                                                byte[] bytes184 = BitConverter.GetBytes((short)number3);
                                                                                                                                                                                                byte b53 = (byte)number4;
                                                                                                                                                                                                num2 += 2 + bytes184.Length + 1;
                                                                                                                                                                                                byte[] bytes185 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                                Buffer.BlockCopy(bytes185, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                                Buffer.BlockCopy(bytes183, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b51;
                                                                                                                                                                                                num3++;
                                                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b52;
                                                                                                                                                                                                num3++;
                                                                                                                                                                                                Buffer.BlockCopy(bytes184, 0, NetMessage.buffer[num].writeBuffer, num3, bytes184.Length);
                                                                                                                                                                                                num3 += 2;
                                                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b53;
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (msgType == 45)
                                                                                                                                                                                                {
                                                                                                                                                                                                    byte[] bytes186 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                                    byte b54 = (byte)number;
                                                                                                                                                                                                    byte b55 = (byte)world.getPlayerList()[(int)b54].team;
                                                                                                                                                                                                    num2 += 2;
                                                                                                                                                                                                    byte[] bytes187 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                                    Buffer.BlockCopy(bytes187, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                                    Buffer.BlockCopy(bytes186, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                                    NetMessage.buffer[num].writeBuffer[5] = b54;
                                                                                                                                                                                                    num3++;
                                                                                                                                                                                                    NetMessage.buffer[num].writeBuffer[num3] = b55;
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (msgType == 46)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        byte[] bytes188 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                                        byte[] bytes189 = BitConverter.GetBytes(number);
                                                                                                                                                                                                        byte[] bytes190 = BitConverter.GetBytes((int)number2);
                                                                                                                                                                                                        num2 += bytes189.Length + bytes190.Length;
                                                                                                                                                                                                        byte[] bytes191 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                                        Buffer.BlockCopy(bytes191, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                                        Buffer.BlockCopy(bytes188, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                                        Buffer.BlockCopy(bytes189, 0, NetMessage.buffer[num].writeBuffer, num3, bytes189.Length);
                                                                                                                                                                                                        num3 += 4;
                                                                                                                                                                                                        Buffer.BlockCopy(bytes190, 0, NetMessage.buffer[num].writeBuffer, num3, bytes190.Length);
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (msgType == 47)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            byte[] bytes192 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                                            byte[] bytes193 = BitConverter.GetBytes((short)number);
                                                                                                                                                                                                            byte[] bytes194 = BitConverter.GetBytes(world.getSigns()[number].x);
                                                                                                                                                                                                            byte[] bytes195 = BitConverter.GetBytes(world.getSigns()[number].y);
                                                                                                                                                                                                            byte[] bytes196 = Encoding.ASCII.GetBytes(world.getSigns()[number].text);
                                                                                                                                                                                                            num2 += bytes193.Length + bytes194.Length + bytes195.Length + bytes196.Length;
                                                                                                                                                                                                            byte[] bytes197 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                                            Buffer.BlockCopy(bytes197, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                                            Buffer.BlockCopy(bytes192, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                                            Buffer.BlockCopy(bytes193, 0, NetMessage.buffer[num].writeBuffer, num3, bytes193.Length);
                                                                                                                                                                                                            num3 += bytes193.Length;
                                                                                                                                                                                                            Buffer.BlockCopy(bytes194, 0, NetMessage.buffer[num].writeBuffer, num3, bytes194.Length);
                                                                                                                                                                                                            num3 += bytes194.Length;
                                                                                                                                                                                                            Buffer.BlockCopy(bytes195, 0, NetMessage.buffer[num].writeBuffer, num3, bytes195.Length);
                                                                                                                                                                                                            num3 += bytes195.Length;
                                                                                                                                                                                                            Buffer.BlockCopy(bytes196, 0, NetMessage.buffer[num].writeBuffer, num3, bytes196.Length);
                                                                                                                                                                                                            num3 += bytes196.Length;
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (msgType == 48)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                byte[] bytes198 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                                                byte[] bytes199 = BitConverter.GetBytes(number);
                                                                                                                                                                                                                byte[] bytes200 = BitConverter.GetBytes((int)number2);
                                                                                                                                                                                                                byte liquid = world.getTile()[number, (int)number2].liquid;
                                                                                                                                                                                                                byte b56 = 0;
                                                                                                                                                                                                                if (world.getTile()[number, (int)number2].lava)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    b56 = 1;
                                                                                                                                                                                                                }
                                                                                                                                                                                                                num2 += bytes199.Length + bytes200.Length + 1 + 1;
                                                                                                                                                                                                                byte[] bytes201 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                                                Buffer.BlockCopy(bytes201, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                                                Buffer.BlockCopy(bytes198, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                                                Buffer.BlockCopy(bytes199, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                                                                                                                                                num3 += 4;
                                                                                                                                                                                                                Buffer.BlockCopy(bytes200, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                                                                                                                                                                                                num3 += 4;
                                                                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = liquid;
                                                                                                                                                                                                                num3++;
                                                                                                                                                                                                                NetMessage.buffer[num].writeBuffer[num3] = b56;
                                                                                                                                                                                                                num3++;
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (msgType == 49)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    byte[] bytes202 = BitConverter.GetBytes(msgType);
                                                                                                                                                                                                                    byte[] bytes203 = BitConverter.GetBytes(num2 - 4);
                                                                                                                                                                                                                    Buffer.BlockCopy(bytes203, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                                                                                                                                                                                                    Buffer.BlockCopy(bytes202, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Statics.netMode != 1)
                {
                    goto IL_3261;
                }
                if (world.getServer().getNetPlay().clientSock.tcpClient.Connected)
                {
                    try
                    {
                        NetMessage.buffer[num].spamCount++;
                        world.getServer().getNetPlay().clientSock.networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2,
                            new AsyncCallback(world.getServer().getNetPlay().clientSock.ClientWriteCallBack), 
                            world.getServer().getNetPlay().clientSock.networkStream);
                        goto IL_339E;
                    }
                    catch
                    {
                        goto IL_339E;
                    }
                    //goto IL_3261;
                }
            IL_339E:
                if (Statics.verboseNetplay)
                {
                    //for (int n = 0; n < num2; n++)
                    //{
                    //}
                    for (int num10 = 0; num10 < num2; num10++)
                    {
                        byte arg_33D5_0 = NetMessage.buffer[num].writeBuffer[num10];
                    }
                    goto IL_33E7;
                }
                goto IL_33E7;
            IL_3261:
                if (remoteClient == -1)
                {
                    for (int num11 = 0; num11 < 9; num11++)
                    {
                        if (num11 != ignoreClient && (NetMessage.buffer[num11].broadcast || (world.getServer().getNetPlay().serverSock[num11].state >= 3 && msgType == 10))
                            && world.getServer().getNetPlay().serverSock[num11].tcpClient.Connected)
                        {
                            try
                            {
                                NetMessage.buffer[num11].spamCount++;
                                world.getServer().getNetPlay().serverSock[num11].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2,
                                    new AsyncCallback(world.getServer().getNetPlay().serverSock[num11].ServerWriteCallBack),
                                    world.getServer().getNetPlay().serverSock[num11].networkStream);
                            }
                            catch
                            {
                            }
                        }
                    }
                    goto IL_339E;
                }
            if (world.getServer().getNetPlay().serverSock[remoteClient].tcpClient.Connected)
                {
                    try
                    {
                        NetMessage.buffer[remoteClient].spamCount++;
                        world.getServer().getNetPlay().serverSock[remoteClient].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2,
                            new AsyncCallback(world.getServer().getNetPlay().serverSock[remoteClient].ServerWriteCallBack), 
                            world.getServer().getNetPlay().serverSock[remoteClient].networkStream);
                    }
                    catch
                    {
                    }
                    goto IL_339E;
                }
                goto IL_339E;
            IL_33E7:
                NetMessage.buffer[num].writeLocked = false;
                if (msgType == 19 && Statics.netMode == 1)
                {
                    int size = 5;
                    NetMessage.SendTileSquare(num, (int)number2, (int)number3, size, world);
                }
                if (msgType == 2 && Statics.netMode == 2)
                {
                    world.getServer().getNetPlay().serverSock[num].kill = true;
                }
            }
        }
        
        public static void RecieveBytes(byte[] bytes, int streamLength, World world, int i = 9)
        {
            lock (NetMessage.buffer[i])
            {
                try
                {
                    Buffer.BlockCopy(bytes, 0, NetMessage.buffer[i].readBuffer, NetMessage.buffer[i].totalData, streamLength);
                    NetMessage.buffer[i].totalData += streamLength;
                    NetMessage.buffer[i].checkBytes = true;
                }
                catch
                {
                    if (Statics.netMode == 1)
                    {
                        //Main.menuMode = 15;
                        //Main.statusText = "Bad header lead to a read buffer overflow.";
                        Console.WriteLine("Bad header lead to a read buffer overflow.");
                        world.getServer().getNetPlay().disconnect = true;
                    }
                    else
                    {
                        world.getServer().getNetPlay().serverSock[i].kill = true;
                    }
                }
            }
        }
        
        public static void CheckBytes(World world, int i = 9)
        {
            lock (NetMessage.buffer[i])
            {
                int num = 0;
                if (NetMessage.buffer[i].totalData >= 4)
                {
                    if (NetMessage.buffer[i].messageLength == 0)
                    {
                        NetMessage.buffer[i].messageLength = BitConverter.ToInt32(NetMessage.buffer[i].readBuffer, 0) + 4;
                    }
                    while (NetMessage.buffer[i].totalData >= NetMessage.buffer[i].messageLength + num && NetMessage.buffer[i].messageLength > 0)
                    {
                        if (!Statics.ignoreErrors)
                        {
                            NetMessage.buffer[i].GetData(num + 4, NetMessage.buffer[i].messageLength - 4, world);
                        }
                        else
                        {
                            try
                            {
                                NetMessage.buffer[i].GetData(num + 4, NetMessage.buffer[i].messageLength - 4, world);
                            }
                            catch
                            {
                            }
                        }
                        num += NetMessage.buffer[i].messageLength;
                        if (NetMessage.buffer[i].totalData - num >= 4)
                        {
                            NetMessage.buffer[i].messageLength = BitConverter.ToInt32(NetMessage.buffer[i].readBuffer, num) + 4;
                        }
                        else
                        {
                            NetMessage.buffer[i].messageLength = 0;
                        }
                    }
                    if (num == NetMessage.buffer[i].totalData)
                    {
                        NetMessage.buffer[i].totalData = 0;
                    }
                    else
                    {
                        if (num > 0)
                        {
                            Buffer.BlockCopy(NetMessage.buffer[i].readBuffer, num, NetMessage.buffer[i].readBuffer, 0, NetMessage.buffer[i].totalData - num);
                            NetMessage.buffer[i].totalData -= num;
                        }
                    }
                    NetMessage.buffer[i].checkBytes = false;
                }
            }
        }
        
        public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size, World world)
        {
            int num = (size - 1) / 2;
            NetMessage.SendData(20, world, whoAmi, -1, "", size, (float)(tileX - num), (float)(tileY - num), 0f);
        }
        
        public static void SendSection(int whoAmi, int sectionX, int sectionY, World world)
        {
            world.getServer().getNetPlay().serverSock[whoAmi].tileSection[sectionX, sectionY] = true;
            int num = sectionX * 200;
            int num2 = sectionY * 150;
            for (int i = num2; i < num2 + 150; i++)
            {
                NetMessage.SendData(10, world, whoAmi, -1, "", 200, (float)num, (float)i, 0f);
            }
        }
        
        public static void greetPlayer(int plr, World world)
        {
            NetMessage.SendData(25, world, plr, -1, "Welcome to " + world.getName() + "!", 255, 255f, 240f, 20f);
            string text = "";
            for (int i = 0; i < 255; i++)
            {
                if (world.getPlayerList()[i].active)
                {
                    if (text == "")
                    {
                        text += world.getPlayerList()[i].name;
                    }
                    else
                    {
                        text = text + ", " + world.getPlayerList()[i].name;
                    }
                }
            }
            NetMessage.SendData(25, world, plr, -1, "Current players: " + text + ".", 255, 255f, 240f, 20f);
        }
        
        public static void sendWater(int x, int y, World world)
        {
            if (Statics.netMode == 1)
            {
                NetMessage.SendData(48, world, -1, -1, "", x, (float)y, 0f, 0f);
                return;
            }
            for (int i = 0; i < 9; i++)
            {
                if ((NetMessage.buffer[i].broadcast || world.getServer().getNetPlay().serverSock[i].state >= 3) &&
                    world.getServer().getNetPlay().serverSock[i].tcpClient.Connected)
                {
                    int num = x / 200;
                    int num2 = y / 150;
                    if (world.getServer().getNetPlay().serverSock[i].tileSection[num, num2])
                    {
                        NetMessage.SendData(48, world, i, -1, "", x, (float)y, 0f, 0f);
                    }
                }
            }
        }
        
        public static void syncPlayers(World world)
        {
            for (int i = 0; i < 8; i++)
            {
                int num = 0;
                if (world.getPlayerList()[i].active)
                {
                    num = 1;
                }
                if (world.getServer().getNetPlay().serverSock[i].state == 10)
                {
                    NetMessage.SendData(14, world, -1, i, "", i, (float)num, 0f, 0f);
                    NetMessage.SendData(13, world, -1, i, "", i, 0f, 0f, 0f);
                    NetMessage.SendData(16, world, -1, i, "", i, 0f, 0f, 0f);
                    NetMessage.SendData(30, world, -1, i, "", i, 0f, 0f, 0f);
                    NetMessage.SendData(45, world, -1, i, "", i, 0f, 0f, 0f);
                    NetMessage.SendData(42, world, -1, i, "", i, 0f, 0f, 0f);
                    NetMessage.SendData(4, world, -1, i, world.getPlayerList()[i].name, i, 0f, 0f, 0f);
                    for (int j = 0; j < 44; j++)
                    {
                        NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].inventory[j].name, i, (float)j, 0f, 0f);
                    }
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[0].name, i, 44f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[1].name, i, 45f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[2].name, i, 46f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[3].name, i, 47f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[4].name, i, 48f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[5].name, i, 49f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[6].name, i, 50f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, i, world.getPlayerList()[i].armor[7].name, i, 51f, 0f, 0f);
                    if (!world.getServer().getNetPlay().serverSock[i].announced)
                    {
                        world.getServer().getNetPlay().serverSock[i].announced = true;
                        NetMessage.SendData(25, world, -1, i, world.getPlayerList()[i].name + " has joined.", 255, 255f, 240f, 20f);
                        Console.WriteLine(world.getServer().getNetPlay().serverSock[i].oldName + " has joined.");
                    }
                }
                else
                {
                    NetMessage.SendData(14, world, -1, i, "", i, (float)num, 0f, 0f);
                    if (world.getServer().getNetPlay().serverSock[i].announced)
                    {
                        world.getServer().getNetPlay().serverSock[i].announced = false;
                        NetMessage.SendData(25, world, -1, i, world.getServer().getNetPlay().serverSock[i].oldName + " has left.", 255, 255f, 240f, 20f);
                        Console.WriteLine(world.getServer().getNetPlay().serverSock[i].oldName + " has left.");
                    }
                }
            }
        }
    }
}
