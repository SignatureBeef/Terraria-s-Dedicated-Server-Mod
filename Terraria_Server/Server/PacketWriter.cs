using System;
using System.IO;
using Terraria_Server.Misc;
using System.Text;

namespace Terraria_Server
{
	public class PacketWriter
	{
		private Stream sink;
		private BinaryWriter bin;
		private int lenAt;
		
		public PacketWriter (Stream stream)
		{
			sink = stream;
			bin = new BinaryWriter (stream, Encoding.ASCII);
		}
		
		private void Begin ()
		{
			lenAt = (int) sink.Position;
			sink.Position += 4;
		}

		private void Begin (Packet id)
		{
			lenAt = (int) sink.Position;
			sink.Position += 4;
			sink.WriteByte ((byte) id);
		}
		
		private void End ()
		{
			var pos = sink.Position;
			sink.Position = lenAt;
			bin.Write ((int) (pos - lenAt - 4));
			sink.Position = pos;
		}
		
		private void Header (Packet id, int length)
		{
			bin.Write (length + 1);
			sink.WriteByte ((byte) id);
		}
		
		private void Byte (byte data)
		{
			sink.WriteByte (data);
		}

		private void Byte (int data)
		{
			sink.WriteByte ((byte) data);
		}

		private void Byte (bool data)
		{
			sink.WriteByte ((byte) (data ? 1 : 0));
		}
		
		private void Short (short data)
		{
			bin.Write (data);
		}

		private void Short (int data)
		{
			bin.Write ((short) data);
		}

		private void Int (int data)
		{
			bin.Write (data);
		}
		
		private void Int (double data)
		{
			bin.Write ((int) data);
		}
		
		private void Float (float data)
		{
			bin.Write (data);
		}
		
		private void String (string data)
		{
			bin.Write (data);
		}
		
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
		
		public void PlayerData (Player player)
		{
			Begin (Packet.PLAYER_DATA);
			
			Byte (player.whoAmi);

			Byte (player.hair);
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
			
			Byte (player.hardCore);
			String (player.Name);
			
			End ();
		}
		
		public void InventoryData (Player player, int invId, string text = "")
		{
			Begin (Packet.INVENTORY_DATA);
			
			Byte (player.whoAmi);
			Byte (invId);
			
			if (invId < 44)
				Byte (Math.Max (0, player.inventory[invId].Stack));
			else
				Byte (Math.Max (0, player.armor[invId].Stack));
				
			String (text ?? "");
			
			End ();
		}
		
		public void WorldRequest ()
		{
			Header (Packet.WORLD_REQUEST, 0);
		}
		
		public void WorldData ()
		{
			Begin (Packet.WORLD_DATA);
			
			Int (Main.time);
			Byte (Main.dayTime);
			Byte (Main.moonPhase);
			Byte (Main.bloodMoon);
			
			Int (Main.maxTilesX);
			Int (Main.maxTilesY);
			Int (Main.spawnTileX);
			Int (Main.spawnTileY);
			
			Int (Main.worldSurface);
			Int (Main.rockLayer);
			Int (Main.worldID);
			
			byte flags = 0;
			
			if (WorldGen.shadowOrbSmashed) flags += 1;
			if (NPC.downedBoss1)           flags += 2;
			if (NPC.downedBoss2)           flags += 4;
			if (NPC.downedBoss3)           flags += 8;
			
			Byte (flags);
			
			String (Main.worldName);
			
			End ();
		}
		
		public void RequestTileBlock ()
		{
			throw new NotImplementedException ("PacketWriter.RequestTileBlock()");
		}
		
		public void SendTileLoading (int number, string text)
		{
			Begin (Packet.SEND_TILE_LOADING);
			
			Int (number);
			String (text);
			
			End ();
		}
		
		private void Tile (int x, int y)
		{
			byte flags = 0;
			
			var tile   = Main.tile[x, y];
			var active = tile.Active;
			var wall   = tile.wall;
			var liquid = tile.liquid;
			
			if (active)          flags += 1;
			if (tile.lighted)    flags += 2;
			if (wall > 0)        flags += 4;
			if (liquid > 0)      flags += 8;
			
			Byte (flags);
			
			if (active)
			{
				var type = tile.type;
				
				Byte (type);
				
				if (Main.tileFrameImportant [type])
				{
					Short (tile.frameX);
					Short (tile.frameY);
				}
			}
			
			if (wall > 0)
			{
				Byte (wall);
			}
			
			if (liquid > 0)
			{
				Byte (liquid);
				Byte (tile.lava);
			}
		}
		
