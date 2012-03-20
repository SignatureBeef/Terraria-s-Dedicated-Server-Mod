using System;
using System.Collections;
using System.Collections.Generic;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;
using Terraria_Server.Definitions;

namespace Terraria_Server.WorldMod
{
	public class PlayerSandbox : ISandbox
	{
		public struct MinMax
		{
			public int Min;
			public int Max;
		}
		
		internal TileData[] tiles = new TileData[16];
		internal int        count;
		internal Dictionary<uint, int> changedTiles = new Dictionary<uint, int> ();
		internal Dictionary<int, MinMax> changedRows = new Dictionary<int, MinMax> ();
		internal List<SideEffect> sideEffects = new List<SideEffect> ();
		
		public IEnumerable<SideEffect> SideEffects
		{
			get { return sideEffects; }
		}
		
		public void Initialize ()
		{
			Array.Clear (tiles, 0, count);
			count = 0;
			
			changedTiles.Clear ();
			changedRows.Clear ();
			
			//lock (sideEffectPool)
			{
				foreach (var eff in sideEffects)
				{
					sideEffectPool.Push (eff);
				}
			}
			
			sideEffects.Clear ();
		}
		
		public int ChangedTileCount { get { return changedTiles.Count; } }
		
		internal void Apply (Player player = null)
		{
			foreach (var kv in changedTiles)
			{
				var k = kv.Key;
				var x = k >> 16;
				var y = k & 0xffff;
				var t = kv.Value;
				
				if (t >= 0)
					Main.tile.data[x, y] = tiles[t];
			}
			
			foreach (var eff in sideEffects)
			{
				if (eff == null || eff.Cancel) continue;
				
				switch (eff.EffectType)
				{
					case SideEffectType.ADD_WATER:
					{
						Terraria_Server.Liquid.AddWater (null, null, eff.X, eff.Y); //Applying => Normal Tile References
						break;
					}
					
					case SideEffectType.SMASH_SHADOW_ORB:
					{
						WorldModify.shadowOrbSmashed = true;
						WorldModify.shadowOrbCount++;
						
						if (WorldModify.shadowOrbCount >= 3)
						{
							WorldModify.shadowOrbCount = 0;
							
							var ctx = new HookContext
							{
								Connection = player.Connection,
								Sender = player,
								Player = player,
							};
							
							var args = new HookArgs.PlayerTriggeredEvent
							{
								X = (int) (player.Position.X/16), 
								Y = (int) (player.Position.Y/16), 
								Type = WorldEventType.BOSS,
								Name = "Eater of Worlds",
							};
							
							HookPoints.PlayerTriggeredEvent.Invoke (ref ctx, ref args);
							
							if (!ctx.CheckForKick () && ctx.Result != HookResult.IGNORE)
							{
								//ProgramLog.Users.Log ("{0} @ {1}: Eater of Worlds summoned by {2}.", player.IPAddress, player.whoAmi, player.Name);
								//NetMessage.SendData (Packet.PLAYER_CHAT, -1, -1, string.Concat (player.Name, " has summoned the Eater of Worlds!"), 255, 255, 128, 150);
								NPC.SpawnOnPlayer(player.whoAmi, (int)NPCType.N13_EATER_OF_WORLDS_HEAD);
							}
						}
						else
						{
							string text = "A horrible chill goes down your spine...";
							if (WorldModify.shadowOrbCount == 2)
							{
								text = "Screams echo around you...";
							}
							NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f, 0);
						}
						break;
					}
					
					case SideEffectType.NEW_ITEM:
					{
						Item.NewItem (eff.X, eff.Y, eff.Width, eff.Height, eff.Type, eff.Stack, eff.NoBroadcast);
						break;
					}
					
					case SideEffectType.KILL_SIGN:
					{
						Sign.KillSign (eff.X, eff.Y);
						break;
					}
					
					case SideEffectType.DESTROY_CHEST:
					{
						Chest.DestroyChest (eff.X, eff.Y);
						break;
					}
					
					case SideEffectType.FALLING_BLOCK_PROJECTILE:
					{
						int p = Projectile.NewProjectile((float)(eff.X * 16 + 8), (float)(eff.Y * 16 + 8), 0f, 0.41f, eff.Type, 10, 0f, 255);
						var proj = Main.projectile[p];
						proj.Creator = player;
						proj.Velocity.Y = 0.5f;
						proj.Position.Y += 2f;
						proj.ai[0] = 1f;
						NetMessage.SendTileSquare (-1, eff.X, eff.Y, 1);
						break;
					}
				}
			}
		}
		
//		public IEnumerable<uint> ChangedTiles ()
//		{
//			foreach (var kv in changedTiles)
//			{
//				yield return kv.Key;
//			}
//		}
		
		public struct TileCoords
		{
			uint coords;
			
			internal TileCoords (uint coords)
			{
				this.coords = coords;
			}
			
