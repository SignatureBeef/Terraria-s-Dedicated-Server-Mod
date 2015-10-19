using System;

namespace TDSM.Core.Net.PacketHandling
{
    /// <summary>
    /// This is used as the bare implementation of a TDSM packet
    /// </summary>
    public interface IPacketHandler
    {
        /// <summary>
        /// Gets the PacketId to be read or written
        /// </summary>
        /// <value>The packet identifier.</value>
        OTA.Packet PacketId { get; }

        /// <summary>
        /// Reads the packet from the buffer
        /// </summary>
        /// <param name="bufferId">Buffer identifier.</param>
        /// <param name="start">Start.</param>
        /// <param name="length">Length.</param>
        /// <returns>True if the packet was consumed, false if other services/vanilla should consume it.</returns>
        bool Read(int bufferId, int start, int length);
    }
}

