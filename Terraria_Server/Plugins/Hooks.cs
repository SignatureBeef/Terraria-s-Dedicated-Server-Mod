using System;

using Terraria_Server.Networking;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;
using Terraria_Server.Collections;
using Terraria_Server.Commands;
using Terraria_Server.Logging;

//
// TODO: split this file into one per hook with partial HookPoints and HookArgs classes,
//       or at least a couple per hook, grouped somehow
//

namespace Terraria_Server.Plugins
{
	public static class HookPoints
	{
		public static readonly HookPoint<HookArgs.NewConnection>             NewConnection;

		public static readonly HookPoint<HookArgs.ServerStateChange>         ServerStateChange;
        public static readonly HookPoint<HookArgs.ConsoleMessageReceived>    ConsoleMessageReceived;
		public static readonly HookPoint<HookArgs.PluginLoadRequest>         PluginLoadRequest;
        public static readonly HookPoint<HookArgs.UnkownSendPacket>          UnkownSendPacket;
        public static readonly HookPoint<HookArgs.UnkownReceivedPacket>      UnkownReceivedPacket;
		
		public static readonly HookPoint<HookArgs.ConnectionRequestReceived> ConnectionRequestReceived;
		public static readonly HookPoint<HookArgs.DisconnectReceived>        DisconnectReceived;
		public static readonly HookPoint<HookArgs.ServerPassReceived>        ServerPassReceived;
		public static readonly HookPoint<HookArgs.PlayerPassReceived>        PlayerPassReceived;
		public static readonly HookPoint<HookArgs.PlayerDataReceived>        PlayerDataReceived;
		public static readonly HookPoint<HookArgs.StateUpdateReceived>       StateUpdateReceived;
		public static readonly HookPoint<HookArgs.InventoryItemReceived>     InventoryItemReceived;
		public static readonly HookPoint<HookArgs.ObituaryReceived>          ObituaryReceived;
		public static readonly HookPoint<HookArgs.PlayerTeleport>            PlayerTeleport;
		
		public static readonly HookPoint<HookArgs.PlayerWorldAlteration>     PlayerWorldAlteration;
		
		public static readonly HookPoint<HookArgs.DoorStateChanged>          DoorStateChanged;
		
		public static readonly HookPoint<HookArgs.LiquidFlowReceived>        LiquidFlowReceived;
		public static readonly HookPoint<HookArgs.ProjectileReceived>        ProjectileReceived;
		public static readonly HookPoint<HookArgs.KillProjectileReceived>    KillProjectileReceived;
		public static readonly HookPoint<HookArgs.TileSquareReceived>        TileSquareReceived;
		
		public static readonly HookPoint<HookArgs.Explosion>                 Explosion;
		
		public static readonly HookPoint<HookArgs.ChestBreakReceived>        ChestBreakReceived;
		public static readonly HookPoint<HookArgs.ChestOpenReceived>         ChestOpenReceived;
		
		public static readonly HookPoint<HookArgs.PvpSettingReceived>        PvpSettingReceived;
		public static readonly HookPoint<HookArgs.PartySettingReceived>      PartySettingReceived;
		
		public static readonly HookPoint<HookArgs.PlayerEnteringGame>        PlayerEnteringGame;
		public static readonly HookPoint<HookArgs.PlayerEnteredGame>         PlayerEnteredGame;
		public static readonly HookPoint<HookArgs.PlayerLeftGame>            PlayerLeftGame;
		
		public static readonly HookPoint<HookArgs.SignTextSet>               SignTextSet;
		public static readonly HookPoint<HookArgs.SignTextGet>               SignTextGet;
		
		public static readonly HookPoint<HookArgs.PluginsLoaded>             PluginsLoaded;
		public static readonly HookPoint<HookArgs.WorldLoaded>               WorldLoaded;
		
		public static readonly HookPoint<HookArgs.PlayerHurt>                PlayerHurt;
		public static readonly HookPoint<HookArgs.NpcHurt>                   NpcHurt;
		public static readonly HookPoint<HookArgs.NpcCreation>               NpcCreation;
		public static readonly HookPoint<HookArgs.PlayerTriggeredEvent>      PlayerTriggeredEvent;
		
		public static readonly HookPoint<HookArgs.PlayerChat>                PlayerChat;
		public static readonly HookPoint<HookArgs.Command>                   Command;
		public static readonly HookPoint<HookArgs.WorldGeneration>			 WorldGeneration;
		public static readonly HookPoint<HookArgs.WorldRequestMessage>		 WorldRequestMessage;
		
