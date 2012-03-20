using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Logging;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
	public class KillProjectileMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.KILL_PROJECTILE;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			short identity = BitConverter.ToInt16(readBuffer, num);
			num += 2;
			byte owner = readBuffer[num];

			owner = (byte)whoAmI;

			int index = Projectile.FindExisting(identity, owner);
			if (index == 1000) return;

			var player = Main.players[whoAmI];

			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};

			var args = new HookArgs.KillProjectileReceived
			{
				Id = identity,
				Owner = owner,
				Index = (short)index,
			};

			HookPoints.KillProjectileReceived.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick())
			{
				return;
			}

			if (ctx.Result == HookResult.IGNORE)
			{
				return;
			}

			if (ctx.Result == HookResult.RECTIFY)
			{
				var msg = NetMessage.PrepareThreadInstance();
				msg.Projectile(Main.projectile[index]);
				msg.Send(whoAmI);
				return;
			}

			var projectile = Main.projectile[index];

			if (projectile.Owner == owner && projectile.identity == identity)
			{
				projectile.Kill(null, null);
				projectile.Reset();
				NetMessage.SendData(29, -1, whoAmI, "", (int)identity, (float)owner);
			}
		}
	}
}