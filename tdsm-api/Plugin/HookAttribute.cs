using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tdsm.api.Plugin
{
    public class HookAttribute : Attribute
    {
        internal readonly HookOrder order;

        public HookAttribute(HookOrder order = HookOrder.NORMAL)
        {
            this.order = order;
        }
    }
}