		static HookPoints ()
		{
            UnkownReceivedPacket      = new HookPoint<HookArgs.UnkownReceivedPacket>("unkown-receive-packet");
            UnkownSendPacket          = new HookPoint<HookArgs.UnkownSendPacket>("unkown-send-packet");
			PlayerTeleport            = new HookPoint<HookArgs.PlayerTeleport> ("player-teleport");
            ConsoleMessageReceived    = new HookPoint<HookArgs.ConsoleMessageReceived>("console-message-received");
			ServerStateChange         = new HookPoint<HookArgs.ServerStateChange> ("server-state-change");
			NewConnection             = new HookPoint<HookArgs.NewConnection> ("new-connection");
			PluginLoadRequest         = new HookPoint<HookArgs.PluginLoadRequest> ("plugin-load-request");
			ConnectionRequestReceived = new HookPoint<HookArgs.ConnectionRequestReceived> ("connection-request-received");
			DisconnectReceived        = new HookPoint<HookArgs.DisconnectReceived> ("disconnect-received");
			ServerPassReceived        = new HookPoint<HookArgs.ServerPassReceived> ("server-pass-received");
			PlayerPassReceived        = new HookPoint<HookArgs.PlayerPassReceived> ("player-pass-received");
			PlayerDataReceived        = new HookPoint<HookArgs.PlayerDataReceived> ("player-data-received");
			StateUpdateReceived       = new HookPoint<HookArgs.StateUpdateReceived> ("state-update-received");
			InventoryItemReceived     = new HookPoint<HookArgs.InventoryItemReceived> ("inventory-item-received");
			ObituaryReceived          = new HookPoint<HookArgs.ObituaryReceived> ("obituary-received");
			PlayerWorldAlteration     = new HookPoint<HookArgs.PlayerWorldAlteration> ("player-world-alteration");
			DoorStateChanged          = new HookPoint<HookArgs.DoorStateChanged> ("door-state-changed");
			LiquidFlowReceived        = new HookPoint<HookArgs.LiquidFlowReceived> ("liquid-flow-received");
			ProjectileReceived        = new HookPoint<HookArgs.ProjectileReceived> ("projectile-received");
			KillProjectileReceived    = new HookPoint<HookArgs.KillProjectileReceived> ("kill-projectile-received");
			TileSquareReceived        = new HookPoint<HookArgs.TileSquareReceived> ("tile-square-received");
			ChestBreakReceived        = new HookPoint<HookArgs.ChestBreakReceived> ("cheat-break-received");
			ChestOpenReceived         = new HookPoint<HookArgs.ChestOpenReceived> ("cheat-open-received");
			PvpSettingReceived        = new HookPoint<HookArgs.PvpSettingReceived> ("pvp-setting-received");
			PartySettingReceived      = new HookPoint<HookArgs.PartySettingReceived> ("party-setting-received");
			PlayerEnteringGame        = new HookPoint<HookArgs.PlayerEnteringGame> ("player-entering-game");
			PlayerEnteredGame         = new HookPoint<HookArgs.PlayerEnteredGame> ("player-entered-game");
			PlayerLeftGame            = new HookPoint<HookArgs.PlayerLeftGame> ("player-left-game");
			Explosion                 = new HookPoint<HookArgs.Explosion> ("explosion");
			SignTextSet               = new HookPoint<HookArgs.SignTextSet> ("sign-text-set");
			SignTextGet               = new HookPoint<HookArgs.SignTextGet> ("sign-text-get");
			PluginsLoaded             = new HookPoint<HookArgs.PluginsLoaded> ("plugins-loaded");
			WorldLoaded               = new HookPoint<HookArgs.WorldLoaded> ("world-loaded");
			PlayerHurt                = new HookPoint<HookArgs.PlayerHurt> ("player-hurt");
			NpcHurt                   = new HookPoint<HookArgs.NpcHurt> ("npc-hurt");
			NpcCreation               = new HookPoint<HookArgs.NpcCreation> ("npc-creation");
			PlayerTriggeredEvent      = new HookPoint<HookArgs.PlayerTriggeredEvent> ("player-triggered-event");
			PlayerChat                = new HookPoint<HookArgs.PlayerChat> ("player-chat");
			Command                   = new HookPoint<HookArgs.Command> ("command");
			WorldGeneration			  = new HookPoint<HookArgs.WorldGeneration> ("world-generation");
			WorldRequestMessage		  = new HookPoint<HookArgs.WorldRequestMessage>("world-request-message");
		}
	}
	
	public static class HookArgs
    {
		public struct WorldRequestMessage
		{
			public int SpawnX { get; set; }
			public int SpawnY { get; set; }
		}

