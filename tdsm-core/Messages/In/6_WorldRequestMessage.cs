using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class WorldRequestMessage : MessageHandler
    {
        public WorldRequestMessage()
        {
            IgnoredStates = SlotState.PLAYER_AUTH;
            ValidStates = SlotState.ACCEPTED | SlotState.ASSIGNING_SLOT;
        }

        public override Packet GetPacket()
        {
            return Packet.WORLD_REQUEST;
        }

        public override void Process(ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            if (conn.State == SlotState.ACCEPTED)
            {
                SlotManager.Schedule(conn, conn.DesiredQueue);
                return;
            }

            int whoAmI = conn.SlotIndex;
            if (Terraria.Netplay.Clients[whoAmI].State() == SlotState.ASSIGNING_SLOT)
            {
                Terraria.Netplay.Clients[whoAmI].SetState(SlotState.SENDING_WORLD);
            }

            var ctx = new HookContext() { Connection = conn, Player = conn.Player };
            var args = new HookArgs.WorldRequestMessage()
            {
                SpawnX = Main.spawnTileX,
                SpawnY = Main.spawnTileY
            };
            HookPoints.WorldRequestMessage.Invoke(ref ctx, ref args);

            //NewNetMessage.SendData(7, whoAmI);
            var msg = NewNetMessage.PrepareThreadInstance();
            msg.WorldData(args.SpawnX, args.SpawnY);
            msg.Send(whoAmI);
        }
    }
}
