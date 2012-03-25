using System;
using System.Text;
using System.IO;

using Terraria_Server.Commands;
using Terraria_Server.Messages;
using Terraria_Server.Misc;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;

namespace Terraria_Server
{
	public partial class NetMessage
	{
		public void ConnectionRequest (string version)
		{
			Begin (Packet.CONNECTION_REQUEST);
			String ("Terraria_Server");
			String (version);
			End ();
		}
		
		public void Disconnect (string reason)
		{
			Begin (Packet.DISCONNECT);
			String (reason);
			End ();
		}
		
		public void ConnectionResponse (int clientId)
		{
			Header (Packet.CONNECTION_RESPONSE, 4);
			Int (clientId);
		}
		
		public void PlayerData (int playerId)
		{
			var player = Main.players[playerId];
			
			Begin (Packet.PLAYER_DATA);
			
			Byte (playerId);

			Byte (player.hair);
			Byte (player.Male ? 1 : 0);
			
			Byte (player.hairColor.R);
			Byte (player.hairColor.G);
			Byte (player.hairColor.B);

			Byte (player.skinColor.R);
			Byte (player.skinColor.G);
			Byte (player.skinColor.B);

			Byte (player.eyeColor.R);
			Byte (player.eyeColor.G);
			Byte (player.eyeColor.B);

			Byte (player.shirtColor.R);
			Byte (player.shirtColor.G);
			Byte (player.shirtColor.B);

			Byte (player.underShirtColor.R);
			Byte (player.underShirtColor.G);
			Byte (player.underShirtColor.B);

			Byte (player.pantsColor.R);
			Byte (player.pantsColor.G);
			Byte (player.pantsColor.B);

			Byte (player.shoeColor.R);
			Byte (player.shoeColor.G);
			Byte (player.shoeColor.B);
			
			Byte (player.Difficulty);
			String (player.Name);
			
			End ();
		}
		
		public void InventoryData (int playerId, int invId, int prefix)
		{
			var player = Main.players[playerId];
			
			Begin (Packet.INVENTORY_DATA);
			
			Byte (playerId);
			Byte (invId);
			
			Item item;
			
			if (invId < 49)
				item = player.inventory[invId];
			else
				item = player.armor[invId - 49];
			
			Byte (Math.Max (0, item.Stack));
			
			Byte (prefix);
			
			Short (item.NetID);
			
			End ();
		}
		
		public void WorldRequest ()
		{
			Header (Packet.WORLD_REQUEST, 0);
		}

		public void WorldData(int spawnX, int spawnY, bool hardMode)
		{
			Begin(Packet.WORLD_DATA);

			Int(Main.Time);
			Byte(Main.dayTime);
			Byte(Main.moonPhase);
			Byte(Main.bloodMoon);

			Int(Main.maxTilesX);
			Int(Main.maxTilesY);
			Int(spawnX);
			Int(spawnY);

			Int(Main.worldSurface);
			Int(Main.rockLayer);
			Int(Main.worldID);

			byte flags = 0;

			if (WorldModify.shadowOrbSmashed) flags += 1;
			if (NPC.downedBoss1) flags += 2;
			if (NPC.downedBoss2) flags += 4;
			if (NPC.downedBoss3) flags += 8;
			if (hardMode) flags += 16;
			if (NPC.downedClown) flags += 32;

			Byte(flags);

			String(Main.worldName);

			End();
		}

		public void WorldData(int spawnX, int spawnY)
		{
			WorldData(spawnX, spawnY, Main.hardMode);
		}

		public void WorldData()
		{
			WorldData(Main.spawnTileX, Main.spawnTileY);
		}

		public void WorldData(bool hardMode)
		{
			WorldData(Main.spawnTileX, Main.spawnTileY, hardMode);
		}
		
		public void RequestTileBlock ()
		{
			throw new NotImplementedException ("NetMessage.RequestTileBlock()");
		}
		
		public void SendTileLoading (int number, string text)
		{
			Begin (Packet.SEND_TILE_LOADING);
			
			Int (number);
			String (text);
			
			End ();
		}
		
