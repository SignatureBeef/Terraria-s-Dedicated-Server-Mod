using System;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class TileBreakMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TILE_BREAK;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [PlayerWorldAlteration]

            byte b2 = ReadByte(readBuffer);
            int num36 = (int)ReadInt16(readBuffer);
            int num37 = (int)ReadInt16(readBuffer);
            short num38 = ReadInt16(readBuffer);
            int num39 = (int)ReadByte(readBuffer);
            bool flag4 = num38 == 1;
            if (Main.tile[num36, num37] == null)
            {
                Main.tile[num36, num37] = new Tile();
            }
            if (Main.netMode == 2)
            {
                if (!flag4)
                {
                    if (b2 == 0 || b2 == 2 || b2 == 4)
                    {
                        Server.slots[whoAmI].spamDelBlock += 1f;
                    }
                    if (b2 == 1 || b2 == 3)
                    {
                        Server.slots[whoAmI].spamAddBlock += 1f;
                    }
                }
                if (!Server.slots[whoAmI].tileSection[Netplay.GetSectionX(num36), Netplay.GetSectionY(num37)])
                {
                    flag4 = true;
                }
            }

            if (b2 == 0)
            {
                WorldGen.KillTile(num36, num37, flag4, false, false);
            }
            if (b2 == 1)
            {
                WorldGen.PlaceTile(num36, num37, (int)num38, false, true, -1, num39);
            }
            if (b2 == 2)
            {
                WorldGen.KillWall(num36, num37, flag4);
            }
            if (b2 == 3)
            {
                WorldGen.PlaceWall(num36, num37, (int)num38, false);
            }
            if (b2 == 4)
            {
                WorldGen.KillTile(num36, num37, flag4, false, true);
            }
            if (b2 == 5)
            {
                WorldGen.PlaceWire(num36, num37);
            }
            if (b2 == 6)
            {
                WorldGen.KillWire(num36, num37);
            }
            if (b2 == 7)
            {
                WorldGen.PoundTile(num36, num37);
            }
            if (b2 == 8)
            {
                WorldGen.PlaceActuator(num36, num37);
            }
            if (b2 == 9)
            {
                WorldGen.KillActuator(num36, num37);
            }
            if (b2 == 10)
            {
                WorldGen.PlaceWire2(num36, num37);
            }
            if (b2 == 11)
            {
                WorldGen.KillWire2(num36, num37);
            }
            if (b2 == 12)
            {
                WorldGen.PlaceWire3(num36, num37);
            }
            if (b2 == 13)
            {
                WorldGen.KillWire3(num36, num37);
            }
            if (b2 == 14)
            {
                WorldGen.SlopeTile(num36, num37, (int)num38);
            }
            if (b2 == 15)
            {
                Minecart.FrameTrack(num36, num37, true, false);
            }
            if (Main.netMode != 2)
            {
                return;
            }
            NewNetMessage.SendData(17, -1, whoAmI, String.Empty, (int)b2, (float)num36, (float)num37, (float)num38, num39);
            if (b2 == 1 && num38 == 53)
            {
                NewNetMessage.SendTileSquare(-1, num36, num37, 1);
                return;
            }
        }
    }
}
