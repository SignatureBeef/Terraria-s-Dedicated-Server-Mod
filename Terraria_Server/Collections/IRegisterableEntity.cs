using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Collections
{
    public interface IRegisterableEntity : ICloneable
    {
        int Type { get; set; }
        String Name { get; set; }
        bool Active { get; set; }
        int aiStyle { get; set; }
        int damage { get; set; }
        int defense { get; set; }
        int lifeMax { get; set; }
        int life { get; set; }
        float scale { get; set; }
        float knockBackResist { get; set; }
    }
}