			public int X { get { return (int) (coords >> 16); } }
			public int Y { get { return (int) (coords & 0xffff); } }
		}
		
		public struct ChangedTile
		{
			public readonly TileCoords Coords;
			public readonly TileData   Tile;
			
			internal ChangedTile (uint coords, TileData tile)
			{
				this.Coords = new TileCoords (coords);
				this.Tile = tile;
			}
		}
		
		public struct ChangedTileCoordsEnumerator : IEnumerator<TileCoords>, IEnumerator
		{
			Dictionary<uint, int>.Enumerator inner;
			
			internal ChangedTileCoordsEnumerator (Dictionary<uint, int>.Enumerator inner)
			{
				this.inner = inner;
			}
			
			public TileCoords Current { get { return new TileCoords (inner.Current.Key); } }
			object IEnumerator.Current { get { return new TileCoords (inner.Current.Key); } }
			
			public bool MoveNext ()
			{
				return inner.MoveNext ();
			}
			
			public void Dispose ()
			{
				inner.Dispose ();
			}
			
			public void Reset ()
			{
				((IEnumerator) inner).Reset ();
			}
			
			public ChangedTileCoordsEnumerator GetEnumerator ()
			{
				return this;
			}
		}
		
		public ChangedTileCoordsEnumerator ChangedTileCoords
		{
			get { return new ChangedTileCoordsEnumerator (changedTiles.GetEnumerator ()); }
		}
		
		public struct ChangedTileEnumerator : IEnumerator<ChangedTile>, IEnumerator
		{
			Dictionary<uint, int>.Enumerator inner;
			TileData[] tiles;
			
			internal ChangedTileEnumerator (Dictionary<uint, int>.Enumerator inner, TileData[] tiles)
			{
				this.inner = inner;
				this.tiles = tiles;
			}
			
			public ChangedTile Current { get { return new ChangedTile (inner.Current.Key, tiles[inner.Current.Value]); } }
			object IEnumerator.Current { get { return new ChangedTile (inner.Current.Key, tiles[inner.Current.Value]); } }
			
			public bool MoveNext ()
			{
				return inner.MoveNext ();
			}
			
			public void Dispose ()
			{
				inner.Dispose ();
			}
			
			public void Reset ()
			{
				((IEnumerator) inner).Reset ();
			}
			
			public ChangedTileEnumerator GetEnumerator ()
			{
				return this;
			}
		}
		
		public ChangedTileEnumerator ChangedTiles
		{
			get { return new ChangedTileEnumerator (changedTiles.GetEnumerator (), tiles); }
		}
		
		static Stack<SideEffect> sideEffectPool = new Stack<SideEffect> ();
		
		SideEffect AddSideEffect (SideEffectType type, int x, int y)
		{
			SideEffect eff = null;
			
			//lock (sideEffectPool)
			{
				if (sideEffectPool.Count > 0)
				{
					eff = sideEffectPool.Pop ();
					eff.Reset ();
				}
			}
			
			eff = eff ?? new SideEffect ();
			
			eff.EffectType = type;
			eff.X = x;
			eff.Y = y;
			
			sideEffects.Add (eff);
			
			return eff;
		}
		
		public void AddWater         (int x, int y)
		{
			AddSideEffect (SideEffectType.ADD_WATER, x, y);
		}
		
		public void ShadowOrbSmashed (int x, int y)
		{
			AddSideEffect (SideEffectType.SMASH_SHADOW_ORB, x, y);
		}

		public void NewItem(int x, int y, int w, int h, int type, int stack = 1, bool noBroadcast = false, int pfix = 0, int NetID = 255)
		{
			var eff = AddSideEffect (SideEffectType.NEW_ITEM, x, y);
			eff.Width = w;
			eff.Height = h;
			eff.Type = type;
			eff.Stack = stack;
			eff.Prefix = pfix;
			eff.NetId = NetID;
			eff.NoBroadcast = noBroadcast;
		}
		
		public void KillSign         (int x, int y)
		{
			AddSideEffect (SideEffectType.KILL_SIGN, x, y);
		}
		
		public void DestroyChest     (int x, int y)
		{
			AddSideEffect (SideEffectType.DESTROY_CHEST, x, y);
		}
		
		public void FallingBlockProjectile (int x, int y, int type)
		{
			var eff = AddSideEffect (SideEffectType.FALLING_BLOCK_PROJECTILE, x, y);
			eff.Type = type;
		}
		
