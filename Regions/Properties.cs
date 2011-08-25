using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server;
using System.IO;

namespace Regions
{
    public class Properties : PropertiesFile
    {
        public Properties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = null;
        }        
    }
}