		private void Tile (TileData tile)
		{
			byte flags = 0;
			
			var active = tile.Active;
			var wall   = tile.Wall;
			var liquid = tile.Liquid;
			
			if (active)          flags += 1;
			//if (tile.Lighted)    flags += 2; //UNUSED
			if (wall > 0)        flags += 4;
			if (liquid > 0)      flags += 8;
			if (tile.Wire)       flags += 16;
			
			Byte (flags);
			
			if (active)
			{
				var type = tile.Type;
				
				Byte (type);
				
				if (Main.tileFrameImportant [type])
				{
					Short (tile.FrameX);
					Short (tile.FrameY);
				}
			}
			
			if (wall > 0)
			{
				Byte (wall);
			}
			
			if (liquid > 0)
			{
				Byte (liquid);
				Byte (tile.Lava);
			}
		}
		
		private void Tile (int x, int y)
		{
			Tile (Main.tile.At (x, y).Data);
		}

		
//#if TEST_COMPRESSION
		private int TileSize (TileData tile)
		{
			int count = 1;
			
			var active = tile.Active;
			var wall   = tile.Wall;
			var liquid = tile.Liquid;
			
			if (active)
			{
				var type = tile.Type;
				
				count += 1;
				
				if (Main.tileFrameImportant [type])
				{
					count += 4;
				}
			}
			
			if (wall > 0)
			{
				count += 1;
			}
			
			if (liquid > 0)
			{
				count += 2;
			}
			
			return count;
		}
		
		private byte CompressedTileFlags (TileData tile, TileData last)
		{
			byte flags = 0;
			
			var active = tile.Active;
			var type   = tile.Type;

			if (active != last.Active)        flags |= 1;
			if (tile.Lighted != last.Lighted) flags |= 2;
			if (tile.Wall != last.Wall)       flags |= 4;
			if (tile.Liquid != last.Liquid)   flags |= 8;
			if (tile.Lava != last.Lava)       flags |= 16;
			if (active)
			{
				if (last.Type != type || (flags & 1) != 0) flags |= 32;
				 
				if (Main.tileFrameImportant [type] && (last.FrameX != tile.FrameX || last.FrameY != tile.FrameY || (flags & 1) != 0))
				{
					flags |= 64;
				}
			}
			
			return flags;
		}
		
		private void CompressedTileBody (byte flags, TileData tile)
		{
			Byte (flags);
			
			var type   = tile.Type;
			var wall   = tile.Wall;
			var liquid = tile.Liquid;
			
			if (tile.Active)
			{
				if ((flags & 32) != 0)
				{
					Byte (type);
				}
				
				if ((flags & 64) != 0)
				{
					Short (tile.FrameX);
					Short (tile.FrameY);
				}
			}
			
			if ((flags & 4) != 0)
			{
				Byte (wall);
			}
			
			if ((flags & 8) != 0)
			{
				Byte (liquid);
			}
		}
		
		public void TileRowCompressed (int numColumns, int firstColumn, int row)
		{
			Begin (Packet.TILE_ROW_COMPRESSED);
			
			Short (numColumns);
			Int (firstColumn);
			Int (row);
			
			TileData last = default(TileData);
			int run = 0;
			
			for (int col = firstColumn; col < firstColumn + numColumns; col++)
			{
				var tile = Main.tile.At (col, row).Data;
				
				byte flags = CompressedTileFlags (tile, last);
				if (flags != 0)
				{
					while (run > 0)
					{
						int count = Math.Min (run, 127);
						Byte (count | 128);
						run -= count;
					}
					
					CompressedTileBody (flags, tile);
					
					last = tile;
				}
				else
				{
					run += 1;
				}
			}
			
			while (run > 0)
			{
				int count = Math.Min (run, 127);
				Byte (count | 128);
				run -= count;
			}
			
			End ();
		}

		public int TileRowSize (int numColumns, int firstColumn, int row)
		{
			int count = 5 + 2 + 4 + 4;
			
			for (int col = firstColumn; col < firstColumn + numColumns; col++)
			{
				count += TileSize (Main.tile.At (col, row).Data);
			}
			
			return count;
		}
//#endif

