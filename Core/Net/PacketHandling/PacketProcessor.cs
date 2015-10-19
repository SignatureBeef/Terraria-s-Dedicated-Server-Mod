using System;
using OTA.Plugin;
using OTA;
using System.Linq;

namespace TDSM.Core.Net.PacketHandling
{
    public static class PacketProcessor
    {
        /// <summary>
        /// TDSM implemented packet hooks
        /// </summary>
        private static readonly IPacketHandler[] _packetHandlers = GetHandlers();

        /// <summary>
        /// Loads all packet handlers in the code base.
        /// </summary>
        private static IPacketHandler[] GetHandlers()
        {
            var max = ((Packet[])Enum.GetValues(typeof(Packet))).Select(x => (byte)x).Max();
            var handlers = new IPacketHandler[max];

            //Load all instances of IPacketHandler
            var type = typeof(IPacketHandler);
            foreach (var messageType in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(clazz => clazz.GetTypes())
                .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                var handler = (IPacketHandler)Activator.CreateInstance(messageType);
                handlers[(int)handler.PacketId] = handler;
            }

            return handlers;
        }

        public static void Initialise(BasePlugin plugin)
        {
            plugin.Hook(HookPoints.ReceiveNetMessage, HandlePacket);
            plugin.Hook(HookPoints.CheckBufferState, CheckState);
        }

        /// <summary>
        /// Handles packets received from OTA
        /// </summary>
        public static void CheckState(ref HookContext ctx, ref HookArgs.CheckBufferState args)
        {
            if (Terraria.Netplay.Clients[args.BufferId].State == -2)
            {
                ctx.SetResult(HookResult.RECTIFY, true, false);
            }
        }

        /// <summary>
        /// Handles packets received from OTA
        /// </summary>
        public static void HandlePacket(ref HookContext ctx, ref HookArgs.ReceiveNetMessage args)
        {
            if (_packetHandlers[args.PacketId] != null)
            {
                if (_packetHandlers[args.PacketId].Read(args.BufferId, args.Start, args.Length))
                {
                    //Consume the packet
                    ctx.SetResult(HookResult.IGNORE, true);
                }
            }
        }
    }
}

