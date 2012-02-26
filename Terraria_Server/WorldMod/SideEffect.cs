using System;
using System.Collections.Generic;
using Terraria_Server.Plugins;
using Terraria_Server.Logging;

namespace Terraria_Server.WorldMod
{
	public enum SideEffectType
	{
		ADD_WATER,
		SMASH_SHADOW_ORB,
		NEW_ITEM,
		KILL_SIGN,
		DESTROY_CHEST,
		FALLING_BLOCK_PROJECTILE,
	}
	
	public class SideEffect
	{
		public SideEffectType EffectType { get; internal set; }
		public bool Cancel { get; set; }
		
		public int X { get; set; }
		public int Y { get; set; }
		public int Type { get; set; }
		internal int Width { get; set; }
		internal int Height { get; set; }
		public int Stack { get; set; }
		public int Prefix { get; set; }
		public int NetId { get; set; }
		internal bool NoBroadcast { get; set; }
		
		internal void Reset ()
		{
			Cancel = false;
		}
	}
}
