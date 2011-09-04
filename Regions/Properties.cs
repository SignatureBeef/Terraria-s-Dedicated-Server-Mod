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
            object temp = SelectionToolID;
        }

        /*public List<String> BlockedProjectiles
        {
            get
            {
                //return getValue("blockedprojectiles", "28,29,37").Split(',').ToList<String>();
                return getValue("blockedprojectiles", "*").Split(',').ToList<String>();
            }
            set
            {
                setValue("blockedprojectiles", string.Join(",", value.ToArray()));
            }
        }*/

        public Int32 SelectionToolID
        {
            get
            {
                return getValue("selectionblockid", 0);
            }
            set
            {
                setValue("selectionblockid", value);
            }
        }
    }
}
