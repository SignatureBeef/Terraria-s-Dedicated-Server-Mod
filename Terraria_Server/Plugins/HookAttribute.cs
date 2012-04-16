using System;

namespace Terraria_Server.Plugins
{
	public class HookAttribute : Attribute
	{
		internal readonly HookOrder order;
		
		public HookAttribute (HookOrder order = HookOrder.NORMAL)
		{
			this.order = order;
		}
	}
}

