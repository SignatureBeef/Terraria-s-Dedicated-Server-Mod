//#define STRUCT
#define VANILLACOMPAT
//#define TESTING
using Microsoft.Xna.Framework;

#if STRUCT
using System.Runtime.InteropServices;
#endif

namespace TDSM.API.Memory
{
    #if STRUCT
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MemTile
    {
    static TileRef[,] data;

    public static void Prepare(int width, int height)
    {
    data = new TileRef[width + 1, height + 1];
    }

    
#if TESTING
    static MemTile()
    {
    Prepare(8400,2400);
    }
    #endif
    
    private short x, y;

    public static readonly MemTile Empty;

    public const int Type_Solid = 0;
    public const int Type_Halfbrick = 1;
    public const int Type_SlopeDownRight = 2;
    public const int Type_SlopeDownLeft = 3;
    public const int Type_SlopeUpRight = 4;
    public const int Type_SlopeUpLeft = 5;
    public const int Liquid_Water = 0;
    public const int Liquid_Lava = 1;
    public const int Liquid_Honey = 2;

    public ushort type
    {
    get
    {
    return data[x, y]._type;
    }
    set
    {
    data[x, y]._type = value;
    }
    }
    public byte wall
    {
    get
    {
    return data[x, y]._wall;
    }
    set
    {
    data[x, y]._wall = value;
    }
    }
    public byte liquid
    {
    get
    {
    return data[x, y]._liquid;
    }
    set
    {
    data[x, y]._liquid = value;
    }
    }
    public short sTileHeader
    {
    get
    {
    return data[x, y]._sTileHeader;
    }
    set
    {
    data[x, y]._sTileHeader = value;
    }
    }
    public byte bTileHeader
    {
    get
    {
    return data[x, y]._bTileHeader;
    }
    set
    {
    data[x, y]._bTileHeader = value;
    }
    }
    public byte bTileHeader2
    {
    get
    {
    return data[x, y]._bTileHeader2;
    }
    set
    {
    data[x, y]._bTileHeader2 = value;
    }
    }
    public byte bTileHeader3
    {
    get
    {
    return data[x, y]._bTileHeader3;
    }
    set
    {
    data[x, y]._bTileHeader3 = value;
    }
    }
    public short frameY
    {
    get
    {
    return data[x, y]._frameY;
    }
    set
    {
    data[x, y]._frameY = value;
    }
    }
    public short frameX
    {
    get
    {
    return data[x, y]._frameX;
    }
    set
    {
    data[x, y]._frameX = value;
    }
    }

    public int collisionType
    {
    get
    {
    if (!this.active())
    {
    return 0;
    }
    if (this.halfBrick())
    {
    return 2;
    }
    if (this.slope() > 0)
    {
    return (int)(2 + this.slope());
    }
    if (Terraria.Main.tileSolid[(int)data[x, y]._type] && !Terraria.Main.tileSolidTop[(int)data[x, y]._type])
    {
    return 1;
    }
    return -1;
    }
    }

    //public Tile() { x = 0; y = 0; }


    public MemTile(short x, short y)
    {
    this.x = x;
    this.y = y;
    data[x, y]._type = 0;
    data[x, y]._wall = 0;
    data[x, y]._liquid = 0;
    data[x, y]._sTileHeader = 0;
    data[x, y]._bTileHeader = 0;
    data[x, y]._bTileHeader2 = 0;
    data[x, y]._bTileHeader3 = 0;
    data[x, y]._frameX = 0;
    data[x, y]._frameY = 0;
    }
    public MemTile(int x, int y)
    {
    this.x = (short)x;
    this.y = (short)y;
    data[x, y]._type = 0;
    data[x, y]._wall = 0;
    data[x, y]._liquid = 0;
    data[x, y]._sTileHeader = 0;
    data[x, y]._bTileHeader = 0;
    data[x, y]._bTileHeader2 = 0;
    data[x, y]._bTileHeader3 = 0;
    data[x, y]._frameX = 0;
    data[x, y]._frameY = 0;
    }

    public MemTile(MemTile copy)
    {
    //if (copy == null)
    //{
    //    data[x, y]._type = 0;
    //    data[x, y]._wall = 0;
    //    data[x, y]._liquid = 0;
    //    data[x, y]._sTileHeader = 0;
    //    data[x, y]._bTileHeader = 0;
    //    data[x, y]._bTileHeader2 = 0;
    //    data[x, y]._bTileHeader3 = 0;
    //    data[x, y]._frameX = 0;
    //    data[x, y]._frameY = 0;
    //    return;
    //}
    this.x = copy.x;
    this.y = copy.y;
    data[x, y]._type = copy.type;
    data[x, y]._wall = copy.wall;
    data[x, y]._liquid = copy.liquid;
    data[x, y]._sTileHeader = copy.sTileHeader;
    data[x, y]._bTileHeader = copy.bTileHeader;
    data[x, y]._bTileHeader2 = copy.bTileHeader2;
    data[x, y]._bTileHeader3 = copy.bTileHeader3;
    data[x, y]._frameX = copy.frameX;
    data[x, y]._frameY = copy.frameY;
    }

    public object Clone()
    {
    return base.MemberwiseClone();
    }

    public void ClearEverything()
    {
    data[x, y]._type = 0;
    data[x, y]._wall = 0;
    data[x, y]._liquid = 0;
    data[x, y]._sTileHeader = 0;
    data[x, y]._bTileHeader = 0;
    data[x, y]._bTileHeader2 = 0;
    data[x, y]._bTileHeader3 = 0;
    data[x, y]._frameX = 0;
    data[x, y]._frameY = 0;
    }

    public void ClearTile()
    {
    this.slope(0);
    this.halfBrick(false);
    this.active(false);
    }

    public void CopyFrom(MemTile from)
    {
    data[x, y]._type = from.type;
    data[x, y]._wall = from.wall;
    data[x, y]._liquid = from.liquid;
    data[x, y]._sTileHeader = from.sTileHeader;
    data[x, y]._bTileHeader = from.bTileHeader;
    data[x, y]._bTileHeader2 = from.bTileHeader2;
    data[x, y]._bTileHeader3 = from.bTileHeader3;
    data[x, y]._frameX = from.frameX;
    data[x, y]._frameY = from.frameY;
    }

    public bool isTheSameAs(MemTile compTile)
    {
    //if (compTile == null)
    //{
    //    return false;
    //}
    if (data[x, y]._sTileHeader != compTile.sTileHeader)
    {
    return false;
    }
    if (this.active())
    {
    if (data[x, y]._type != compTile.type)
    {
    return false;
    }
    if (Terraria.Main.tileFrameImportant[(int)data[x, y]._type] && (data[x, y]._frameX != compTile.frameX || data[x, y]._frameY != compTile.frameY))
    {
    return false;
    }
    }
    if (data[x, y]._wall != compTile.wall || data[x, y]._liquid != compTile.liquid)
    {
    return false;
    }
    if (compTile.liquid == 0)
    {
    if (this.wallColor() != compTile.wallColor())
    {
    return false;
    }
    }
    else if (data[x, y]._bTileHeader != compTile.bTileHeader)
    {
    return false;
    }
    return true;
    }

    public int wallFrameX()
    {
    return (int)((data[x, y]._bTileHeader2 & 15) * 36);
    }

    public void wallFrameX(int wallFrameX)
    {
    data[x, y]._bTileHeader2 = (byte)((int)(data[x, y]._bTileHeader2 & 240) | (wallFrameX / 36 & 15));
    }

    public int wallFrameY()
    {
    return (int)((data[x, y]._bTileHeader3 & 7) * 36);
    }

    public void wallFrameY(int wallFrameY)
    {
    data[x, y]._bTileHeader3 = (byte)((int)(data[x, y]._bTileHeader3 & 248) | (wallFrameY / 36 & 7));
    }

    public byte frameNumber()
    {
    return (byte)((data[x, y]._bTileHeader2 & 48) >> 4);
    }

    public void frameNumber(byte frameNumber)
    {
    data[x, y]._bTileHeader2 = (byte)((int)(data[x, y]._bTileHeader2 & 207) | (int)(frameNumber & 3) << 4);
    }

    public byte wallFrameNumber()
    {
    return (byte)((data[x, y]._bTileHeader2 & 192) >> 6);
    }

    public void wallFrameNumber(byte wallFrameNumber)
    {
    data[x, y]._bTileHeader2 = (byte)((int)(data[x, y]._bTileHeader2 & 63) | (int)(wallFrameNumber & 3) << 6);
    }

    public bool topSlope()
    {
    byte b = this.slope();
    return b == 1 || b == 2;
    }

    public bool bottomSlope()
    {
    byte b = this.slope();
    return b == 3 || b == 4;
    }

    public bool leftSlope()
    {
    byte b = this.slope();
    return b == 2 || b == 4;
    }

    public bool rightSlope()
    {
    byte b = this.slope();
    return b == 1 || b == 3;
    }

    public byte slope()
    {
    return (byte)((data[x, y]._sTileHeader & 28672) >> 12);
    }

    public bool HasSameSlope(MemTile tile)
    {
    return (data[x, y]._sTileHeader & 29696) == (tile.sTileHeader & 29696);
    }

    public void slope(byte slope)
    {
    data[x, y]._sTileHeader = (short)(((int)data[x, y]._sTileHeader & 36863) | (int)(slope & 7) << 12);
    }

    public int blockType()
    {
    if (this.halfBrick())
    {
    return 1;
    }
    int num = (int)this.slope();
    if (num > 0)
    {
    num++;
    }
    return num;
    }

    public byte color()
    {
    return (byte)(data[x, y]._sTileHeader & 31);
    }

    public void color(byte color)
    {
    if (color > 30)
    {
    color = 30;
    }
    data[x, y]._sTileHeader = (short)(((int)data[x, y]._sTileHeader & 65504) | (int)color);
    }

    public byte wallColor()
    {
    return (byte)(data[x, y]._bTileHeader & 31);
    }

    public void wallColor(byte wallColor)
    {
    if (wallColor > 30)
    {
    wallColor = 30;
    }
    data[x, y]._bTileHeader = (byte)(((data[x, y]._bTileHeader & 224) | wallColor));
    }

    public bool lava()
    {
    return (data[x, y]._bTileHeader & 32) == 32;
    }

    public void lava(bool lava)
    {
    if (lava)
    {
    data[x, y]._bTileHeader = (byte)(((data[x, y]._bTileHeader & 159) | 32));
    return;
    }
    data[x, y]._bTileHeader &= 223;
    }

    public bool honey()
    {
    return (data[x, y]._bTileHeader & 64) == 64;
    }

    public void honey(bool honey)
    {
    if (honey)
    {
    data[x, y]._bTileHeader = (byte)(((data[x, y]._bTileHeader & 159) | 64));
    return;
    }
    data[x, y]._bTileHeader &= 191;
    }

    public void liquidType(int liquidType)
    {
    if (liquidType == 0)
    {
    data[x, y]._bTileHeader &= 159;
    return;
    }
    if (liquidType == 1)
    {
    this.lava(true);
    return;
    }
    if (liquidType == 2)
    {
    this.honey(true);
    }
    }

    public byte liquidType()
    {
    return (byte)((data[x, y]._bTileHeader & 96) >> 5);
    }

    public bool checkingLiquid()
    {
    return (data[x, y]._bTileHeader3 & 8) == 8;
    }

    public void checkingLiquid(bool checkingLiquid)
    {
    if (checkingLiquid)
    {
    data[x, y]._bTileHeader3 |= 8;
    return;
    }
    data[x, y]._bTileHeader3 &= 247;
    }

    public bool skipLiquid()
    {
    return (data[x, y]._bTileHeader3 & 16) == 16;
    }

    public void skipLiquid(bool skipLiquid)
    {
    if (skipLiquid)
    {
    data[x, y]._bTileHeader3 |= 16;
    return;
    }
    data[x, y]._bTileHeader3 &= 239;
    }

    public bool wire()
    {
    return (data[x, y]._sTileHeader & 128) == 128;
    }

    public void wire(bool wire)
    {
    if (wire)
    {
    data[x, y]._sTileHeader |= 128;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 65407);
    }

    public bool wire2()
    {
    return (data[x, y]._sTileHeader & 256) == 256;
    }

    public void wire2(bool wire2)
    {
    if (wire2)
    {
    data[x, y]._sTileHeader |= 256;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 65279);
    }

    public bool wire3()
    {
    return (data[x, y]._sTileHeader & 512) == 512;
    }

    public void wire3(bool wire3)
    {
    if (wire3)
    {
    data[x, y]._sTileHeader |= 512;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 65023);
    }

    public bool halfBrick()
    {
    return (data[x, y]._sTileHeader & 1024) == 1024;
    }

    public void halfBrick(bool halfBrick)
    {
    if (halfBrick)
    {
    data[x, y]._sTileHeader |= 1024;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 64511);
    }

    public bool actuator()
    {
    return (data[x, y]._sTileHeader & 2048) == 2048;
    }

    public void actuator(bool actuator)
    {
    if (actuator)
    {
    data[x, y]._sTileHeader |= 2048;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 63487);
    }

    public bool nactive()
    {
    int num = (int)(data[x, y]._sTileHeader & 96);
    return num == 32;
    }

    public bool inActive()
    {
    return (data[x, y]._sTileHeader & 64) == 64;
    }

    public void inActive(bool inActive)
    {
    if (inActive)
    {
    data[x, y]._sTileHeader |= 64;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 65471);
    }

    public bool active()
    {
    return (data[x, y]._sTileHeader & 32) == 32;
    }

    public void active(bool active)
    {
    if (active)
    {
    data[x, y]._sTileHeader |= 32;
    return;
    }
    data[x, y]._sTileHeader = (short)((int)data[x, y]._sTileHeader & 65503);
    }

    public void ResetToType(ushort type)
    {
    data[x, y]._liquid = 0;
    data[x, y]._sTileHeader = 32;
    data[x, y]._bTileHeader = 0;
    data[x, y]._bTileHeader2 = 0;
    data[x, y]._bTileHeader3 = 0;
    data[x, y]._frameX = 0;
    data[x, y]._frameY = 0;
    data[x, y]._type = type;
    }

    public Color actColor(Color oldColor)
    {
    if (!this.inActive())
    {
    return oldColor;
    }
    double num = 0.4;
    return new Color((int)((byte)(num * (double)oldColor.R)), (int)((byte)(num * (double)oldColor.G)), (int)((byte)(num * (double)oldColor.B)), (int)oldColor.A);
    }

    public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
    {
    if (applyToNeighbors)
    {
    MemTile.SmoothSlope(x + 1, y, false);
    MemTile.SmoothSlope(x - 1, y, false);
    MemTile.SmoothSlope(x, y + 1, false);
    MemTile.SmoothSlope(x, y - 1, false);
    }
    var tile = Terraria.Main.tile[x, y];
    if (!Terraria.WorldGen.SolidOrSlopedTile(x, y))
    {
    return;
    }
    bool flag = !Terraria.WorldGen.TileEmpty(x, y - 1);
    bool flag2 = !Terraria.WorldGen.SolidOrSlopedTile(x, y - 1) && flag;
    bool flag3 = Terraria.WorldGen.SolidOrSlopedTile(x, y + 1);
    bool flag4 = Terraria.WorldGen.SolidOrSlopedTile(x - 1, y);
    bool flag5 = Terraria.WorldGen.SolidOrSlopedTile(x + 1, y);
    switch ((flag ? 1 : 0) << 3 | (flag3 ? 1 : 0) << 2 | (flag4 ? 1 : 0) << 1 | (flag5 ? 1 : 0))
    {
    case 4:
    tile.slope(0);
    tile.halfBrick(true);
    return;
    case 5:
    tile.halfBrick(false);
    tile.slope(2);
    return;
    case 6:
    tile.halfBrick(false);
    tile.slope(1);
    return;
    case 9:
    if (!flag2)
    {
    tile.halfBrick(false);
    tile.slope(4);
    return;
    }
    return;
    case 10:
    if (!flag2)
    {
    tile.halfBrick(false);
    tile.slope(3);
    return;
    }
    return;
    }
    tile.halfBrick(false);
    tile.slope(0);
    }

    internal void ClearMetadata()
    {
    data[x, y]._liquid = 0;
    data[x, y]._sTileHeader = 0;
    data[x, y]._bTileHeader = 0;
    data[x, y]._bTileHeader2 = 0;
    data[x, y]._bTileHeader3 = 0;
    data[x, y]._frameX = 0;
    data[x, y]._frameY = 0;
    }

    public static bool operator ==(MemTile t1, MemTile t2)
    {
    return t1.x == t2.x
    && t1.y == t2.y
    && t1.liquid == t2.liquid
    && t1.sTileHeader == t2.sTileHeader
    && t1.bTileHeader == t2.bTileHeader
    && t1.bTileHeader2 == t2.bTileHeader2
    && t1.bTileHeader3 == t2.bTileHeader3
    && t1.frameX == t2.frameX
    && t1.frameY == t2.frameY;
    }

    public static bool operator !=(MemTile t1, MemTile t2)
    {
    return t1.x != t2.x
    || t1.y != t2.y
    || t1.liquid != t2.liquid
    || t1.sTileHeader != t2.sTileHeader
    || t1.bTileHeader != t2.bTileHeader
    || t1.bTileHeader2 != t2.bTileHeader2
    || t1.bTileHeader3 != t2.bTileHeader3
    || t1.frameX != t2.frameX
    || t1.frameY != t2.frameY;
    }

    public override bool Equals(object obj)
    {
    if (obj == null)
    {
    if (this.x == 0 && this.y == 0) return true;
    return false;
    }
    else if (obj is MemTile)
    {
    return ((MemTile)obj) == this;
    }
    return false;
    }

    public override int GetHashCode()
    {
    return base.GetHashCode();
    }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TileRef
    {
    internal ushort _type;

    internal byte _wall;

    internal byte _liquid;

    internal short _sTileHeader;

    internal byte _bTileHeader;

    internal byte _bTileHeader2;

    internal byte _bTileHeader3;

    internal short _frameX;

    internal short _frameY;
    }
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct Tile
    //{
    //    public static readonly Tile Empty;

    //    public const int Type_Solid = 0;

    //    public const int Type_Halfbrick = 1;

    //    public const int Type_SlopeDownRight = 2;

    //    public const int Type_SlopeDownLeft = 3;

    //    public const int Type_SlopeUpRight = 4;

    //    public const int Type_SlopeUpLeft = 5;

    //    public const int Liquid_Water = 0;

    //    public const int Liquid_Lava = 1;

    //    public const int Liquid_Honey = 2;

    //    private ushort _type;
    //    public ushort type
    //    {
    //        get
    //        {
    //            return _type;
    //        }
    //        set
    //        {
    //            _type = value;
    //        }
    //    }

    //    private byte _wall;
    //    public byte wall
    //    {
    //        get
    //        {
    //            return _wall;
    //        }
    //        set
    //        {
    //            _wall = value;
    //        }
    //    }

    //    private byte _liquid;
    //    public byte liquid
    //    {
    //        get
    //        {
    //            return _liquid;
    //        }
    //        set
    //        {
    //            _liquid = value;
    //        }
    //    }

    //    private short _sTileHeader;
    //    public short sTileHeader
    //    {
    //        get
    //        {
    //            return _sTileHeader;
    //        }
    //        set
    //        {
    //            _sTileHeader = value;
    //        }
    //    }

    //    private byte _bTileHeader;
    //    public byte bTileHeader
    //    {
    //        get
    //        {
    //            return _bTileHeader;
    //        }
    //        set
    //        {
    //            _bTileHeader = value;
    //        }
    //    }

    //    private byte _bTileHeader2;
    //    public byte bTileHeader2
    //    {
    //        get
    //        {
    //            return _bTileHeader2;
    //        }
    //        set
    //        {
    //            _bTileHeader2 = value;
    //        }
    //    }

    //    private byte _bTileHeader3;
    //    public byte bTileHeader3
    //    {
    //        get
    //        {
    //            return _bTileHeader3;
    //        }
    //        set
    //        {
    //            _bTileHeader3 = value;
    //        }
    //    }

    //    private short _frameX;
    //    public short frameX
    //    {
    //        get
    //        {
    //            return _frameX;
    //        }
    //        set
    //        {
    //            _frameX = value;
    //        }
    //    }

    //    private short _frameY;
    //    public short frameY
    //    {
    //        get
    //        {
    //            return _frameY;
    //        }
    //        set
    //        {
    //            _frameY = value;
    //        }
    //    }

    //    public int collisionType
    //    {
    //        get
    //        {
    //            if (!this.active())
    //            {
    //                return 0;
    //            }
    //            if (this.halfBrick())
    //            {
    //                return 2;
    //            }
    //            if (this.slope() > 0)
    //            {
    //                return (int)(2 + this.slope());
    //            }
    //            if (Main.tileSolid[(int)this._type] && !Main.tileSolidTop[(int)this._type])
    //            {
    //                return 1;
    //            }
    //            return -1;
    //        }
    //    }

    //    //public Tile()
    //    //{
    //    //    this._type = 0;
    //    //    this._wall = 0;
    //    //    this._liquid = 0;
    //    //    this._sTileHeader = 0;
    //    //    this._bTileHeader = 0;
    //    //    this._bTileHeader2 = 0;
    //    //    this._bTileHeader3 = 0;
    //    //    this._frameX = 0;
    //    //    this._frameY = 0;
    //    //}

    //    public Tile(Tile copy)
    //    {
    //        //if (copy == null)
    //        //{
    //        //    this._type = 0;
    //        //    this._wall = 0;
    //        //    this._liquid = 0;
    //        //    this._sTileHeader = 0;
    //        //    this._bTileHeader = 0;
    //        //    this._bTileHeader2 = 0;
    //        //    this._bTileHeader3 = 0;
    //        //    this._frameX = 0;
    //        //    this._frameY = 0;
    //        //    return;
    //        //}
    //        this._type = copy._type;
    //        this._wall = copy._wall;
    //        this._liquid = copy._liquid;
    //        this._sTileHeader = copy._sTileHeader;
    //        this._bTileHeader = copy._bTileHeader;
    //        this._bTileHeader2 = copy._bTileHeader2;
    //        this._bTileHeader3 = copy._bTileHeader3;
    //        this._frameX = copy._frameX;
    //        this._frameY = copy._frameY;
    //    }

    //    public object Clone()
    //    {
    //        return base.MemberwiseClone();
    //    }

    //    public void ClearEverything()
    //    {
    //        this._type = 0;
    //        this._wall = 0;
    //        this._liquid = 0;
    //        this._sTileHeader = 0;
    //        this._bTileHeader = 0;
    //        this._bTileHeader2 = 0;
    //        this._bTileHeader3 = 0;
    //        this._frameX = 0;
    //        this._frameY = 0;
    //    }

    //    public void ClearTile()
    //    {
    //        this.slope(0);
    //        this.halfBrick(false);
    //        this.active(false);
    //    }

    //    public void CopyFrom(Tile from)
    //    {
    //        this._type = from._type;
    //        this._wall = from._wall;
    //        this._liquid = from._liquid;
    //        this._sTileHeader = from._sTileHeader;
    //        this._bTileHeader = from._bTileHeader;
    //        this._bTileHeader2 = from._bTileHeader2;
    //        this._bTileHeader3 = from._bTileHeader3;
    //        this._frameX = from._frameX;
    //        this._frameY = from._frameY;
    //    }

    //    public bool isTheSameAs(Tile compTile)
    //    {
    //        //if (compTile == null)
    //        //{
    //        //    return false;
    //        //}
    //        if (this._sTileHeader != compTile._sTileHeader)
    //        {
    //            return false;
    //        }
    //        if (this.active())
    //        {
    //            if (this._type != compTile._type)
    //            {
    //                return false;
    //            }
    //            if (Main.tileFrameImportant[(int)this._type] && (this._frameX != compTile._frameX || this._frameY != compTile._frameY))
    //            {
    //                return false;
    //            }
    //        }
    //        if (this._wall != compTile._wall || this._liquid != compTile._liquid)
    //        {
    //            return false;
    //        }
    //        if (compTile._liquid == 0)
    //        {
    //            if (this.wallColor() != compTile.wallColor())
    //            {
    //                return false;
    //            }
    //        }
    //        else if (this._bTileHeader != compTile._bTileHeader)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    public int wallFrameX()
    //    {
    //        return (int)((this._bTileHeader2 & 15) * 36);
    //    }

    //    public void wallFrameX(int wallFrameX)
    //    {
    //        this._bTileHeader2 = (byte)((int)(this._bTileHeader2 & 240) | (wallFrameX / 36 & 15));
    //    }

    //    public int wallFrameY()
    //    {
    //        return (int)((this._bTileHeader3 & 7) * 36);
    //    }

    //    public void wallFrameY(int wallFrameY)
    //    {
    //        this._bTileHeader3 = (byte)((int)(this._bTileHeader3 & 248) | (wallFrameY / 36 & 7));
    //    }

    //    public byte frameNumber()
    //    {
    //        return (byte)((this._bTileHeader2 & 48) >> 4);
    //    }

    //    public void frameNumber(byte frameNumber)
    //    {
    //        this._bTileHeader2 = (byte)((int)(this._bTileHeader2 & 207) | (int)(frameNumber & 3) << 4);
    //    }

    //    public byte wallFrameNumber()
    //    {
    //        return (byte)((this._bTileHeader2 & 192) >> 6);
    //    }

    //    public void wallFrameNumber(byte wallFrameNumber)
    //    {
    //        this._bTileHeader2 = (byte)((int)(this._bTileHeader2 & 63) | (int)(wallFrameNumber & 3) << 6);
    //    }

    //    public bool topSlope()
    //    {
    //        byte b = this.slope();
    //        return b == 1 || b == 2;
    //    }

    //    public bool bottomSlope()
    //    {
    //        byte b = this.slope();
    //        return b == 3 || b == 4;
    //    }

    //    public bool leftSlope()
    //    {
    //        byte b = this.slope();
    //        return b == 2 || b == 4;
    //    }

    //    public bool rightSlope()
    //    {
    //        byte b = this.slope();
    //        return b == 1 || b == 3;
    //    }

    //    public byte slope()
    //    {
    //        return (byte)((this._sTileHeader & 28672) >> 12);
    //    }

    //    public bool HasSameSlope(Tile tile)
    //    {
    //        return (this._sTileHeader & 29696) == (tile._sTileHeader & 29696);
    //    }

    //    public void slope(byte slope)
    //    {
    //        this._sTileHeader = (short)(((int)this._sTileHeader & 36863) | (int)(slope & 7) << 12);
    //    }

    //    public int blockType()
    //    {
    //        if (this.halfBrick())
    //        {
    //            return 1;
    //        }
    //        int num = (int)this.slope();
    //        if (num > 0)
    //        {
    //            num++;
    //        }
    //        return num;
    //    }

    //    public byte color()
    //    {
    //        return (byte)(this._sTileHeader & 31);
    //    }

    //    public void color(byte color)
    //    {
    //        if (color > 30)
    //        {
    //            color = 30;
    //        }
    //        this._sTileHeader = (short)(((int)this._sTileHeader & 65504) | (int)color);
    //    }

    //    public byte wallColor()
    //    {
    //        return (byte)(this._bTileHeader & 31);
    //    }

    //    public void wallColor(byte wallColor)
    //    {
    //        if (wallColor > 30)
    //        {
    //            wallColor = 30;
    //        }
    //        this._bTileHeader = (byte)(((this._bTileHeader & 224) | wallColor));
    //    }

    //    public bool lava()
    //    {
    //        return (this._bTileHeader & 32) == 32;
    //    }

    //    public void lava(bool lava)
    //    {
    //        if (lava)
    //        {
    //            this._bTileHeader = (byte)(((this._bTileHeader & 159) | 32));
    //            return;
    //        }
    //        this._bTileHeader &= 223;
    //    }

    //    public bool honey()
    //    {
    //        return (this._bTileHeader & 64) == 64;
    //    }

    //    public void honey(bool honey)
    //    {
    //        if (honey)
    //        {
    //            this._bTileHeader = (byte)(((this._bTileHeader & 159) | 64));
    //            return;
    //        }
    //        this._bTileHeader &= 191;
    //    }

    //    public void liquidType(int liquidType)
    //    {
    //        if (liquidType == 0)
    //        {
    //            this._bTileHeader &= 159;
    //            return;
    //        }
    //        if (liquidType == 1)
    //        {
    //            this.lava(true);
    //            return;
    //        }
    //        if (liquidType == 2)
    //        {
    //            this.honey(true);
    //        }
    //    }

    //    public byte liquidType()
    //    {
    //        return (byte)((this._bTileHeader & 96) >> 5);
    //    }

    //    public bool checkingLiquid()
    //    {
    //        return (this._bTileHeader3 & 8) == 8;
    //    }

    //    public void checkingLiquid(bool checkingLiquid)
    //    {
    //        if (checkingLiquid)
    //        {
    //            this._bTileHeader3 |= 8;
    //            return;
    //        }
    //        this._bTileHeader3 &= 247;
    //    }

    //    public bool skipLiquid()
    //    {
    //        return (this._bTileHeader3 & 16) == 16;
    //    }

    //    public void skipLiquid(bool skipLiquid)
    //    {
    //        if (skipLiquid)
    //        {
    //            this._bTileHeader3 |= 16;
    //            return;
    //        }
    //        this._bTileHeader3 &= 239;
    //    }

    //    public bool wire()
    //    {
    //        return (this._sTileHeader & 128) == 128;
    //    }

    //    public void wire(bool wire)
    //    {
    //        if (wire)
    //        {
    //            this._sTileHeader |= 128;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 65407);
    //    }

    //    public bool wire2()
    //    {
    //        return (this._sTileHeader & 256) == 256;
    //    }

    //    public void wire2(bool wire2)
    //    {
    //        if (wire2)
    //        {
    //            this._sTileHeader |= 256;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 65279);
    //    }

    //    public bool wire3()
    //    {
    //        return (this._sTileHeader & 512) == 512;
    //    }

    //    public void wire3(bool wire3)
    //    {
    //        if (wire3)
    //        {
    //            this._sTileHeader |= 512;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 65023);
    //    }

    //    public bool halfBrick()
    //    {
    //        return (this._sTileHeader & 1024) == 1024;
    //    }

    //    public void halfBrick(bool halfBrick)
    //    {
    //        if (halfBrick)
    //        {
    //            this._sTileHeader |= 1024;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 64511);
    //    }

    //    public bool actuator()
    //    {
    //        return (this._sTileHeader & 2048) == 2048;
    //    }

    //    public void actuator(bool actuator)
    //    {
    //        if (actuator)
    //        {
    //            this._sTileHeader |= 2048;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 63487);
    //    }

    //    public bool nactive()
    //    {
    //        int num = (int)(this._sTileHeader & 96);
    //        return num == 32;
    //    }

    //    public bool inActive()
    //    {
    //        return (this._sTileHeader & 64) == 64;
    //    }

    //    public void inActive(bool inActive)
    //    {
    //        if (inActive)
    //        {
    //            this._sTileHeader |= 64;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 65471);
    //    }

    //    public bool active()
    //    {
    //        return (this._sTileHeader & 32) == 32;
    //    }

    //    public void active(bool active)
    //    {
    //        if (active)
    //        {
    //            this._sTileHeader |= 32;
    //            return;
    //        }
    //        this._sTileHeader = (short)((int)this._sTileHeader & 65503);
    //    }

    //    public void ResetToType(ushort _type)
    //    {
    //        this._liquid = 0;
    //        this._sTileHeader = 32;
    //        this._bTileHeader = 0;
    //        this._bTileHeader2 = 0;
    //        this._bTileHeader3 = 0;
    //        this._frameX = 0;
    //        this._frameY = 0;
    //        this._type = _type;
    //    }

    //    public Color actColor(Color oldColor)
    //    {
    //        if (!this.inActive())
    //        {
    //            return oldColor;
    //        }
    //        double num = 0.4;
    //        return new Color((int)((byte)(num * (double)oldColor.R)), (int)((byte)(num * (double)oldColor.G)), (int)((byte)(num * (double)oldColor.B)), (int)oldColor.A);
    //    }

    //    public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
    //    {
    //        if (applyToNeighbors)
    //        {
    //            Tile.SmoothSlope(x + 1, y, false);
    //            Tile.SmoothSlope(x - 1, y, false);
    //            Tile.SmoothSlope(x, y + 1, false);
    //            Tile.SmoothSlope(x, y - 1, false);
    //        }
    //        Tile tile = Main.tile[x, y];
    //        if (!WorldGen.SolidOrSlopedTile(x, y))
    //        {
    //            return;
    //        }
    //        bool flag = !WorldGen.TileEmpty(x, y - 1);
    //        bool flag2 = !WorldGen.SolidOrSlopedTile(x, y - 1) && flag;
    //        bool flag3 = WorldGen.SolidOrSlopedTile(x, y + 1);
    //        bool flag4 = WorldGen.SolidOrSlopedTile(x - 1, y);
    //        bool flag5 = WorldGen.SolidOrSlopedTile(x + 1, y);
    //        switch ((flag ? 1 : 0) << 3 | (flag3 ? 1 : 0) << 2 | (flag4 ? 1 : 0) << 1 | (flag5 ? 1 : 0))
    //        {
    //            case 4:
    //                tile.slope(0);
    //                tile.halfBrick(true);
    //                return;
    //            case 5:
    //                tile.halfBrick(false);
    //                tile.slope(2);
    //                return;
    //            case 6:
    //                tile.halfBrick(false);
    //                tile.slope(1);
    //                return;
    //            case 9:
    //                if (!flag2)
    //                {
    //                    tile.halfBrick(false);
    //                    tile.slope(4);
    //                    return;
    //                }
    //                return;
    //            case 10:
    //                if (!flag2)
    //                {
    //                    tile.halfBrick(false);
    //                    tile.slope(3);
    //                    return;
    //                }
    //                return;
    //        }
    //        tile.halfBrick(false);
    //        tile.slope(0);
    //    }

    //    internal void ClearMetadata()
    //    {
    //        this._liquid = 0;
    //        this._sTileHeader = 0;
    //        this._bTileHeader = 0;
    //        this._bTileHeader2 = 0;
    //        this._bTileHeader3 = 0;
    //        this._frameX = 0;
    //        this._frameY = 0;
    //    }

    //    public static bool operator ==(Tile t1, Tile t2)
    //    {
    //        return false;
    //    }

    //    public static bool operator !=(Tile t1, Tile t2)
    //    {
    //        return false;
    //    }
    //}
    
#elif VANILLACOMPAT
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public class VanillaTile
    {
        public const int Type_Solid = 0;

        public const int Type_Halfbrick = 1;

        public const int Type_SlopeDownRight = 2;

        public const int Type_SlopeDownLeft = 3;

        public const int Type_SlopeUpRight = 4;

        public const int Type_SlopeUpLeft = 5;

        public const int Liquid_Water = 0;

        public const int Liquid_Lava = 1;

        public const int Liquid_Honey = 2;


        public byte wall;

        public byte liquid;

        public byte bTileHeader;

        public byte bTileHeader2;

        public byte bTileHeader3;

        public short sTileHeader;

        public ushort type;

        public short frameX;

        public short frameY;

        public int collisionType
        {
            get
            {
                if (!this.active())
                {
                    return 0;
                }
                if (this.halfBrick())
                {
                    return 2;
                }
                if (this.slope() > 0)
                {
                    return (int)(2 + this.slope());
                }
                if (Terraria.Main.tileSolid[(int)this.type] && !Terraria.Main.tileSolidTop[(int)this.type])
                {
                    return 1;
                }
                return -1;
            }
        }

        public VanillaTile()
        {
            this.type = 0;
            this.wall = 0;
            this.liquid = 0;
            this.sTileHeader = 0;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
        }

        public VanillaTile(VanillaTile copy)
        {
            if (copy == null)
            {
                this.type = 0;
                this.wall = 0;
                this.liquid = 0;
                this.sTileHeader = 0;
                this.bTileHeader = 0;
                this.bTileHeader2 = 0;
                this.bTileHeader3 = 0;
                this.frameX = 0;
                this.frameY = 0;
                return;
            }
            this.type = copy.type;
            this.wall = copy.wall;
            this.liquid = copy.liquid;
            this.sTileHeader = copy.sTileHeader;
            this.bTileHeader = copy.bTileHeader;
            this.bTileHeader2 = copy.bTileHeader2;
            this.bTileHeader3 = copy.bTileHeader3;
            this.frameX = copy.frameX;
            this.frameY = copy.frameY;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public void ClearEverything()
        {
            this.type = 0;
            this.wall = 0;
            this.liquid = 0;
            this.sTileHeader = 0;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
        }

        public void ClearTile()
        {
            this.slope(0);
            this.halfBrick(false);
            this.active(false);
        }

        public void CopyFrom(VanillaTile from)
        {
            this.type = from.type;
            this.wall = from.wall;
            this.liquid = from.liquid;
            this.sTileHeader = from.sTileHeader;
            this.bTileHeader = from.bTileHeader;
            this.bTileHeader2 = from.bTileHeader2;
            this.bTileHeader3 = from.bTileHeader3;
            this.frameX = from.frameX;
            this.frameY = from.frameY;
        }

        public bool isTheSameAs(VanillaTile compTile)
        {
            if (compTile == null)
            {
                return false;
            }
            if (this.sTileHeader != compTile.sTileHeader)
            {
                return false;
            }
            if (this.active())
            {
                if (this.type != compTile.type)
                {
                    return false;
                }
                if (Terraria.Main.tileFrameImportant[(int)this.type] && (this.frameX != compTile.frameX || this.frameY != compTile.frameY))
                {
                    return false;
                }
            }
            if (this.wall != compTile.wall || this.liquid != compTile.liquid)
            {
                return false;
            }
            if (compTile.liquid == 0)
            {
                if (this.wallColor() != compTile.wallColor())
                {
                    return false;
                }
            }
            else if (this.bTileHeader != compTile.bTileHeader)
            {
                return false;
            }
            return true;
        }

        public int wallFrameX()
        {
            return (int)((this.bTileHeader2 & 15) * 36);
        }

        public void wallFrameX(int wallFrameX)
        {
            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 240) | (wallFrameX / 36 & 15));
        }

        public int wallFrameY()
        {
            return (int)((this.bTileHeader3 & 7) * 36);
        }

        public void wallFrameY(int wallFrameY)
        {
            this.bTileHeader3 = (byte)((int)(this.bTileHeader3 & 248) | (wallFrameY / 36 & 7));
        }

        public byte frameNumber()
        {
            return (byte)((this.bTileHeader2 & 48) >> 4);
        }

        public void frameNumber(byte frameNumber)
        {
            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 207) | (int)(frameNumber & 3) << 4);
        }

        public byte wallFrameNumber()
        {
            return (byte)((this.bTileHeader2 & 192) >> 6);
        }

        public void wallFrameNumber(byte wallFrameNumber)
        {
            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 63) | (int)(wallFrameNumber & 3) << 6);
        }

        public bool topSlope()
        {
            byte b = this.slope();
            return b == 1 || b == 2;
        }

        public bool bottomSlope()
        {
            byte b = this.slope();
            return b == 3 || b == 4;
        }

        public bool leftSlope()
        {
            byte b = this.slope();
            return b == 2 || b == 4;
        }

        public bool rightSlope()
        {
            byte b = this.slope();
            return b == 1 || b == 3;
        }

        public byte slope()
        {
            return (byte)((this.sTileHeader & 28672) >> 12);
        }

        public bool HasSameSlope(VanillaTile tile)
        {
            return (this.sTileHeader & 29696) == (tile.sTileHeader & 29696);
        }

        public void slope(byte slope)
        {
            this.sTileHeader = (short)(((int)this.sTileHeader & 36863) | (int)(slope & 7) << 12);
        }

        public int blockType()
        {
            if (this.halfBrick())
            {
                return 1;
            }
            int num = (int)this.slope();
            if (num > 0)
            {
                num++;
            }
            return num;
        }

        public byte color()
        {
            return (byte)(this.sTileHeader & 31);
        }

        public void color(byte color)
        {
            if (color > 30)
            {
                color = 30;
            }
            this.sTileHeader = (short)(((int)this.sTileHeader & 65504) | (int)color);
        }

        public byte wallColor()
        {
            return (byte)(this.bTileHeader & 31);
        }

        public void wallColor(byte wallColor)
        {
            if (wallColor > 30)
            {
                wallColor = 30;
            }
            this.bTileHeader = (byte)((this.bTileHeader & 224) | wallColor);
        }

        public bool lava()
        {
            return (this.bTileHeader & 32) == 32;
        }

        public void lava(bool lava)
        {
            if (lava)
            {
                this.bTileHeader = (byte)((this.bTileHeader & 159) | 32);
                return;
            }
            this.bTileHeader &= 223;
        }

        public bool honey()
        {
            return (this.bTileHeader & 64) == 64;
        }

        public void honey(bool honey)
        {
            if (honey)
            {
                this.bTileHeader = (byte)((this.bTileHeader & 159) | 64);
                return;
            }
            this.bTileHeader &= 191;
        }

        public void liquidType(int liquidType)
        {
            if (liquidType == 0)
            {
                this.bTileHeader &= 159;
                return;
            }
            if (liquidType == 1)
            {
                this.lava(true);
                return;
            }
            if (liquidType == 2)
            {
                this.honey(true);
            }
        }

        public byte liquidType()
        {
            return (byte)((this.bTileHeader & 96) >> 5);
        }

        public bool checkingLiquid()
        {
            return (this.bTileHeader3 & 8) == 8;
        }

        public void checkingLiquid(bool checkingLiquid)
        {
            if (checkingLiquid)
            {
                this.bTileHeader3 |= 8;
                return;
            }
            this.bTileHeader3 &= 247;
        }

        public bool skipLiquid()
        {
            return (this.bTileHeader3 & 16) == 16;
        }

        public void skipLiquid(bool skipLiquid)
        {
            if (skipLiquid)
            {
                this.bTileHeader3 |= 16;
                return;
            }
            this.bTileHeader3 &= 239;
        }

        public bool wire()
        {
            return (this.sTileHeader & 128) == 128;
        }

        public void wire(bool wire)
        {
            if (wire)
            {
                this.sTileHeader |= 128;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65407);
        }

        public bool wire2()
        {
            return (this.sTileHeader & 256) == 256;
        }

        public void wire2(bool wire2)
        {
            if (wire2)
            {
                this.sTileHeader |= 256;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65279);
        }

        public bool wire3()
        {
            return (this.sTileHeader & 512) == 512;
        }

        public void wire3(bool wire3)
        {
            if (wire3)
            {
                this.sTileHeader |= 512;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65023);
        }

        public bool halfBrick()
        {
            return (this.sTileHeader & 1024) == 1024;
        }

        public void halfBrick(bool halfBrick)
        {
            if (halfBrick)
            {
                this.sTileHeader |= 1024;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 64511);
        }

        public bool actuator()
        {
            return (this.sTileHeader & 2048) == 2048;
        }

        public void actuator(bool actuator)
        {
            if (actuator)
            {
                this.sTileHeader |= 2048;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 63487);
        }

        public bool nactive()
        {
            int num = (int)(this.sTileHeader & 96);
            return num == 32;
        }

        public bool inActive()
        {
            return (this.sTileHeader & 64) == 64;
        }

        public void inActive(bool inActive)
        {
            if (inActive)
            {
                this.sTileHeader |= 64;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65471);
        }

        public bool active()
        {
            return (this.sTileHeader & 32) == 32;
        }

        public void active(bool active)
        {
            if (active)
            {
                this.sTileHeader |= 32;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65503);
        }

        public void ResetToType(ushort type)
        {
            this.liquid = 0;
            this.sTileHeader = 32;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
            this.type = type;
        }

        public Color actColor(Color oldColor)
        {
            if (!this.inActive())
            {
                return oldColor;
            }
            double num = 0.4;
            return new Color((int)((byte)(num * (double)oldColor.R)), (int)((byte)(num * (double)oldColor.G)), (int)((byte)(num * (double)oldColor.B)), (int)oldColor.A);
        }

        public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
        {
            if (applyToNeighbors)
            {
                VanillaTile.SmoothSlope(x + 1, y, false);
                VanillaTile.SmoothSlope(x - 1, y, false);
                VanillaTile.SmoothSlope(x, y + 1, false);
                VanillaTile.SmoothSlope(x, y - 1, false);
            }

            var tile = Terraria.Main.tile[x, y];
            if (!Terraria.WorldGen.SolidOrSlopedTile(x, y))
            {
                return;
            }
            bool flag = !Terraria.WorldGen.TileEmpty(x, y - 1);
            bool flag2 = !Terraria.WorldGen.SolidOrSlopedTile(x, y - 1) && flag;
            bool flag3 = Terraria.WorldGen.SolidOrSlopedTile(x, y + 1);
            bool flag4 = Terraria.WorldGen.SolidOrSlopedTile(x - 1, y);
            bool flag5 = Terraria.WorldGen.SolidOrSlopedTile(x + 1, y);
            switch ((flag ? 1 : 0) << 3 | (flag3 ? 1 : 0) << 2 | (flag4 ? 1 : 0) << 1 | (flag5 ? 1 : 0))
            {
                case 4:
                    tile.slope(0);
                    tile.halfBrick(true);
                    return;
                case 5:
                    tile.halfBrick(false);
                    tile.slope(2);
                    return;
                case 6:
                    tile.halfBrick(false);
                    tile.slope(1);
                    return;
                case 9:
                    if (!flag2)
                    {
                        tile.halfBrick(false);
                        tile.slope(4);
                        return;
                    }
                    return;
                case 10:
                    if (!flag2)
                    {
                        tile.halfBrick(false);
                        tile.slope(3);
                        return;
                    }
                    return;
            }
            tile.halfBrick(false);
            tile.slope(0);
        }

        public void ClearMetadata()
        {
            this.liquid = 0;
            this.sTileHeader = 0;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
        }
    }
    #else
    public class VanillaTile
    {
    public const int Type_Solid = 0;

    public const int Type_Halfbrick = 1;

    public const int Type_SlopeDownRight = 2;

    public const int Type_SlopeDownLeft = 3;

    public const int Type_SlopeUpRight = 4;

    public const int Type_SlopeUpLeft = 5;

    public const int Liquid_Water = 0;

    public const int Liquid_Lava = 1;

    public const int Liquid_Honey = 2;

    public ushort type;

    public byte wall;

    public byte liquid;

    public short sTileHeader;

    public byte bTileHeader;

    public byte bTileHeader2;

    public byte bTileHeader3;

    public short frameX;

    public short frameY;

    public int collisionType
    {
    get
    {
    if (!this.active())
    {
    return 0;
    }
    if (this.halfBrick())
    {
    return 2;
    }
    if (this.slope() > 0)
    {
    return (int)(2 + this.slope());
    }
    if (Terraria.Main.tileSolid[(int)this.type] && !Terraria.Main.tileSolidTop[(int)this.type])
    {
    return 1;
    }
    return -1;
    }
    }

    public VanillaTile()
    {
    this.type = 0;
    this.wall = 0;
    this.liquid = 0;
    this.sTileHeader = 0;
    this.bTileHeader = 0;
    this.bTileHeader2 = 0;
    this.bTileHeader3 = 0;
    this.frameX = 0;
    this.frameY = 0;
    }

    public VanillaTile(VanillaTile copy)
    {
    if (copy == null)
    {
    this.type = 0;
    this.wall = 0;
    this.liquid = 0;
    this.sTileHeader = 0;
    this.bTileHeader = 0;
    this.bTileHeader2 = 0;
    this.bTileHeader3 = 0;
    this.frameX = 0;
    this.frameY = 0;
    return;
    }
    this.type = copy.type;
    this.wall = copy.wall;
    this.liquid = copy.liquid;
    this.sTileHeader = copy.sTileHeader;
    this.bTileHeader = copy.bTileHeader;
    this.bTileHeader2 = copy.bTileHeader2;
    this.bTileHeader3 = copy.bTileHeader3;
    this.frameX = copy.frameX;
    this.frameY = copy.frameY;
    }

    public object Clone()
    {
    return base.MemberwiseClone();
    }

    public void ClearEverything()
    {
    this.type = 0;
    this.wall = 0;
    this.liquid = 0;
    this.sTileHeader = 0;
    this.bTileHeader = 0;
    this.bTileHeader2 = 0;
    this.bTileHeader3 = 0;
    this.frameX = 0;
    this.frameY = 0;
    }

    public void ClearTile()
    {
    this.slope(0);
    this.halfBrick(false);
    this.active(false);
    }

    public void CopyFrom(VanillaTile from)
    {
    this.type = from.type;
    this.wall = from.wall;
    this.liquid = from.liquid;
    this.sTileHeader = from.sTileHeader;
    this.bTileHeader = from.bTileHeader;
    this.bTileHeader2 = from.bTileHeader2;
    this.bTileHeader3 = from.bTileHeader3;
    this.frameX = from.frameX;
    this.frameY = from.frameY;
    }

    public bool isTheSameAs(VanillaTile compTile)
    {
    if (compTile == null)
    {
    return false;
    }
    if (this.sTileHeader != compTile.sTileHeader)
    {
    return false;
    }
    if (this.active())
    {
    if (this.type != compTile.type)
    {
    return false;
    }
    if (Terraria.Main.tileFrameImportant[(int)this.type] && (this.frameX != compTile.frameX || this.frameY != compTile.frameY))
    {
    return false;
    }
    }
    if (this.wall != compTile.wall || this.liquid != compTile.liquid)
    {
    return false;
    }
    if (compTile.liquid == 0)
    {
    if (this.wallColor() != compTile.wallColor())
    {
    return false;
    }
    }
    else if (this.bTileHeader != compTile.bTileHeader)
    {
    return false;
    }
    return true;
    }

    public int wallFrameX()
    {
    return (int)((this.bTileHeader2 & 15) * 36);
    }

    public void wallFrameX(int wallFrameX)
    {
    this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 240) | (wallFrameX / 36 & 15));
    }

    public int wallFrameY()
    {
    return (int)((this.bTileHeader3 & 7) * 36);
    }

    public void wallFrameY(int wallFrameY)
    {
    this.bTileHeader3 = (byte)((int)(this.bTileHeader3 & 248) | (wallFrameY / 36 & 7));
    }

    public byte frameNumber()
    {
    return (byte)((this.bTileHeader2 & 48) >> 4);
    }

    public void frameNumber(byte frameNumber)
    {
    this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 207) | (int)(frameNumber & 3) << 4);
    }

    public byte wallFrameNumber()
    {
    return (byte)((this.bTileHeader2 & 192) >> 6);
    }

    public void wallFrameNumber(byte wallFrameNumber)
    {
    this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 63) | (int)(wallFrameNumber & 3) << 6);
    }

    public bool topSlope()
    {
    byte b = this.slope();
    return b == 1 || b == 2;
    }

    public bool bottomSlope()
    {
    byte b = this.slope();
    return b == 3 || b == 4;
    }

    public bool leftSlope()
    {
    byte b = this.slope();
    return b == 2 || b == 4;
    }

    public bool rightSlope()
    {
    byte b = this.slope();
    return b == 1 || b == 3;
    }

    public byte slope()
    {
    return (byte)((this.sTileHeader & 28672) >> 12);
    }

    public bool HasSameSlope(VanillaTile tile)
    {
    return (this.sTileHeader & 29696) == (tile.sTileHeader & 29696);
    }

    public void slope(byte slope)
    {
    this.sTileHeader = (short)(((int)this.sTileHeader & 36863) | (int)(slope & 7) << 12);
    }

    public int blockType()
    {
    if (this.halfBrick())
    {
    return 1;
    }
    int num = (int)this.slope();
    if (num > 0)
    {
    num++;
    }
    return num;
    }

    public byte color()
    {
    return (byte)(this.sTileHeader & 31);
    }

    public void color(byte color)
    {
    if (color > 30)
    {
    color = 30;
    }
    this.sTileHeader = (short)(((int)this.sTileHeader & 65504) | (int)color);
    }

    public byte wallColor()
    {
    return (byte)(this.bTileHeader & 31);
    }

    public void wallColor(byte wallColor)
    {
    if (wallColor > 30)
    {
    wallColor = 30;
    }
    this.bTileHeader = (byte)((this.bTileHeader & 224) | wallColor);
    }

    public bool lava()
    {
    return (this.bTileHeader & 32) == 32;
    }

    public void lava(bool lava)
    {
    if (lava)
    {
    this.bTileHeader = (byte)((this.bTileHeader & 159) | 32);
    return;
    }
    this.bTileHeader &= 223;
    }

    public bool honey()
    {
    return (this.bTileHeader & 64) == 64;
    }

    public void honey(bool honey)
    {
    if (honey)
    {
    this.bTileHeader = (byte)((this.bTileHeader & 159) | 64);
    return;
    }
    this.bTileHeader &= 191;
    }

    public void liquidType(int liquidType)
    {
    if (liquidType == 0)
    {
    this.bTileHeader &= 159;
    return;
    }
    if (liquidType == 1)
    {
    this.lava(true);
    return;
    }
    if (liquidType == 2)
    {
    this.honey(true);
    }
    }

    public byte liquidType()
    {
    return (byte)((this.bTileHeader & 96) >> 5);
    }

    public bool checkingLiquid()
    {
    return (this.bTileHeader3 & 8) == 8;
    }

    public void checkingLiquid(bool checkingLiquid)
    {
    if (checkingLiquid)
    {
    this.bTileHeader3 |= 8;
    return;
    }
    this.bTileHeader3 &= 247;
    }

    public bool skipLiquid()
    {
    return (this.bTileHeader3 & 16) == 16;
    }

    public void skipLiquid(bool skipLiquid)
    {
    if (skipLiquid)
    {
    this.bTileHeader3 |= 16;
    return;
    }
    this.bTileHeader3 &= 239;
    }

    public bool wire()
    {
    return (this.sTileHeader & 128) == 128;
    }

    public void wire(bool wire)
    {
    if (wire)
    {
    this.sTileHeader |= 128;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 65407);
    }

    public bool wire2()
    {
    return (this.sTileHeader & 256) == 256;
    }

    public void wire2(bool wire2)
    {
    if (wire2)
    {
    this.sTileHeader |= 256;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 65279);
    }

    public bool wire3()
    {
    return (this.sTileHeader & 512) == 512;
    }

    public void wire3(bool wire3)
    {
    if (wire3)
    {
    this.sTileHeader |= 512;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 65023);
    }

    public bool halfBrick()
    {
    return (this.sTileHeader & 1024) == 1024;
    }

    public void halfBrick(bool halfBrick)
    {
    if (halfBrick)
    {
    this.sTileHeader |= 1024;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 64511);
    }

    public bool actuator()
    {
    return (this.sTileHeader & 2048) == 2048;
    }

    public void actuator(bool actuator)
    {
    if (actuator)
    {
    this.sTileHeader |= 2048;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 63487);
    }

    public bool nactive()
    {
    int num = (int)(this.sTileHeader & 96);
    return num == 32;
    }

    public bool inActive()
    {
    return (this.sTileHeader & 64) == 64;
    }

    public void inActive(bool inActive)
    {
    if (inActive)
    {
    this.sTileHeader |= 64;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 65471);
    }

    public bool active()
    {
    return (this.sTileHeader & 32) == 32;
    }

    public void active(bool active)
    {
    if (active)
    {
    this.sTileHeader |= 32;
    return;
    }
    this.sTileHeader = (short)((int)this.sTileHeader & 65503);
    }

    public void ResetToType(ushort type)
    {
    this.liquid = 0;
    this.sTileHeader = 32;
    this.bTileHeader = 0;
    this.bTileHeader2 = 0;
    this.bTileHeader3 = 0;
    this.frameX = 0;
    this.frameY = 0;
    this.type = type;
    }

    public Color actColor(Color oldColor)
    {
    if (!this.inActive())
    {
    return oldColor;
    }
    double num = 0.4;
    return new Color((int)((byte)(num * (double)oldColor.R)), (int)((byte)(num * (double)oldColor.G)), (int)((byte)(num * (double)oldColor.B)), (int)oldColor.A);
    }

    public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
    {
    if (applyToNeighbors)
    {
    VanillaTile.SmoothSlope(x + 1, y, false);
    VanillaTile.SmoothSlope(x - 1, y, false);
    VanillaTile.SmoothSlope(x, y + 1, false);
    VanillaTile.SmoothSlope(x, y - 1, false);
    }

    var tile = Terraria.Main.tile[x, y];
    if (!Terraria.WorldGen.SolidOrSlopedTile(x, y))
    {
    return;
    }
    bool flag = !Terraria.WorldGen.TileEmpty(x, y - 1);
    bool flag2 = !Terraria.WorldGen.SolidOrSlopedTile(x, y - 1) && flag;
    bool flag3 = Terraria.WorldGen.SolidOrSlopedTile(x, y + 1);
    bool flag4 = Terraria.WorldGen.SolidOrSlopedTile(x - 1, y);
    bool flag5 = Terraria.WorldGen.SolidOrSlopedTile(x + 1, y);
    switch ((flag ? 1 : 0) << 3 | (flag3 ? 1 : 0) << 2 | (flag4 ? 1 : 0) << 1 | (flag5 ? 1 : 0))
    {
    case 4:
    tile.slope(0);
    tile.halfBrick(true);
    return;
    case 5:
    tile.halfBrick(false);
    tile.slope(2);
    return;
    case 6:
    tile.halfBrick(false);
    tile.slope(1);
    return;
    case 9:
    if (!flag2)
    {
    tile.halfBrick(false);
    tile.slope(4);
    return;
    }
    return;
    case 10:
    if (!flag2)
    {
    tile.halfBrick(false);
    tile.slope(3);
    return;
    }
    return;
    }
    tile.halfBrick(false);
    tile.slope(0);
    }

    internal void ClearMetadata()
    {
    this.liquid = 0;
    this.sTileHeader = 0;
    this.bTileHeader = 0;
    this.bTileHeader2 = 0;
    this.bTileHeader3 = 0;
    this.frameX = 0;
    this.frameY = 0;
    }
    }
    #endif
}
