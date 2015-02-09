using System;
using Terraria_Server.Commands;

namespace Terraria_Server
{
    public class WorldSender : Sender
    {
        public WorldSender()
        {
            Op = true;
            Name = "WORLD";
        }

        public override void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
        }
    }
}