		public void SendTileRow (int numColumns, int firstColumn, int row)
		{
			Begin (Packet.SEND_TILE_ROW);
			
			Short (numColumns);
			Int (firstColumn);
			Int (row);
			
			for (int col = firstColumn; col < firstColumn + numColumns; col++)
			{
				var tile = Main.tile.At (col, row).Data;
				
				Tile (tile);
								
				int run = 0;
				
				for (int i = col + 1; i < firstColumn + numColumns; i++)
				{
					var tile2 = Main.tile.At (i, row).Data;
					
					if (tile.Active != tile2.Active) break;
					
					if (tile.Active)
					{
						if (tile.Type != tile2.Type) break;
						
						if (Main.tileFrameImportant[tile.Type])
						{
							if (tile.FrameX != tile2.FrameX || tile.FrameY != tile2.FrameY) break;
						}
					}
					
					if (tile.Wall != tile2.Wall || tile.Liquid != tile2.Liquid || tile.Lava != tile2.Lava || tile.Wire != tile2.Wire)
						break;
					
					run += 1;
				}
				
				Short (run);
				col += run;
			}
			
			End ();
		}
		
		public void SendTileConfirm (int sectionX, int sectionY, int sectionXAgain, int sectionYAgain)
		{
			Begin (Packet.SEND_TILE_CONFIRM);
			
			Int (sectionX);
			Int (sectionY);
			Int (sectionXAgain);
			Int (sectionYAgain);
			
			End ();
		}
		
		public void ReceivingPlayerJoined (int playerId)
		{
			var player = Main.players[playerId];
			
			Begin (Packet.RECEIVING_PLAYER_JOINED);
			
			Byte (playerId);
			Int (player.SpawnX);
			Int (player.SpawnY);
			
			End ();
		}
		
		public void ReceivingPlayerJoined (int playerId, int sx, int sy)
		{
			Begin (Packet.RECEIVING_PLAYER_JOINED);
			
			Byte (playerId);
			Int (sx);
			Int (sy);
			
			End ();
		}
		
		public void PlayerStateUpdate (int playerId)
		{
			var player = Main.players[playerId];
			
			Begin (Packet.PLAYER_STATE_UPDATE);
			
			byte flags = 0;
			if (player.controlUp)      flags += 1;
			if (player.controlDown)    flags += 2;
			if (player.controlLeft)    flags += 4;
			if (player.controlRight)   flags += 8;
			if (player.controlJump)    flags += 16;
			if (player.controlUseItem) flags += 32;
			if (player.direction == 1) flags += 64;
			
			Byte (playerId);
			Byte (flags);
			Byte (player.selectedItemIndex);
			
			Float (player.Position.X);
			Float (player.Position.Y);
			Float (player.Velocity.X);
			Float (player.Velocity.Y);
			
			End ();
		}
		
		public void PlayerStateUpdate (int playerId, float px, float py)
		{
			var player = Main.players[playerId];
			
			Begin (Packet.PLAYER_STATE_UPDATE);
			
			byte flags = 0;
			if (player.controlUp)      flags += 1;
			if (player.controlDown)    flags += 2;
			if (player.controlLeft)    flags += 4;
			if (player.controlRight)   flags += 8;
			if (player.controlJump)    flags += 16;
			if (player.controlUseItem) flags += 32;
			if (player.direction == 1) flags += 64;
			
			Byte (playerId);
			Byte (flags);
			Byte (player.selectedItemIndex);
			
			Float (px);
			Float (py);
			Float (player.Velocity.X);
			Float (player.Velocity.Y);
			
			End ();
		}
		
		public void SynchBegin (int playerId, int active)
		{
			Header (Packet.SYNCH_BEGIN, 2);

			Byte(playerId);
			Byte(active);
		}
		
		public void UpdatePlayers ()
		{
			//Header (Packet.UPDATE_PLAYERS, 0);
			throw new NotImplementedException("NetMessage.UpdatePlayers()");
		}
		
		public void PlayerHealthUpdate (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.PLAYER_HEALTH_UPDATE, 5);
			
			Byte (playerId);
			Short (player.statLife);
			Short (player.statLifeMax);
		}
		
		public void TileBreak (int tileAction, int x, int y, int tileType = 0, int style = 0)
		{
			Header (Packet.TILE_BREAK, 11);
			
			Byte (tileAction);
			Int (x);
			Int (y);
			Byte (tileType);
			Byte (style);
		}
		
