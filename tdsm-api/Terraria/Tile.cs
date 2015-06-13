//using Microsoft.Xna.Framework;
//using Terraria;
//
//namespace tdsm
//{
//    public struct Tile
//    {
//        public static Tile Empty = default(Tile);
//
//        public ushort type;
//        public byte wall;
//        public byte liquid;
//        public short sTileHeader;
//        public byte bTileHeader;
//        public byte bTileHeader2;
//        public byte bTileHeader3;
//        public short frameX;
//        public short frameY;
//        public int collisionType
//        {
//            get
//            {
//                if (!this.active())
//                {
//                    return 0;
//                }
//                if (this.halfBrick())
//                {
//                    return 2;
//                }
//                if (this.slope() > 0)
//                {
//                    return (int)(2 + this.slope());
//                }
//                if (Main.tileSolid[(int)this.type] && !Main.tileSolidTop[(int)this.type])
//                {
//                    return 1;
//                }
//                return -1;
//            }
//        }
//        //public Tile()
//        //{
//        //    this.type = 0;
//        //    this.wall = 0;
//        //    this.liquid = 0;
//        //    this.sTileHeader = 0;
//        //    this.bTileHeader = 0;
//        //    this.bTileHeader2 = 0;
//        //    this.bTileHeader3 = 0;
//        //    this.frameX = 0;
//        //    this.frameY = 0;
//        //}
//        public Tile(Tile copy)
//        {
//            if (copy.Equals(default(Tile)))
//            {
//                this.type = 0;
//                this.wall = 0;
//                this.liquid = 0;
//                this.sTileHeader = 0;
//                this.bTileHeader = 0;
//                this.bTileHeader2 = 0;
//                this.bTileHeader3 = 0;
//                this.frameX = 0;
//                this.frameY = 0;
//                return;
//            }
//            this.type = copy.type;
//            this.wall = copy.wall;
//            this.liquid = copy.liquid;
//            this.sTileHeader = copy.sTileHeader;
//            this.bTileHeader = copy.bTileHeader;
//            this.bTileHeader2 = copy.bTileHeader2;
//            this.bTileHeader3 = copy.bTileHeader3;
//            this.frameX = copy.frameX;
//            this.frameY = copy.frameY;
//        }
//        public object Clone()
//        {
//            return base.MemberwiseClone();
//        }
//
//
//        public void Clear()
//        {
//            this.type = 0;
//            this.wall = 0;
//            this.liquid = 0;
//            this.sTileHeader = 0;
//            this.bTileHeader = 0;
//            this.bTileHeader2 = 0;
//            this.bTileHeader3 = 0;
//            this.frameX = 0;
//            this.frameY = 0;
//        }
//        public void CopyFrom(Tile from)
//        {
//            this.type = from.type;
//            this.wall = from.wall;
//            this.liquid = from.liquid;
//            this.sTileHeader = from.sTileHeader;
//            this.bTileHeader = from.bTileHeader;
//            this.bTileHeader2 = from.bTileHeader2;
//            this.bTileHeader3 = from.bTileHeader3;
//            this.frameX = from.frameX;
//            this.frameY = from.frameY;
//        }
//        public bool isTheSameAs(Tile compTile)
//        {
//            if (compTile.Equals(default(Tile)))
//            {
//                return false;
//            }
//            if (this.sTileHeader != compTile.sTileHeader)
//            {
//                return false;
//            }
//            if (this.active())
//            {
//                if (this.type != compTile.type)
//                {
//                    return false;
//                }
//                if (Main.tileFrameImportant[(int)this.type] && (this.frameX != compTile.frameX || this.frameY != compTile.frameY))
//                {
//                    return false;
//                }
//            }
//            if (this.wall != compTile.wall || this.liquid != compTile.liquid)
//            {
//                return false;
//            }
//            if (compTile.liquid == 0)
//            {
//                if (this.wallColor() != compTile.wallColor())
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                if (this.bTileHeader != compTile.bTileHeader)
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//        public int wallFrameX()
//        {
//            return (int)((this.bTileHeader2 & 15) * 36);
//        }
//        public void wallFrameX(int wallFrameX)
//        {
//            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 240) | (wallFrameX / 36 & 15));
//        }
//        public int wallFrameY()
//        {
//            return (int)((this.bTileHeader3 & 7) * 36);
//        }
//        public void wallFrameY(int wallFrameY)
//        {
//            this.bTileHeader3 = (byte)((int)(this.bTileHeader3 & 248) | (wallFrameY / 36 & 7));
//        }
//        public byte frameNumber()
//        {
//            return (byte)((this.bTileHeader2 & 48) >> 4);
//        }
//        public void frameNumber(byte frameNumber)
//        {
//            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 207) | (int)(frameNumber & 3) << 4);
//        }
//        public byte wallFrameNumber()
//        {
//            return (byte)((this.bTileHeader2 & 192) >> 6);
//        }
//        public void wallFrameNumber(byte wallFrameNumber)
//        {
//            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 63) | (int)(wallFrameNumber & 3) << 6);
//        }
//        public bool topSlope()
//        {
//            byte b = this.slope();
//            return b == 1 || b == 2;
//        }
//        public bool bottomSlope()
//        {
//            byte b = this.slope();
//            return b == 3 || b == 4;
//        }
//        public bool leftSlope()
//        {
//            byte b = this.slope();
//            return b == 2 || b == 4;
//        }
//        public bool rightSlope()
//        {
//            byte b = this.slope();
//            return b == 1 || b == 3;
//        }
//        public byte slope()
//        {
//            return (byte)((this.sTileHeader & 28672) >> 12);
//        }
//        public void slope(byte slope)
//        {
//            this.sTileHeader = (short)(((int)this.sTileHeader & 36863) | (int)(slope & 7) << 12);
//        }
//        public int blockType()
//        {
//            if (this.halfBrick())
//            {
//                return 1;
//            }
//            int num = (int)this.slope();
//            if (num > 0)
//            {
//                num++;
//            }
//            return num;
//        }
//        public byte color()
//        {
//            return (byte)(this.sTileHeader & 31);
//        }
//        public void color(byte color)
//        {
//            if (color > 30)
//            {
//                color = 30;
//            }
//            this.sTileHeader = (short)(((int)this.sTileHeader & 65504) | (int)color);
//        }
//        public byte wallColor()
//        {
//            return (byte)(this.bTileHeader & 31);
//        }
//        public void wallColor(byte wallColor)
//        {
//            if (wallColor > 30)
//            {
//                wallColor = 30;
//            }
//            this.bTileHeader = (byte)((this.bTileHeader & 224) | wallColor);
//        }
//        public bool lava()
//        {
//            return (this.bTileHeader & 32) == 32;
//        }
//        public void lava(bool lava)
//        {
//            if (lava)
//            {
//                this.bTileHeader = (byte)((this.bTileHeader & 159) | 32);
//                return;
//            }
//            this.bTileHeader &= 223;
//        }
//        public bool honey()
//        {
//            return (this.bTileHeader & 64) == 64;
//        }
//        public void honey(bool honey)
//        {
//            if (honey)
//            {
//                this.bTileHeader = (byte)((this.bTileHeader & 159) | 64);
//                return;
//            }
//            this.bTileHeader &= 191;
//        }
//        public void liquidType(int liquidType)
//        {
//            if (liquidType == 0)
//            {
//                this.bTileHeader &= 159;
//                return;
//            }
//            if (liquidType == 1)
//            {
//                this.lava(true);
//                return;
//            }
//            if (liquidType == 2)
//            {
//                this.honey(true);
//            }
//        }
//        public byte liquidType()
//        {
//            return (byte)((this.bTileHeader & 96) >> 5);
//        }
//        public bool checkingLiquid()
//        {
//            return (this.bTileHeader3 & 8) == 8;
//        }
//        public void checkingLiquid(bool checkingLiquid)
//        {
//            if (checkingLiquid)
//            {
//                this.bTileHeader3 |= 8;
//                return;
//            }
//            this.bTileHeader3 &= 247;
//        }
//        public bool skipLiquid()
//        {
//            return (this.bTileHeader3 & 16) == 16;
//        }
//        public void skipLiquid(bool skipLiquid)
//        {
//            if (skipLiquid)
//            {
//                this.bTileHeader3 |= 16;
//                return;
//            }
//            this.bTileHeader3 &= 239;
//        }
//        public bool wire()
//        {
//            return (this.sTileHeader & 128) == 128;
//        }
//        public void wire(bool wire)
//        {
//            if (wire)
//            {
//                this.sTileHeader |= 128;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 65407);
//        }
//        public bool wire2()
//        {
//            return (this.sTileHeader & 256) == 256;
//        }
//        public void wire2(bool wire2)
//        {
//            if (wire2)
//            {
//                this.sTileHeader |= 256;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 65279);
//        }
//        public bool wire3()
//        {
//            return (this.sTileHeader & 512) == 512;
//        }
//        public void wire3(bool wire3)
//        {
//            if (wire3)
//            {
//                this.sTileHeader |= 512;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 65023);
//        }
//        public bool halfBrick()
//        {
//            return (this.sTileHeader & 1024) == 1024;
//        }
//        public void halfBrick(bool halfBrick)
//        {
//            if (halfBrick)
//            {
//                this.sTileHeader |= 1024;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 64511);
//        }
//        public bool actuator()
//        {
//            return (this.sTileHeader & 2048) == 2048;
//        }
//        public void actuator(bool actuator)
//        {
//            if (actuator)
//            {
//                this.sTileHeader |= 2048;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 63487);
//        }
//        public bool nactive()
//        {
//            int num = (int)(this.sTileHeader & 96);
//            return num == 32;
//        }
//        public bool inActive()
//        {
//            return (this.sTileHeader & 64) == 64;
//        }
//        public void inActive(bool inActive)
//        {
//            if (inActive)
//            {
//                this.sTileHeader |= 64;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 65471);
//        }
//        public bool active()
//        {
//            return (this.sTileHeader & 32) == 32;
//        }
//        public void active(bool active)
//        {
//            if (active)
//            {
//                this.sTileHeader |= 32;
//                return;
//            }
//            this.sTileHeader = (short)((int)this.sTileHeader & 65503);
//        }
//        public Color actColor(Color oldColor)
//        {
//            if (!this.inActive())
//            {
//                return oldColor;
//            }
//            double num = 0.4;
//            return new Color((int)((byte)(num * (double)oldColor.R)), (int)((byte)(num * (double)oldColor.G)), (int)((byte)(num * (double)oldColor.B)), (int)oldColor.A);
//        }
//    }
//}