        public struct UnkownReceivedPacket
        {
            public ClientConnection Conn        { get; set; }
            public byte[]           ReadBuffer  { get; set; }
            public int              Start       { get; set; }
            public int              Length      { get; set; }
        }

        public struct UnkownSendPacket
        {
            public NetMessage   Message         { get; set; }
            public int          PacketId        { get; set; }
            public int          RemoteClient    { get; set; }
            public int          IgnoreClient    { get; set; }
            public string       Text            { get; set; }
            public int          Number          { get; set; }
            public float        Number2         { get; set; }
            public float        Number3         { get; set; }
            public float        Number4         { get; set; }
            public int          Number5         { get; set; }
        }

		public struct WorldGeneration { }

        public struct NewConnection
        {
        }

        public struct ConsoleMessageReceived
        {
            public string Message { get; set; }
			public SendingLogger Logger { get; set; }
        }

        public struct ServerStateChange
        {
            public ServerState ServerChangeState { get; set; }
        }

        public struct PlayerTeleport
        {
            public Vector2  ToLocation  { get; set; }
        }
		
		public struct PluginLoadRequest
		{
			public string     Path         { get; set; }
			public BasePlugin LoadedPlugin { get; set; }
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
                player.pantsColor      = PantsColor;
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
						//Console.Write ((byte) c);
						error = "Invalid name: contains non-printable characters.";
						return false;
					}
					//Console.Write (c);
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
			public int    Prefix        { get; set; }
			public int    NetID         { get; set; }
		}
		
		public struct ObituaryReceived
		{
			public int    Direction { get; set; }
			public int    Damage    { get; set; }
			public byte   PvpFlag   { get; set; }
			public string Obituary  { get; set; }
		}
		
		public struct PvpSettingReceived
		{
			public bool   PvpFlag   { get; set; }
		}
		
		public struct PartySettingReceived
		{
			public byte   Party     { get; set; }
		}
		
		public struct PlayerWorldAlteration
		{
			public int    X         { get; set; }
			public int    Y         { get; set; }
			public byte   Action    { get; set; }
			public byte   Type      { get; set; }
			public byte   Style     { get; set; }
			
			public bool   TypeChecked { get; set; }
			
			public WorldMod.PlayerSandbox Sandbox { get; internal set; }
			
			public bool TileWasRemoved
			{
				get { return Action == 0 || Action == 4 || Action == 100; }
			}
			
			public bool NoItem
			{
				get { return Action == 4 || Action == 101; }
				set
				{
					if (value)
					{
						if (Action == 0) Action = 4;
						else if (Action == 100) Action = 101;
					}
					else
					{
						if (Action == 4) Action = 0;
						else if (Action == 101) Action = 100;
					}
				}
			}
			
			public bool TileWasPlaced
			{
				get { return Action == 1; }
			}
			
			public bool WallWasRemoved
			{
				get { return Action == 2 || Action == 100 || Action == 101; }
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
			public float AI_0      { get; set; }
			public float AI_1      { get; set; }
			
			public int   ExistingIndex     { get; set; }
			
			internal Projectile projectile;
			
			public Projectile CreateProjectile ()
			{
				if (projectile != null) return projectile;
				
				var index = Projectile.ReserveSlot (Id, Owner);
				
				if (index == 1000) return null;
				
				projectile = Registries.Projectile.Create (TypeByte);
				
				projectile.whoAmI = index;
				Apply (projectile);
				
				return projectile;
			}
			
			public void Apply (Projectile projectile)
			{
				if (Owner < 255)
					projectile.Creator = Main.players[Owner];
				projectile.identity = Id;
				projectile.Owner = Owner;
				projectile.damage = Damage;
				projectile.knockBack = Knockback;
				projectile.Position = new Vector2 (X, Y);
				projectile.Velocity = new Vector2 (VX, VY);
				projectile.ai[0] = AI_0;
				projectile.ai[1] = AI_1;
			}
			
			internal void CleanupProjectile ()
			{
				if (projectile != null)
				{
					Projectile.FreeSlot (projectile.identity, projectile.Owner, projectile.whoAmI);
					projectile = null;
				}
			}
			
			public ProjectileType Type
			{
				get { return (ProjectileType) TypeByte; }
				set { TypeByte = (byte) value; }
			}

			public Projectile Current
			{
				get { return Main.projectile[Id]; }
			}
		}
		
		public struct KillProjectileReceived
		{
			public short Index { get; set; }
			public short Id    { get; set; }
			public byte  Owner { get; set; }
		}
		
		public struct Explosion
		{
			public Projectile Source { get; set; }
		}
		
		public struct ChestBreakReceived
		{
			public int X { get; set; }
			public int Y { get; set; }
		}
		
