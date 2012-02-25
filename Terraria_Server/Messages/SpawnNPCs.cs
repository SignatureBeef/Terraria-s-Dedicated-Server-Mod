using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Definitions;
using Terraria_Server.Misc;

namespace Terraria_Server.Messages
{
	public struct SentMessage
	{
	    public DateTime Time { get; set; }
		public int Type { get; set; }
	}

	public class SpawnNPCs : SpammableMessage<Int32, SentMessage>
	{
		public const Int32 MIN_WAIT = 10;

		public override Packet GetPacket()
		{
			return Packet.SPAWN_NPCS;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			int plr = BitConverter.ToInt32(readBuffer, num);
			num += 4;
			int typeOrInvasion = BitConverter.ToInt32(readBuffer, num);

			var player = Main.players[whoAmI];

			if (plr != whoAmI)
			{
				player.Kick("SpawnNPC Player Forgery!");
				return;
			}
			
			SentMessage last;
			if (Register.TryGetValue(plr, out last) && last.Type == typeOrInvasion)
			{
				if ((DateTime.Now - last.Time).TotalSeconds >= MIN_WAIT)
				{
					player.Kick("SpawnNPC packet spam!");
					return;
				}
			}

			Purge();
			AddOrUpdate(plr, new SentMessage()
			{
				Time = DateTime.Now,
				Type = typeOrInvasion
			});

			if (typeOrInvasion == (int)NPCType.N04_EYE_OF_CTHULHU ||
				typeOrInvasion == (int)NPCType.N13_EATER_OF_WORLDS_HEAD ||
				typeOrInvasion == (int)NPCType.N50_KING_SLIME ||
				typeOrInvasion == (int)NPCType.N125_RETINAZER ||
				typeOrInvasion == (int)NPCType.N126_SPAZMATISM ||
				typeOrInvasion == (int)NPCType.N134_THE_DESTROYER ||
				typeOrInvasion == (int)NPCType.N127_SKELETRON_PRIME ||
				typeOrInvasion == (int)NPCType.N128_PRIME_CANNON)
			{
				if (!NPC.IsNPCSummoned(typeOrInvasion))
					NPC.SpawnOnPlayer(plr, typeOrInvasion);
			}
			else
			{
				if (typeOrInvasion >= 0)
				{
					player.Kick("Attempt to summon an unsupported NPC.");
					return;
				}

				int invasionType = typeOrInvasion;

				if (typeOrInvasion == -1 || typeOrInvasion == -2)
					invasionType *= -1;
				else
				{
					player.Kick("Attempt to invoke an unknown invasion.");
					return;
				}

				if (Main.invasionType == 0)
				{
					if (invasionType > 0)
					{
						Main.invasionDelay = 0;
						Main.StartInvasion((InvasionType)invasionType);
					}
				}
				else
					player.sendMessage("Please wait until the current invasion has been defeated.", ChatColor.Purple);
			}
		}

		/// <summary>
		/// Clears data when over time
		/// </summary>
		public void Purge()
		{
			var removable = from x in Register where (DateTime.Now - x.Value.Time).TotalSeconds > MIN_WAIT select x.Key;

			lock (Register)
			{
				foreach (var id in removable)
					Register.Remove(id);
			}
		}
	}
}
