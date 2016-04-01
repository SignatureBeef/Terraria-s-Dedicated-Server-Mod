using System;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct NpcHurtReceived
        {
            public Terraria.NPC Victim { get; set; }

            public int Damage { get; set; }

            public int HitDirection { get; set; }

            public float Knockback { get; set; }

            public bool Critical { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.NpcHurtReceived> NpcHurtReceived = new HookPoint<TDSMHookArgs.NpcHurtReceived>();
    }
}