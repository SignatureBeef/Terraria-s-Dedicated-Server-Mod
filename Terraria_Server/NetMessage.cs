using System;
using System.Text;
using System.IO;

using Terraria_Server.Commands;
using Terraria_Server.Events;
using Terraria_Server.Messages;
using Terraria_Server.Misc;

namespace Terraria_Server
{
	public partial class NetMessage
	{
		public static NetMessage PrepareThreadInstance()
		{
			if (threadInstance == null)
			{
				threadInstance = new NetMessage (65535);
			}
			else
			{
				threadInstance.sink.Position = 0;
				threadInstance.lenAt = 0;
			}
			return threadInstance;
		}
		
		public byte[] Output
		{
			get
			{
				var copy = new byte [sink.Position];
				Array.Copy (buf, copy, sink.Position);
				return copy;
			}
		}
		
		public NetMessage (int bufSize = 65535)
		{
			buf = new byte [bufSize];
			sink = new SealedMemoryStream (buf);
			bin = new SealedBinaryWriter (sink);
		}
		
		public void Clear ()
		{
			lenAt = 0;
			sink.Position = 0;
		}
		public static MessageBuffer[] buffer = new MessageBuffer[257];

		public static void BootPlayer(int plr, String msg)
		{
			Netplay.slots[plr].Kick (msg);
		}
		
