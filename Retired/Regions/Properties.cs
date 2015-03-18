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
        public Properties(string propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = SelectionToolID;
            temp = RectifyChanges;
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
                setValue("blockedprojectiles", String.Join(",", value.ToArray()));
            }
        }*/

        public int SelectionToolID
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

        public bool RectifyChanges
        {
            get
            {
                return getValue("rectify", true);
            }
            set
            {
                setValue("rectify", value);
            }
        }
    }
}
