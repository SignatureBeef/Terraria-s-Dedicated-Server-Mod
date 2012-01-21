using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Collections
{
    public interface IRegisterableEntity : ICloneable
    {
        int Type { get; set; }
        int NetID { get; set; }
        string Name { get; set; }
        bool Active { get; set; }
    }
}
