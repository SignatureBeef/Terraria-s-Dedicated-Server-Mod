using System;
using Terraria_Server.Misc;

namespace Terraria_Server.Events
{
	public struct PlayerStateUpdateData
	{
		byte flags;
		
		public byte SelectedItemIndex { get; set; }
		public Vector2 position;
		public Vector2 velocity;
		
		public bool ControlUp
		{
			get { return (flags & 1) != 0; }
			set { SetFlag (1, value); }
		}
		
		public bool ControlDown
		{
			get { return (flags & 2) != 0; }
			set { SetFlag (2, value); }
		}
		
		public bool ControlLeft
		{
			get { return (flags & 4) != 0; }
			set { SetFlag (4, value); }
		}
		
		public bool ControlRight
		{
			get { return (flags & 8) != 0; }
			set { SetFlag (8, value); }
		}
		
		public bool ControlJump
		{
			get { return (flags & 16) != 0; }
			set { SetFlag (16, value); }
		}
		
		public bool ControlUseItem
		{
			get { return (flags & 32) != 0; }
			set { SetFlag (32, value); }
		}
		
		public int Direction
		{
			get { return ((flags & 64) != 0) ? 1 : -1; }
			set { SetFlag (64, value == 1); }
		}
		
		internal void SetFlag (byte f, bool value)
		{
			if (value)
				flags |= f;
			else
				flags &= (byte) ~f;
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
			player.Position = position;
			player.Velocity = velocity;
		}
		
		public void Parse (byte[] buf, int at)
		{
			flags = buf[at++];
			SelectedItemIndex = buf[at++];
			
			position.X = BitConverter.ToSingle (buf, at); at += 4;
			position.Y = BitConverter.ToSingle (buf, at); at += 4;
			velocity.X = BitConverter.ToSingle (buf, at); at += 4;
			velocity.Y = BitConverter.ToSingle (buf, at);
		}
	}
	
    public class PlayerStateUpdateEvent : BasePlayerEvent
    {
        public byte State { get; set; }
        public PlayerStateUpdateData data;
    }
}