		public TileData TileAt (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx];
			return Main.tile.At(x, y).Data;
		}
		
		public bool ActiveAt (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].Active;
			return Main.tile.At(x, y).Active;
		}
		
		public bool LitAt    (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].Lighted;
			return Main.tile.At(x, y).Lighted;
		}
		
		public bool LavaAt   (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].Lava;
			return Main.tile.At(x, y).Lava;
		}
		
		public byte TypeAt   (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].Type;
			return Main.tile.At(x, y).Type;
		}
		
		public byte WallAt   (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].Wall;
			return Main.tile.At(x, y).Wall;
		}
		
		public byte Liquid (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].Liquid;
			return Main.tile.At(x, y).Liquid;
		}
		
		public byte FrameAt  (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].FrameNumber;
			return Main.tile.At(x, y).FrameNumber;
		}
		
		public short FrameXAt (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].FrameX;
			return Main.tile.At(x, y).FrameX;
		}
		
		public short FrameYAt (int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint) (x << 16) | (uint) y, out idx))
				return tiles[idx].FrameY;
			return Main.tile.At(x, y).FrameY;
		}
		
		int Change (int x, int y)
		{
			int idx;
			uint k = (uint) (x << 16) | (uint) y;
			if (! changedTiles.TryGetValue (k, out idx))
			{
				if (count == tiles.Length)
				{
					var n = new TileData[2 * tiles.Length];
					Array.Copy (tiles, n, count);
					tiles = n;
				}
			
				idx = count;
				tiles[idx] = Main.tile.At(x, y).Data;
				count += 1;
				changedTiles[k] = idx;
			}
			
			MinMax row;
			if (! changedRows.TryGetValue (y, out row))
			{
				changedRows[y] = new MinMax { Min = x, Max = x };
			}
			else
			{
				changedRows[y] = new MinMax { Min = Math.Min(x, row.Min), Max = Math.Max (x, row.Max) };
			}
			
			return idx;
		}
		
		public void RevertAt (int x, int y)
		{
			uint k = (uint) (x << 16) | (uint) y;

            if (changedTiles.Remove(k)) { }
				//count -= 1;
		}
		
		public void SetTileAt (int x, int y, TileData data)
		{
			var idx = Change (x, y);
			tiles[idx] = data;
		}
		
		public void SetActiveAt (int x, int y, bool val)
		{
			var idx = Change (x, y);
			tiles[idx].Active = val;
		}
		
		public void SetLitAt    (int x, int y, bool val)
		{
			var idx = Change (x, y);
			tiles[idx].Lighted = val;
		}
		
		public void SetLavaAt   (int x, int y, bool val)
		{
			var idx = Change (x, y);
			tiles[idx].Lava = val;
		}
		
		public void SetTypeAt   (int x, int y, byte val)
		{
			var idx = Change (x, y);
			tiles[idx].Type = val;
		}
		
		public void SetWallAt   (int x, int y, byte val)
		{
			var idx = Change (x, y);
			tiles[idx].Wall = val;
		}
		
		public void SetLiquid (int x, int y, byte val)
		{
			var idx = Change (x, y);
			tiles[idx].Liquid = val;
		}
		
		public void SetFrameAt  (int x, int y, byte val)
		{
			var idx = Change (x, y);
			tiles[idx].FrameNumber = val;
		}
		
		public void SetFrameXAt (int x, int y, short val)
		{
			var idx = Change (x, y);
			tiles[idx].FrameX = val;
		}

		public void SetFrameYAt(int x, int y, short val)
		{
			var idx = Change(x, y);
			tiles[idx].FrameY = val;
		}

		public void SetWire(int x, int y, bool val)
		{
			var idx = Change(x, y);
			tiles[idx].Wire = val;
		}

		public bool Wire(int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint)(x << 16) | (uint)y, out idx))
				return tiles[idx].Wire;

			return Main.tile.At(x, y).Wire;
		}

		public void SetSkipLiquid(int x, int y, bool val)
		{
			var idx = Change(x, y);
			tiles[idx].SkipLiquid = val;
		}

		public bool SkipLiquid(int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint)(x << 16) | (uint)y, out idx))
				return tiles[idx].SkipLiquid;

			return Main.tile.At(x, y).SkipLiquid;
		}

		public void SetCheckingLiquid(int x, int y, bool val)
		{
			var idx = Change(x, y);
			tiles[idx].CheckingLiquid = val;
		}

		public bool CheckingLiquid(int x, int y)
		{
			int idx;
			if (changedTiles.TryGetValue((uint)(x << 16) | (uint)y, out idx))
				return tiles[idx].CheckingLiquid;

			return Main.tile.At(x, y).CheckingLiquid;
		}

		public void AddLiquid(int x, int y, int val)
		{
			var idx = Change(x, y);
			tiles[idx].liquid = (byte)(tiles[idx].liquid + val);
		}

		public void AddFrameX(int x, int y, int val)
		{
			var idx = Change(x, y);
			tiles[idx].FrameX = (byte)(tiles[idx].FrameX + val);
		}

		public void AddFrameY(int x, int y, int val)
		{
			var idx = Change(x, y);
			tiles[idx].FrameY = (byte)(tiles[idx].FrameY + val);
		}

		public bool Exists(int x, int y)
		{
			return changedTiles.ContainsKey((uint)(x << 16) | (uint)y);
		}
	}
}
