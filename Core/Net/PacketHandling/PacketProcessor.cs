using System;
using OTA.Plugin;
using OTA;
using System.Linq;
using TDSM.Core.Net.PacketHandling.Misc;

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
                .SelectMany(clazz => clazz.GetTypesLoaded())
                .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                var handler = (IPacketHandler)Activator.CreateInstance(messageType);
                handlers[(int)handler.PacketId] = handler;
            }

            return handlers;
        }

        [TDSMComponent(ComponentEvent.Initialise)]
        internal static void Initialise(Entry plugin)
        {
            plugin.Hook(HookPoints.ReceiveNetMessage, HandlePacket);
            plugin.Hook(HookPoints.CheckBufferState, CheckState);
        }

        /// <summary>
        /// Handles the connection state when receiving a packet
        /// </summary>
        public static void CheckState(ref HookContext ctx, ref HookArgs.CheckBufferState args)
        {
            if (Terraria.Netplay.Clients[args.BufferId].State == (int)ConnectionState.WaitingForUserPassword)
            {
                //Since this is a custom state, we accept it [true to kick the connection, false to accept]
                ctx.SetResult(HookResult.RECTIFY, true, false /* TODO validate packets */);
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
                    //Packet informed us that it was read, let OTA know we consumed the packet
                    ctx.SetResult(HookResult.IGNORE, true);
                }
            }
        }
    }
}

