using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Commands
{
    public interface ISender
    {
        String Name { get; }
        bool Op { get; set; }
        void sendMessage(String Message, int A = 255, float R = 255f, float G = 0f, float B = 0f);
    }
}
