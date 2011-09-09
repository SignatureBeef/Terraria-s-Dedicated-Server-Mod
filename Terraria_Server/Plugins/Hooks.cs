using System;

using Terraria_Server.Networking;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;

namespace Terraria_Server.Plugins
{
	public static class HookPoints
	{
		public static readonly HookPoint<HookArgs.NewConnection>             NewConnection;
		
		public static readonly HookPoint<HookArgs.ConnectionRequestReceived> ConnectionRequestReceived;
		public static readonly HookPoint<HookArgs.DisconnectReceived>        DisconnectReceived;
		public static readonly HookPoint<HookArgs.ServerPassReceived>        ServerPassReceived;
		public static readonly HookPoint<HookArgs.PlayerPassReceived>        PlayerPassReceived;
		public static readonly HookPoint<HookArgs.PlayerDataReceived>        PlayerDataReceived;
		public static readonly HookPoint<HookArgs.StateUpdateReceived>       StateUpdateReceived;
		public static readonly HookPoint<HookArgs.InventoryItemReceived>     InventoryItemReceived;
		public static readonly HookPoint<HookArgs.ObituaryReceived>          ObituaryReceived;
		public static readonly HookPoint<HookArgs.TileChangeReceived>        TileChangeReceived;
		
		public static readonly HookPoint<HookArgs.DoorStateChanged>          DoorStateChanged;
		
		public static readonly HookPoint<HookArgs.LiquidFlowReceived>        LiquidFlowReceived;
		public static readonly HookPoint<HookArgs.ProjectileReceived>        ProjectileReceived;
		
		static HookPoints ()
		{
			NewConnection             = new HookPoint<HookArgs.NewConnection> ("new-connection");
			ConnectionRequestReceived = new HookPoint<HookArgs.ConnectionRequestReceived> ("connection-request-received");
			DisconnectReceived        = new HookPoint<HookArgs.DisconnectReceived> ("disconnect-received");
			ServerPassReceived        = new HookPoint<HookArgs.ServerPassReceived> ("server-pass-received");
			PlayerPassReceived        = new HookPoint<HookArgs.PlayerPassReceived> ("player-pass-received");
			PlayerDataReceived        = new HookPoint<HookArgs.PlayerDataReceived> ("player-data-received");
			StateUpdateReceived       = new HookPoint<HookArgs.StateUpdateReceived> ("state-update-received");
			InventoryItemReceived     = new HookPoint<HookArgs.InventoryItemReceived> ("inventory-item-received");
			ObituaryReceived          = new HookPoint<HookArgs.ObituaryReceived> ("obituary-received");
			TileChangeReceived        = new HookPoint<HookArgs.TileChangeReceived> ("tile-change-received");
			DoorStateChanged          = new HookPoint<HookArgs.DoorStateChanged> ("door-state-changed");
			LiquidFlowReceived        = new HookPoint<HookArgs.LiquidFlowReceived> ("liquid-flow-received");
			ProjectileReceived        = new HookPoint<HookArgs.ProjectileReceived> ("projectile-received");
		}
	}
	
	public static class HookArgs
	{
		public struct NewConnection
		{
		}
		
		public struct ConnectionRequestReceived
		{
			public string Version    { get; set; }
		}
		
		public struct DisconnectReceived
		{
			public string   Content    { get; set; }
			public string[] Lines      { get; set; }
		}
		
		public struct ServerPassReceived
		{
			public string Password   { get; set; }
		}
		
		public struct PlayerPassReceived
		{
			public string Password   { get; set; }
		}
		
		public struct PlayerDataReceived
		{
			public string Name       { get; set; }
			
			public byte   Hair       { get; set; }
			public bool   Male       { get; set; }
			public byte   Difficulty { get; set; }
			
			public Color  HairColor       { get; set; }
			public Color  SkinColor       { get; set; }
			public Color  EyeColor        { get; set; }
			public Color  ShirtColor      { get; set; }
			public Color  UndershirtColor { get; set; }
			public Color  PantsColor      { get; set; }
			public Color  ShoeColor       { get; set; }
			
			public bool   NameChecked     { get; set; }
			public bool   BansChecked     { get; set; }
			
			public void Parse (byte[] buf, int at, int length)
			{
				int start = at - 2;
				
				Hair = buf[at++];
				Male = buf[at++] == 1;
				
				HairColor       = ParseColor (buf, at); at += 3;
				SkinColor       = ParseColor (buf, at); at += 3;
				EyeColor        = ParseColor (buf, at); at += 3;
				ShirtColor      = ParseColor (buf, at); at += 3;
				UndershirtColor = ParseColor (buf, at); at += 3;
				PantsColor      = ParseColor (buf, at); at += 3;
				ShoeColor       = ParseColor (buf, at); at += 3;
				
				Difficulty = buf[at++];
				
				Name = System.Text.Encoding.ASCII.GetString (buf, at, length - at + start).Trim();
			}
			
			public void Apply (Player player)
			{
				player.hair = Hair;
				player.Male = Male;
				player.Difficulty = Difficulty;
				player.Name = Name;
				
				player.hairColor       = HairColor;
				player.skinColor       = SkinColor;
				player.eyeColor        = EyeColor;
				player.shirtColor      = ShirtColor;
				player.underShirtColor = UndershirtColor;
				player.shoeColor       = ShoeColor;
			}
			
			public static Color ParseColor (byte[] buf, int at)
			{
				return new Color (buf[at++], buf[at++], buf[at++]);
			}
			