		public void SendTileRow (int numColumns, int firstColumn, int row)
		{
			Begin (Packet.SEND_TILE_ROW);
			
			Short (numColumns);
			Int (firstColumn);
			Int (row);
			
			for (int col = firstColumn; col < firstColumn + numColumns; col++)
			{
				Tile (col, row);
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
		
		public void ReceivingPlayerJoined (Player player)
		{
			Begin (Packet.RECEIVING_PLAYER_JOINED);
			
			Byte (player.whoAmi);
			Int (player.SpawnX);
			Int (player.SpawnY);
			
			End ();
		}
		
		public void PlayerStateUpdate (Player player)
		{
			Begin (Packet.PLAYER_STATE_UPDATE);
			
			byte flags = 0;
			if (player.controlUp)      flags += 1;
			if (player.controlDown)    flags += 2;
			if (player.controlLeft)    flags += 4;
			if (player.controlRight)   flags += 8;
			if (player.controlJump)    flags += 16;
			if (player.controlUseItem) flags += 32;
			if (player.direction == 1) flags += 64;
			
			Byte (player.whoAmi);
			Byte (flags);
			Byte (player.selectedItemIndex);
			
			Float (player.Position.X);
			Float (player.Position.Y);
			Float (player.Velocity.X);
			Float (player.Velocity.Y);
			
			End ();
		}
		
		public void SynchBegin (Player player, int foo)
		{
			Header (Packet.SYNCH_BEGIN, 2);
			
			Byte (player.whoAmi);
			Byte (foo);
		}
		
		public void UpdatePlayers ()
		{
			Header (Packet.UPDATE_PLAYERS, 0);
		}
		
		public void PlayerHealthUpdate (Player player)
		{
			Header (Packet.PLAYER_HEALTH_UPDATE, 5);
			
			Byte (player.whoAmi);
			Short (player.statLife);
			Short (player.statLifeMax);
		}
		
		public void TileBreak (int tileAction, int x, int y, int tileType = 0, int number5 = 0)
		{
			Header (Packet.TILE_BREAK, 11);
			
			Byte (tileAction);
			Int (x);
			Int (y);
			Byte (tileType);
			Byte (number5);
		}
		
		public void TimeSunMoonUpdate ()
		{
			throw new NotImplementedException ("PacketWriter.TimeSunMoonUpdate()");
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
			
			String (item.Name);
			
			End ();
		}
		
		public void ItemOwnerInfo (int itemId)
		{
			var item = Main.item[itemId];
			
			Header (Packet.ITEM_OWNER_INFO, 3);
			
			Short (itemId);
			Byte (item.Owner);
		}
		
		public void NPCInfo (NPC npc)
		{
			Begin (Packet.NPC_INFO);
			
			Short (npc.whoAmI);
			
			Float (npc.Position.X);
			Float (npc.Position.Y);
			Float (npc.Velocity.X);
			Float (npc.Velocity.Y);
			
			Short (npc.target);
			
			Byte (npc.direction + 1);
			Byte (npc.directionY + 1);
			
			if (npc.Active)
				Short (npc.life);
			else
				Short (0);
			
			for (int i = 0; i < NPC.MAX_AI; i++)
				Float (npc.ai[i]);
			
			String (npc.Name);
			
			End ();
		}
		
		public void StrikeNPC (NPC npc, Player player)
		{
			Header (Packet.STRIKE_NPC, 3);
			
			Short (npc.whoAmI);
			Byte (player.whoAmi);
		}
		
		public void PlayerChat (Player player, string text, Color color)
		{
			Begin (Packet.PLAYER_CHAT);
			
			Byte (player.whoAmi);
			Byte (color.R);
			Byte (color.G);
			Byte (color.B);
			
			String (text);
			
			End ();
		}
		
		public void StrikePlayer (Player victim, string deathText, int direction, int damage, int pvpFlag)
		{
			Begin (Packet.STRIKE_PLAYER);
			
			Byte (victim.whoAmi);
			Byte (direction + 1);
			Short (damage);
			Byte (pvpFlag);
			
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
		
		public void DamageNPC (NPC npc, int damage, float knockback, int direction)
		{
			Header (Packet.DAMAGE_NPC, 9);
			
			Short (npc.whoAmI);
			Short (damage);
			
			Float (knockback);
			
			Byte (direction + 1);
		}
		
		public void KillProjectile (Projectile proj)
		{
			Header (Packet.KILL_PROJECTILE, 3);
			
			Short (proj.identity);
			Byte (proj.Owner);
		}
		
		public void PlayerPVPChange (Player player)
		{
			Header (Packet.PLAYER_PVP_CHANGE, 2);
			
			Byte (player.whoAmi);
			Byte (player.hostile);
		}
		
		public void OpenChest ()
		{
			throw new NotImplementedException ("PacketWriter.OpenChest()");
		}
		
		public void ChestItem (int chestId, int itemId)
		{
			var chest = Main.chest[chestId];
			var item = chest.contents[itemId];
			
			Begin (Packet.CHEST_ITEM);
			
			Short (chestId);
			Byte (itemId);
			Byte (item.Stack);
			
			String (item.Name ?? "");
			
			End ();
		}
		
		public void PlayerChestUpdate (int chestId)
		{
			Header (Packet.PLAYER_CHEST_UPDATE, 9);
			
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
		
		public void HealPlayer (Player player, int amount)
		{
			Header (Packet.HEAL_PLAYER, 3);
			
			Byte (player.whoAmi);
			Short (amount);
		}
		
		public void EnterZone (Player player)
		{
			Header (Packet.ENTER_ZONE, 5);
			
			Byte (player.whoAmi);
			
			Byte (player.zoneEvil);
			Byte (player.zoneMeteor);
			Byte (player.zoneDungeon);
			Byte (player.zoneJungle);
		}
		
		public void PasswordRequest ()
		{
			Header (Packet.PASSWORD_REQUEST, 0);
		}
		
		public void PasswordResponse ()
		{
			throw new NotImplementedException ("PacketWriter.PasswordResponse()");
		}
		
		public void ItemOwnerUpdate (int itemId)
		{
			Header (Packet.ITEM_OWNER_UPDATE, 2);
			
			Short (itemId);
		}
		
		public void NPCTalk (Player player)
		{
			Header (Packet.NPC_TALK, 3);
			
			Byte (player.whoAmi);
			Short (player.talkNPC);
		}
		
		public void PlayerBallswing (Player player)
		{
			Header (Packet.PLAYER_BALLSWING, 7);
			
			Byte (player.whoAmi);
			Float (player.itemRotation);
			Short (player.itemAnimation);
		}
		
		public void PlayerManaUpdate (Player player)
		{
			Header (Packet.PLAYER_MANA_UPDATE, 5);
			
			Byte (player.whoAmi);
			Short (player.statMana);
			Short (player.statManaMax);
		}
		
		public void PlayerUseManaUpdate (Player player, int amount)
		{
			Header (Packet.PLAYER_USE_MANA_UPDATE, 3);
			
			Byte (player.whoAmi);
			Short (amount);
		}
		
		public void KillPlayerPVP (Player victim, string deathText, int direction, int damage, int pvpFlag)
		{
			Begin (Packet.KILL_PLAYER_PVP);
			
			Byte (victim.whoAmi);
			Byte (direction + 1);
			Short (damage);
			Byte (pvpFlag);
			
			String (deathText);
			
			End ();
		}
		
		public void PlayerJoinParty (Player player)
		{
			Header (Packet.PLAYER_JOIN_PARTY, 2);
			
			Byte (player.whoAmi);
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
			
			String (sign.text);
			
			End ();
		}
		
		public void FlowLiquid (int x, int y)
		{
			var tile = Main.tile[x, y];
			
			Header (Packet.FLOW_LIQUID, 10);
			
			Int (x);
			Int (y);
			Byte (tile.liquid);
			Byte (tile.lava);
		}
		
		public void SendSpawn ()
		{
			Header (Packet.SEND_SPAWN, 0);
		}
		
		public void PlayerBuffs (Player player)
		{
			Header (Packet.PLAYER_BUFFS, 11);
		
			Byte (player.whoAmi);
			
			for (int i = 0; i < 10; i++)
			{
				Byte (player.buffType[i]);
			}
		}
		
		public void SummonSkeletron ()
		{
			throw new NotImplementedException ("PacketWriter.SummonSkeletron()");
		}
	}
}
