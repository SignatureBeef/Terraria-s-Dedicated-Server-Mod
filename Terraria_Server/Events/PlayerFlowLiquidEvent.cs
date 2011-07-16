using System;

using Terraria_Server.Misc;

namespace Terraria_Server.Events
{
	public class PlayerFlowLiquidEvent : BasePlayerEvent
	{
		public Vector2 Position { get; set; }
		
		public int Liquid { get; set; }
		
		public bool Lava { get; set; }
		
		public PlayerFlowLiquidEvent ()
		{
		}
	}
}