			public bool CheckName (out string error)
			{
				error = null;
				NameChecked = true;
				
				if (Name.Length > 20)
				{
					error = "Invalid name: longer than 20 characters.";
					return false;
				}
	
				if (Name == "")
				{
					error = "Invalid name: whitespace or empty.";
					return false;
				}
				
				foreach (char c in Name)
				{
					if (c < 32 || c > 126)
					{
						Console.Write ((byte) c);
						error = "Invalid name: contains non-printable characters.";
						return false;
					}
					Console.Write (c);
				}
				
				if (Name.Contains (" " + " "))
				{
					error = "Invalid name: contains double spaces.";
					return false;
				}
				
				return true;
			}
		}
		
		public struct StateUpdateReceived
		{
			public byte Flags { get; set; }
			
			public byte SelectedItemIndex { get; set; }
			
			public float X { get; set; }
			public float Y { get; set; }
			
			public float VX { get; set; }
			public float VY { get; set; }
			
			public bool ControlUp
			{
				get { return (Flags & 1) != 0; }
				set { SetFlag (1, value); }
			}
			
			public bool ControlDown
			{
				get { return (Flags & 2) != 0; }
				set { SetFlag (2, value); }
			}
			
			public bool ControlLeft
			{
				get { return (Flags & 4) != 0; }
				set { SetFlag (4, value); }
			}
			
			public bool ControlRight
			{
				get { return (Flags & 8) != 0; }
				set { SetFlag (8, value); }
			}
			
			public bool ControlJump
			{
				get { return (Flags & 16) != 0; }
				set { SetFlag (16, value); }
			}
			
			public bool ControlUseItem
			{
				get { return (Flags & 32) != 0; }
				set { SetFlag (32, value); }
			}
			
			public int Direction
			{
				get { return ((Flags & 64) != 0) ? 1 : -1; }
				set { SetFlag (64, value == 1); }
			}
			
			internal void SetFlag (byte f, bool value)
			{
				if (value)
					Flags |= f;
				else
					Flags &= (byte) ~f;
			}
			
			public void ApplyKeys (Player player)
			{
				player.controlUp = ControlUp;
				player.controlDown = ControlDown;
				player.controlLeft = ControlLeft;
				player.controlRight = ControlRight;
				player.controlJump = ControlJump;
				player.controlUseItem = ControlUseItem;
			}
			
			public void ApplyParams (Player player)
			{
				player.selectedItemIndex = SelectedItemIndex;
				player.direction = Direction;
				player.Position = new Vector2 (X, Y);
				player.Velocity = new Vector2 (VX, VY);
			}
			
			public void Parse (byte[] buf, int at)
			{
				Flags = buf[at++];
				SelectedItemIndex = buf[at++];
				
				X = BitConverter.ToSingle (buf, at); at += 4;
				Y = BitConverter.ToSingle (buf, at); at += 4;
				VX = BitConverter.ToSingle (buf, at); at += 4;
				VY = BitConverter.ToSingle (buf, at);
			}
		}
		
		public struct InventoryItemReceived
		{
			public int    InventorySlot { get; set; }
			public int    Amount        { get; set; }
			public string Name          { get; set; }
		}
		
		public struct ObituaryReceived
		{
			public int    Direction { get; set; }
			public int    Damage    { get; set; }
			public byte   PvpFlag   { get; set; }
			public string Obituary  { get; set; }
		}
		
		public struct TileChangeReceived
		{
			public int    X         { get; set; }
			public int    Y         { get; set; }
			public byte   Action    { get; set; }
			public byte   Type      { get; set; }
			public byte   Style     { get; set; }
			
			public bool   TypeChecked { get; set; }
			
			public bool TileWasRemoved
			{
				get { return Action == 0 || Action == 4; }
			}
			
			public bool TileWasPlaced
			{
				get { return Action == 1; }
			}
			
			public bool WallWasRemoved
			{
				get { return Action == 2; }
			}
			
			public bool WallWasPlaced
			{
				get { return Action == 3; }
			}
			
			public bool RemovalFailed
			{
				get { return Type == 1 && (Action == 0 || Action == 2 || Action == 4); }
				set { if (Action == 0 || Action == 2 || Action == 4) Type = value ? (byte)1 : (byte)0; }
			}
		}
		
		public struct DoorStateChanged
		{
			public int  X         { get; set; }
			public int  Y         { get; set; }
			public int  Direction { get; set; }
			
			public bool Open      { get; set; }
			public bool Close  
			{
				get { return ! Open; }
				set { Open = ! value; }
			}
		}
		
		public struct LiquidFlowReceived
		{
			public int  X         { get; set; }
			public int  Y         { get; set; }
			public byte Amount    { get; set; }
			
			public bool Lava      { get; set; }
			public bool Water
			{
				get { return ! Lava; }
				set { Lava = ! value; }
			}
		}
		
		public struct ProjectileReceived
		{
			public short Id        { get; set; }
			public float X         { get; set; }
			public float Y         { get; set; }
			public float VX        { get; set; }
			public float VY        { get; set; }
			public float Knockback { get; set; }
			public short Damage    { get; set; }
			public byte  Owner     { get; set; }
			public byte  TypeByte  { get; set; }
			
			public ProjectileType Type
			{
				get { return (ProjectileType) TypeByte; }
				set { TypeByte = (byte) value; }
			}
		}
	}
}