		public static int SendData (int packetId, int remoteClient = -1, int ignoreClient = -1, String text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			try
			{
				var msg = PrepareThreadInstance();
	
				switch (packetId)
				{
					case (int)Packet.CONNECTION_REQUEST:
						msg.ConnectionRequest (Statics.CURRENT_TERRARIA_RELEASE_STR);
						break;
	
					case (int)Packet.DISCONNECT:
						msg.Disconnect (text);
						break;
						
					case (int)Packet.CONNECTION_RESPONSE:
						msg.ConnectionResponse (remoteClient);
						break;
	
					case (int)Packet.PLAYER_DATA:
						msg.PlayerData (number);
						break;
						
					case (int)Packet.INVENTORY_DATA:
						msg.InventoryData (number, (byte)number2, text);
						break;
						
					case (int)Packet.WORLD_REQUEST:
						msg.WorldRequest ();
						break;
						
					case (int)Packet.WORLD_DATA:
						msg.WorldData ();
						break;
						
					case (int)Packet.REQUEST_TILE_BLOCK:
						msg.RequestTileBlock ();
						break;
						
					case (int)Packet.SEND_TILE_LOADING:
						msg.SendTileLoading (number, text);
						break;
						
					case (int)Packet.SEND_TILE_ROW:
						msg.SendTileRow (number, (int)number2, (int)number3);
						break;
						
					case (int)Packet.SEND_TILE_CONFIRM:
						msg.SendTileConfirm (number, (int)number2, (int)number3, (int)number4);
						break;
						
					case (int)Packet.RECEIVING_PLAYER_JOINED:
						msg.ReceivingPlayerJoined (number);
						break;
						
					case (int)Packet.PLAYER_STATE_UPDATE:
						msg.PlayerStateUpdate (number);
						break;
						
					case (int)Packet.SYNCH_BEGIN:
						msg.SynchBegin (number, (int)number2);
						break;
						
					case (int)Packet.UPDATE_PLAYERS:
						msg.UpdatePlayers ();
						break;
						
					case (int)Packet.PLAYER_HEALTH_UPDATE:
						msg.PlayerHealthUpdate (number);
						break;
						
					case (int)Packet.TILE_BREAK:
						msg.TileBreak (number, (int)number2, (int)number3, (int)number4, (int)number5);
						break;
						
					case (int)Packet.TIME_SUN_MOON_UPDATE:
						msg.TimeSunMoonUpdate ();
						break;
						
					case (int)Packet.DOOR_UPDATE:
						msg.DoorUpdate (number, (int)number2, (int)number3, (int)number4);
						break;
						
					case (int)Packet.TILE_SQUARE:
						msg.TileSquare (number, (int)number2, (int)number3);
						break;
						
					case (int)Packet.ITEM_INFO:
						msg.ItemInfo (number);
						break;
						
					case (int)Packet.ITEM_OWNER_INFO:
						msg.ItemOwnerInfo (number);
						break;
						
					case (int)Packet.NPC_INFO:
						msg.NPCInfo (number);
						break;
						
					case (int)Packet.STRIKE_NPC:
						msg.StrikeNPC (number, (int)number2);
						break;
						
					case (int)Packet.PLAYER_CHAT:
						msg.PlayerChat (number, text, (byte)number2, (byte)number3, (byte)number4);
						break;
						
					case (int)Packet.STRIKE_PLAYER:
						msg.StrikePlayer (number, text, (int)number2, (int)number3, (int)number4);
						break;
						
					case (int)Packet.PROJECTILE:
						msg.Projectile (Main.projectile[number]);
						break;
	
					case (int)Packet.DAMAGE_NPC:
						msg.DamageNPC (number, (int)number2, number3, (int)number4);
						break;
	
					case (int)Packet.KILL_PROJECTILE:
						msg.KillProjectile (Main.projectile[number]);
						break;
						
					case (int)Packet.PLAYER_PVP_CHANGE:
						msg.PlayerPVPChange (number);
						break;
	
					case (int)Packet.OPEN_CHEST:
						msg.OpenChest ();
						break;
						
					case (int)Packet.CHEST_ITEM:
						msg.ChestItem (number, (int)number2);
						break;
						
					case (int)Packet.PLAYER_CHEST_UPDATE:
						msg.PlayerChestUpdate (number);
						break;
	
					case (int)Packet.KILL_TILE:
						msg.KillTile ();
						break;
						
					case (int)Packet.HEAL_PLAYER:
						msg.HealPlayer (number, (int)number2);
						break;
						
					case (int)Packet.ENTER_ZONE:
						msg.EnterZone (number);
						break;
						
					case (int)Packet.PASSWORD_REQUEST:
						msg.PasswordRequest ();
						break;
						
					case (int)Packet.PASSWORD_RESPONSE:
						msg.PasswordResponse ();
						break;
						
					case (int)Packet.ITEM_OWNER_UPDATE:
						msg.ItemOwnerUpdate (number);
						break;
						
					case (int)Packet.NPC_TALK:
						msg.NPCTalk (number);
						break;
	
					case (int)Packet.PLAYER_BALLSWING:
						msg.PlayerBallswing (number);
						break;
						
					case (int)Packet.PLAYER_MANA_UPDATE:
						msg.PlayerManaUpdate (number);
						break;
						
					case (int)Packet.PLAYER_USE_MANA_UPDATE:
						msg.PlayerUseManaUpdate (number, (int)number2);
						break;
						
					case (int)Packet.KILL_PLAYER_PVP:
						msg.KillPlayerPVP (number, text, (int)number2, (int)number3, (int)number4);
						break;
						
					case (int)Packet.PLAYER_JOIN_PARTY:
						msg.PlayerJoinParty (number);
						break;
						
					case (int)Packet.READ_SIGN:
						msg.ReadSign (number, (int)number2);
						break;
						
					case (int)Packet.WRITE_SIGN:
						msg.WriteSign (number);
						break;
						
					case (int)Packet.FLOW_LIQUID:
						msg.FlowLiquid (number, (int)number2);
						break;
						
					case (int)Packet.SEND_SPAWN:
						msg.SendSpawn ();
						break;
						
					case (int)Packet.PLAYER_BUFFS:
						msg.PlayerBuffs (number);
						break;
						
					case (int)Packet.SUMMON_SKELETRON:
						msg.SummonSkeletron ();
						break;
						
					default:
						{
							//Unknown packet :3
							return 0;
						}
				}
					
				var bytes = msg.Output;
				if (remoteClient == -1)
				{
					for (int num11 = 0; num11 < 256; num11++)
					{
						if (num11 != ignoreClient && Netplay.slots[num11].state >= SlotState.PLAYING && Netplay.slots[num11].Connected)
						{
							NetMessage.buffer[num11].spamCount++;
							Netplay.slots[num11].Send (bytes);
						}
					}
					
				}
				else if (Netplay.slots[remoteClient].Connected)
				{
					NetMessage.buffer[remoteClient].spamCount++;
					Netplay.slots[remoteClient].Send (bytes);
				}
				return bytes.Length;
			}
			catch (Exception e)
			{
				Program.tConsole.WriteLine("Issue sending Data - NetMessage Error!");
				Program.tConsole.WriteLine(e.Message);
				Program.tConsole.WriteLine(e.StackTrace);
			}
			return 0;
		}
		