		public struct ChestOpenReceived
		{
			public int    X          { get; set; }
			public int    Y          { get; set; }
			public short  ChestIndex { get; set; } 
		}
		
		public struct PlayerEnteringGame
		{
			public int Slot { get; set; }
		}
		
		public struct PlayerEnteredGame
		{
			public int Slot { get; set; }
		}
		
		public struct PlayerLeftGame
		{
			public int Slot { get; set; }
		}
		
		public struct SignTextGet
		{
			public int    X         { get; set; }
			public int    Y         { get; set; }
			public short  SignIndex { get; set; } 
			public string Text      { get; set; }
		}
		
		public struct SignTextSet
		{
			public int    X         { get; set; }
			public int    Y         { get; set; }
			public short  SignIndex { get; set; } 
			public string Text      { get; set; }
			public Sign   OldSign   { get; set; }
		}
		
		public struct PluginsLoaded
		{
		}
		
		public struct WorldLoaded
		{
			public int Width  { get; set; }
			public int Height { get; set; }
		}
		
		public struct PlayerHurt
		{
			public Player Victim       { get; internal set; }
			public int    Damage       { get; set; }
			public int    HitDirection { get; set; }
			public bool   Pvp          { get; set; }
			public bool   Quiet        { get; set; }
			public string Obituary     { get; set; }
			public bool   Critical     { get; set; }
		}
		
		public struct NpcHurt
		{
			public NPC    Victim       { get; set; }
			public int    Damage       { get; set; }
			public int    HitDirection { get; set; }
			public float  Knockback    { get; set; }
			public bool   Critical     { get; set; }
		}
		
		public struct NpcCreation
		{
			public int    X          { get; set; }
			public int    Y          { get; set; }
			public string Name       { get; set; }
			public NPC    CreatedNpc { get; set; }
		}
		
		public struct PlayerTriggeredEvent
		{
			public int    X { get; set; }
			public int    Y { get; set; }
			
			public WorldEventType Type { get; set; }
			public string         Name { get; internal set; }
		}
		
		public struct PlayerChat
		{
			public string Message { get; set; }
			public Color  Color   { get; set; }
		}
		
		public struct Command
		{
			public string       Prefix           { get; internal set; }
			public ArgumentList Arguments        { get; set; }
			public string       ArgumentString   { get; set; }
		}
		
		public struct TileSquareReceived
		{
			public int  X    { get; internal set; }
			public int  Y    { get; internal set; }
			public int  Size { get; internal set; }
			
			internal byte[] readBuffer;
			internal int    start;
			internal int    applied;
			
			public void ForEach (object state, TileSquareForEachFunc func)
			{
				int num = start;
				
				for (int x = X; x < X + Size; x++)
				{
					for (int y = Y; y < Y + Size; y++)
					{
						TileData tile = Main.tile.At(x, y).Data;
						
						byte b9 = readBuffer[num++];
						
						bool wasActive = tile.Active;
						
						tile.Active = ((b9 & 1) == 1);
						
						if ((b9 & 2) == 2)
						{
							tile.Lighted = true;
						}
						
						if (tile.Active)
						{
							int wasType = (int)tile.Type;
							tile.Type = readBuffer[num++];
							
							if (tile.Type < Main.MAX_TILE_SETS && Main.tileFrameImportant[(int)tile.Type])
							{
								tile.FrameX = BitConverter.ToInt16 (readBuffer, num);
								num += 2;
								tile.FrameY = BitConverter.ToInt16 (readBuffer, num);
								num += 2;
							}
							else if (!wasActive || (int)tile.Type != wasType)
							{
								tile.FrameX = -1;
								tile.FrameY = -1;
							}
						}
						
						if ((b9 & 4) == 4)
							tile.Wall = readBuffer[num++];
						else
							tile.Wall = 0;
						
						if ((b9 & 8) == 8)
						{
							tile.Liquid = readBuffer[num++];
							byte b10 = readBuffer[num++];
							tile.Lava = (b10 == 1);
						}
						else
							tile.Liquid = 0;
						
						var result = func (x, y, ref tile, state);
						if (result == TileSquareForEachResult.ACCEPT)
						{
							applied += 1;
							Main.tile.At(x, y).SetData (tile);
						}
						else if (result == TileSquareForEachResult.BREAK)
						{
							return;
						}
					}
				}
			}
		}
	}
	
	public enum TileSquareForEachResult
	{
		ACCEPT,
		IGNORE,
		BREAK,
	}
	
	public delegate TileSquareForEachResult TileSquareForEachFunc (int x, int y, ref TileData tile, object state);
	
	public enum WorldEventType
	{
		BOSS,
		INVASION,
		SHADOW_ORB,
	}
}

