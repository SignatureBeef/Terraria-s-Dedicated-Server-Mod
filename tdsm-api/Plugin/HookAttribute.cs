using System;

namespace TDSM.API.Plugin
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