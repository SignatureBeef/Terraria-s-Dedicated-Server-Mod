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
    }
}