		public void TimeSunMoonUpdate ()
		{
			throw new NotImplementedException ("NetMessage.TimeSunMoonUpdate()");
		}
		
		public void DoorUpdate (int doorAction, int x, int y, int doorDirection)
		{
			Header (Packet.DOOR_UPDATE, 10);
			
			Byte (doorAction);
			Int (x);
			Int (y);
			Byte (doorDirection == 1);
		}
		
		public void TileSquare (int size, int X, int Y)
		{
			Begin (Packet.TILE_SQUARE);
			
			Short (size);
			Int (X);
			Int (Y);
			
			for (int x = X; x < X + size; x++)
			{
				for (int y = Y; y < Y + size; y++)
				{
					Tile (x, y);
				}
			}
			
			End ();
		}
		
		public void SingleTileSquare (int X, int Y, TileData tile)
		{
			Begin (Packet.TILE_SQUARE);
			
			Short (1);
			Int (X);
			Int (Y);
			
			Tile (tile);
			
			End ();
		}
		
		public void ItemInfo (int itemId)
		{
			var item = Main.item[itemId];
			
			Begin (Packet.ITEM_INFO);
			
			Short (itemId);
			
			Float (item.Position.X);
			Float (item.Position.Y);
			Float (item.Velocity.X);
			Float (item.Velocity.Y);
			
			Byte (item.Stack);
			Byte (item.Prefix);
			
			if (item.Active && item.Stack > 0)
				Short (item.NetID);
			else
				Short (0);
			
			End ();
		}
		
		public void ItemOwnerInfo (int itemId)
		{
			var item = Main.item[itemId];
			
			Header (Packet.ITEM_OWNER_INFO, 3);
			
			Short (itemId);
			Byte (item.Owner);
		}
		
		public void NPCInfo (int npcId)
		{
			var npc = Main.npcs[npcId];
			
			Begin (Packet.NPC_INFO);
			
			Short (npcId);
			
			Float (npc.Position.X);
			Float (npc.Position.Y);
			Float (npc.Velocity.X);
			Float (npc.Velocity.Y);
			
			Short (npc.target);
			
			Byte (npc.direction + 1);
			Byte (npc.directionY + 1);
			
			if (npc.Active)
				Int (npc.life);
			else
				Int(0);

			if (!npc.Active || npc.life <= 0)
				npc.netSkip = 0;
			
			for (int i = 0; i < NPC.MAX_AI; i++)
				Float (npc.ai[i]);
			
			Int (npc.NetID);
			
			End ();
		}
		
		public void StrikeNPC (int npcId, int playerId)
		{
			Header (Packet.STRIKE_NPC, 3);
			
			Short (npcId);
			Byte (playerId);
		}
		
		public void PlayerChat (int playerId, string text, int r, int g, int b)
		{
			Begin (Packet.PLAYER_CHAT);
			
			Byte (playerId);
			Byte (r);
			Byte (g);
			Byte (b);

			String (text ?? System.String.Empty);
			
			End ();
		}
		
		public void StrikePlayer (int victimId, string deathText, int direction, int damage, int pvpFlag, bool crit = false)
		{
			Begin (Packet.STRIKE_PLAYER);
			
			Byte (victimId);
			Byte (direction + 1);
			Short (damage);
			Byte (pvpFlag);
			Byte (crit ? 1 : 0);
			
			String (deathText);
			
			End ();
		}
		
		public void Projectile (Projectile proj)
		{
			Begin (Packet.PROJECTILE);
			
			Short (proj.identity);
			
			Float (proj.Position.X);
			Float (proj.Position.Y);
			Float (proj.Velocity.X);
			Float (proj.Velocity.Y);
			Float (proj.knockBack);
			
			Short (proj.damage);
			
			Byte (proj.Owner);
			Byte ((byte) proj.type);
			
			for (int i = 0; i < Terraria_Server.Projectile.MAX_AI; i++)
				Float (proj.ai[i]);
			
			End ();
		}
		
