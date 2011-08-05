using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Definitions;

namespace Terraria_Server.Events
{
    public class TimeChangedEvent : Event
    {
        public Time GetCycle
        {
            get
            {
                if (Main.dayTime == true)
                {
                    if (Main.time >= 13500)
                    {
                        return Time.DAWN;
                    }
                    else
                    {
                        return Time.DUSK;
                    }
                }
                else
                {
                    if (Main.time >= 16200)
                    {
                        return Time.NIGHT;
                    }
                    else
                    {
                        return Time.DUSK;
                    }
                }
            }
        }

        public Double GetTime
        {
            get
            {
                return Main.time;
            }
        }
    }
}