		public static void CheckBytes(int i = 256)
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
						if (!Main.ignoreErrors)
						{
							NetMessage.buffer[i].GetData(num + 4, NetMessage.buffer[i].messageLength - 4);
						}
						else
						{
							try
							{
								NetMessage.buffer[i].GetData(num + 4, NetMessage.buffer[i].messageLength - 4);
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
		
		public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size)
		{
			int num = (size - 1) / 2;
			float x = tileX - num;
			float y = tileY - num;
			NetMessage.SendData(20, whoAmi, -1, "", size, x, y, 0f);
		}

		public static void SendSection(int whoAmi, int sectionX, int sectionY)
		{
			try
			{
				var msg = NetMessage.PrepareThreadInstance();

				if (sectionX >= 0 && sectionY >= 0 && sectionX < Main.maxSectionsX && sectionY < Main.maxSectionsY)
				{
					Netplay.slots[whoAmi].tileSection[sectionX, sectionY] = true;
					int num = sectionX * 200;
					int num2 = sectionY * 150;
					for (int i = num2; i < num2 + 150; i++)
					{
						//NetMessage.SendData(10, whoAmi, -1, "", 200, (float)num, (float)i, 0f);
						msg.Clear ();
						msg.SendTileRow (200, num, i);
						Netplay.slots[whoAmi].Send (msg.Output);
					}
					
					//Console.WriteLine ("SendSection: {0} bytes", ts.stream.Position);
					//Netplay.slots[whoAmi].Send (ts.buffer, 0, (int)ts.stream.Position);
				}
				
			}
			catch
			{
			}
		}
		
		public static void GreetPlayer (int plr)
		{
			String[] motd = Program.properties.Greeting.Split('@');
			for (int i = 0; i < motd.Length; i++)
			{
				if (motd != null && motd.Length > 0)
				{
					if (motd[i] != null && motd[i].Trim().Length > 0)
					{
						NetMessage.SendData(((int)Packet.PLAYER_CHAT), plr, -1, motd[i], 255, 0f, 0f, 255f);
					}
				}
			}

			String text = "";
			for (int i = 0; i < 255; i++)
			{
				if (Main.players[i].Active)
				{
					if (text == "")
					{
						text += Main.players[i].Name;
					}
					else
					{
						text = text + ", " + Main.players[i].Name;
					}
				}
			}
			NetMessage.SendData(25, plr, -1, "Current players: " + text + ".", 255, 255f, 240f, 20f);
		}
		
		public static void SendWater(int x, int y)
		{
			var msg = NetMessage.PrepareThreadInstance();
			msg.FlowLiquid (x, y);
			var bytes = msg.Output;
			
			for (int i = 0; i < 256; i++)
			{
				if (Netplay.slots[i].state >= SlotState.PLAYING && Netplay.slots[i].Connected)
				{
					int num = x / 200;
					int num2 = y / 150;
					if (Netplay.slots[i].tileSection[num, num2])
					{
						Netplay.slots[i].Send (bytes);
					}
				}
			}
		}
		
		public static void SyncPlayers() /* FIXME: always sends all updates to all players */
		{
			bool flag = false;
			for (int i = 0; i < 255; i++)
			{
				int num = 0;
				if (Main.players[i].Active)
				{
					num = 1;
				}
				if (Netplay.slots[i].state == SlotState.PLAYING)
				{
					if (Main.autoShutdown && !flag)
					{
						String text = Netplay.slots[i].remoteAddress;
						String a = text;
						for (int j = 0; j < text.Length; j++)
						{
							if (text.Substring(j, 1) == ":")
							{
								a = text.Substring(0, j);
							}
						}
						if (a == "127.0.0.1")
						{
							flag = true;
						}
					}

					var msg = NetMessage.PrepareThreadInstance();
					
					msg.SynchBegin (i, num);
					msg.PlayerStateUpdate (i);
					msg.PlayerHealthUpdate (i);
					msg.PlayerPVPChange (i);
					msg.PlayerJoinParty (i);
					msg.PlayerManaUpdate (i);
					msg.PlayerBuffs (i);
					msg.PlayerData (i);
					
					for (int k = 0; k < 44; k++)
					{
						msg.InventoryData (i, k, Main.players[i].inventory[k].Name);
					}
					
					for (int k = 0; k < 11; k++)
					{
						msg.InventoryData (i, k+44, Main.players[i].armor[k].Name);
					}
					
					if (!Netplay.slots[i].announced)
					{
						msg.PlayerChat (255, Main.players[i].Name + " has joined.", 255, 240, 20);
						
						Netplay.slots[i].announced = true;
						
						if (Main.dedServ)
						{
							Program.tConsole.WriteLine(Main.players[i].Name + " has joined.");

							PlayerLoginEvent Event = new PlayerLoginEvent();
							Event.Socket = Netplay.slots[i];
							Event.Sender = Main.players[i];
							Program.server.PluginManager.processHook(Plugin.Hooks.PLAYER_LOGIN, Event);
						}
					}
					
					var bytes = msg.Output;
					for (int k = 0; k < 256; k++)
					{
						if (k != i && Netplay.slots[k].state >= SlotState.PLAYING && Netplay.slots[k].Connected)
						{
							NetMessage.buffer[k].spamCount++;
							Netplay.slots[k].Send (bytes);
						}
					}

				}
				else
				{
					NetMessage.SendData(14, -1, i, "", i, (float)num, 0f, 0f);
					if (Netplay.slots[i].announced)
					{
						Netplay.slots[i].announced = false;
						NetMessage.SendData(25, -1, i, Netplay.slots[i].oldName + " has left.", 255, 255f, 240f, 20f);
						if (Main.dedServ)
						{
							Program.tConsole.WriteLine(Netplay.slots[i].oldName + " has left.");

							PlayerLogoutEvent Event = new PlayerLogoutEvent();
							Event.Socket = Netplay.slots[i];
							Event.Sender = Main.players[i];
							Program.server.PluginManager.processHook(Plugin.Hooks.PLAYER_LOGOUT, Event);
						}
					}
				}
			}
			if (Main.autoShutdown && !flag)
			{
				Commands.Commands.SaveAll();
				Netplay.disconnect = true;
			}
		}
		
		//
		// PRIVATES
		//
		
		private readonly SealedMemoryStream sink;
		private readonly SealedBinaryWriter bin;
		private readonly byte[] buf;
		private int lenAt;
		
		[ThreadStatic]
		private static NetMessage threadInstance;

		sealed class SealedMemoryStream : System.IO.MemoryStream
		{
			public SealedMemoryStream (byte[] buf) : base(buf) {}
		}
		
		sealed class SealedBinaryWriter : System.IO.BinaryWriter
		{
			public SealedBinaryWriter (Stream stream) : base(stream, Encoding.ASCII) {}
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
			//sink.Write (BitConverter.GetBytes(data), 0, 2);
			bin.Write (data);
		}

		private void Short (int data)
		{
			Short ((short) data);
		}

		private void Int (int data)
		{
			//sink.Write (BitConverter.GetBytes(data), 0, 4);
			bin.Write (data);
		}
		
		private void Int (double data)
		{
			Int ((int) data);
		}

#if UNSAFE
		private unsafe void Float (float data)
		{
			var bytes = (byte*) &data;
			sink.WriteByte (bytes[0]);
			sink.WriteByte (bytes[1]);
			sink.WriteByte (bytes[2]);
			sink.WriteByte (bytes[3]);
		}
#else
		private void Float (float data)
		{
			sink.Write (BitConverter.GetBytes(data), 0, 4);
		}
#endif
		
		private void String (string data)
		{
			foreach (char c in data)
			{
				if (c < 128)
					sink.WriteByte ((byte) c);
				else
					sink.WriteByte ((byte) '?');
			}
		}
		
	}
}