		public void EraseProjectile (int id, int owner)
		{
			Begin (Packet.PROJECTILE);
			
			Short (id);
			
			Float (-1000);
			Float (-1000);
			Float (1);
			Float (1);
			Float (0);
			
			Short (0);
			
			Byte (owner);
			Byte (0);
			
			for (int i = 0; i < Terraria_Server.Projectile.MAX_AI; i++)
				Float (0.0f);
			
			End ();
		}
		
		public void DamageNPC (int npcId, int damage, float knockback, int direction, bool crit = false)
		{
			Header (Packet.DAMAGE_NPC, 10);
			
			Short (npcId);
			Short (damage);
			
			Float (knockback);
			
			Byte (direction + 1);
			Byte (crit ? 1 : 0);
		}
		
		public void KillProjectile (int identity, int owner)
		{
			Header (Packet.KILL_PROJECTILE, 3);
			
			Short (identity);
			Byte (owner);
		}
		
		public void PlayerPVPChange (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.PLAYER_PVP_CHANGE, 2);
			
			Byte (playerId);
			Byte (player.hostile);
		}
		
		public void OpenChest ()
		{
			throw new NotImplementedException ("NetMessage.OpenChest()");
		}
		
		public void ChestItem (int chestId, int itemId)
		{
			var chest = Main.chest[chestId];
			var item = chest.contents[itemId];
			
			Begin (Packet.CHEST_ITEM);
			
			Short (chestId);
			
			Byte (itemId);
			Byte (item.Stack);
			Byte (item.Prefix);
			
			if (item.Name == null)
				Short (0);
			else
				Short (item.NetID);
			
			End ();
		}
		
		public void PlayerChestUpdate (int chestId)
		{
			Header (Packet.PLAYER_CHEST_UPDATE, 10);
			
			Short (chestId);
			
			if (chestId > -1)
			{
				var chest = Main.chest[chestId];
				
				Int (chest.x);
				Int (chest.y);
			}
			else
			{
				Int (0);
				Int (0);
			}
		}
		
		public void KillTile ()
		{
			throw new NotImplementedException ("NetMessage.KillTile()");
		}
		
		public void HealPlayer (int playerId, int amount)
		{
			Header (Packet.HEAL_PLAYER, 3);
			
			Byte (playerId);
			Short (amount);
		}
		
		public void EnterZone (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.ENTER_ZONE, 6);
			
			Byte (playerId);
			
			Byte (player.zoneEvil);
			Byte (player.zoneMeteor);
			Byte (player.zoneDungeon);
			Byte (player.zoneJungle);
			Byte (player.zoneHoly);
		}
		
		public void PasswordRequest ()
		{
			Header (Packet.PASSWORD_REQUEST, 0);
		}
		
		public void PasswordResponse ()
		{
			throw new NotImplementedException ("NetMessage.PasswordResponse()");
		}
		
		public void ItemOwnerUpdate (int itemId)
		{
			Header (Packet.ITEM_OWNER_UPDATE, 2);
			
			Short (itemId);
		}
		
		public void NPCTalk (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.NPC_TALK, 3);
			
			Byte (playerId);
			Short (player.talkNPC);
		}
		
		public void PlayerBallswing (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.PLAYER_BALLSWING, 7);
			
			Byte (playerId);
			Float (player.itemRotation);
			Short (player.itemAnimation);
		}
		
		public void PlayerManaUpdate (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.PLAYER_MANA_UPDATE, 5);
			
			Byte (playerId);
			Short (player.statMana);
			Short (player.statManaMax);
		}
		
		public void PlayerUseManaUpdate (int playerId, int amount)
		{
			Header (Packet.PLAYER_USE_MANA_UPDATE, 3);
			
			Byte (playerId);
			Short (amount);
		}
		
		public void KillPlayerPVP (int victimId, string deathText, int direction, int damage, int pvpFlag)
		{
			Begin (Packet.KILL_PLAYER_PVP);
			
			Byte (victimId);
			Byte (direction + 1);
			Short (damage);
			Byte (pvpFlag);
			
			String (deathText);
			
			End ();
		}
		
		public void PlayerJoinParty (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.PLAYER_JOIN_PARTY, 2);
			
			Byte (playerId);
			Byte (player.team);
		}
		
		public void ReadSign (int x, int y)
		{
			Header (Packet.READ_SIGN, 8);
			
			Int (x);
			Int (y);
		}
		
		public void WriteSign (int signId)
		{
			var sign = Main.sign[signId];
			
			Begin (Packet.WRITE_SIGN);
			
			Short (signId);
			Int (sign.x);
			Int (sign.y);
			
			String (sign.text, true);
			
			End ();
		}
		
		public void WriteSign (int signId, int x, int y, string text)
		{
			Begin (Packet.WRITE_SIGN);
			
			Short (signId);
			Int (x);
			Int (y);
			
			String (text, true);
			
			End ();
		}
		
		public void FlowLiquid (int x, int y)
		{
			var tile = Main.tile.At(x, y);
			
			Header (Packet.FLOW_LIQUID, 10);
			
			Int (x);
			Int (y);
			Byte (tile.Liquid);
			Byte (tile.Lava);
		}
		
		public void SendSpawn ()
		{
			Header (Packet.SEND_SPAWN, 0);
		}
		
		public void PlayerBuffs (int playerId)
		{
			var player = Main.players[playerId];
			
			Header (Packet.PLAYER_BUFFS, 11);
		
			Byte (playerId);
			
			for (int i = 0; i < 10; i++)
			{
				Byte (player.buffType[i]);
			}
		}
		
		public void SummonSkeletron (int action, int param)
		{
			Header (Packet.SUMMON_SKELETRON, 2);
			
			Byte (action);
			Byte (param);
		}
		
		public void ChestUnlock (int playerId, int param, int x, int y)
		{
			Header (Packet.CHEST_UNLOCK, 10);
			
			Byte (playerId);
			Byte (param);
			
			Int (x);
			Int (y);
		}
		
		public void NPCAddBuff (int npcId, int type, int time)
		{
			Header (Packet.NPC_ADD_BUFF, 5);
			
			Short (npcId);
			Byte (type);
			Short (time);
		}
		
		public void NPCBuffs (int npcId)
		{
			Begin (Packet.NPC_BUFFS);
			
			Short (npcId);
			
			var npc = Main.npcs[npcId];
			for (int i = 0; i < 5; i++)
			{
				Byte (npc.buffType[i]);
				Int (npc.buffTime[i]);
			}
			
			End ();
		}
		
		public void PlayerAddBuff (int playerId, int type, int time)
		{
			Header (Packet.PLAYER_ADD_BUFF, 4);
			
			Byte (playerId);
			Byte (type);
			Short (time);
		}
		
		public void NPCName (int id, string text)
		{
			Begin (Packet.NPC_NAME);
			
			Short (id);
			String (text);
			
			End ();
		}
		
		public void WorldBalance (int good, int evil)
		{
			Header (Packet.WORLD_BALANCE, 2);
			
			Byte (good);
			Byte (evil);
		}
		
		public void PlayHarp (int playerId, float note)
		{
			Header (Packet.PLAY_HARP, 5);
			
			Byte (playerId);
			Float (note);
		}
		
		public void HitSwitch (int x, int y)
		{
			Header (Packet.HIT_SWITCH, 8);
			
			Int (x);
			Int (y);
		}
		
		public void NPCHome (int npcId, int homeTileX, int homeTileY, bool homeless)
		{
			Header (Packet.NPC_HOME, 7);
			
			Short (npcId);
			Short (homeTileX);
			Short (homeTileY);
			Byte (homeless);
		}

        public void ClientMod(int PlayerID)
        {
            //Let them know they are op
            var player = Main.players[PlayerID];
            Header(Packet.CLIENT_MOD, 4);

            Int((player.Op) ? 1 : 0);

            //Tell whether RPG is allowed.
            Int((Server.AllowTDCMRPG) ? 1 : 0);
        }

        public void RpgNPCSpawned(int npcId)
        {
            Header(Packet.CLIENT_MOD_SPAWN_NPC, 4);

            Int(npcId);
        }

		public void SpawnNPC(int PlayerId, int NPCId) //Can do invasion too (-1)
		{
			/*Header(Packet.SPAWN_NPCS, 8);

			Int(PlayerId);
			Int(NPCId);*/
			throw new Exception("NetMessage.SpawnNPC");
		}
	}
}
