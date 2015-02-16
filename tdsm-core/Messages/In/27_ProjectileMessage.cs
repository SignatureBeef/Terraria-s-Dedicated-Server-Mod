using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ProjectileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PROJECTILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [ProjectileReceived]

            int num67 = (int)ReadInt16(readBuffer);
            var position3 = ReadVector2(readBuffer);
            var velocity3 = ReadVector2(readBuffer);
            float knockBack = ReadSingle(readBuffer);
            int damage = (int)ReadInt16(readBuffer);
            int num68 = (int)ReadByte(readBuffer);
            int num69 = (int)ReadInt16(readBuffer);
            BitsByte bitsByte10 = ReadByte(readBuffer);
            float[] array2 = new float[Projectile.maxAI];
            for (int num70 = 0; num70 < Projectile.maxAI; num70++)
            {
                if (bitsByte10[num70])
                {
                    array2[num70] = ReadSingle(readBuffer);
                }
                else
                {
                    array2[num70] = 0f;
                }
            }
            if (Main.netMode == 2)
            {
                num68 = whoAmI;
                if (Main.projHostile[num69])
                {
                    return;
                }
            }
            int num71 = 1000;
            for (int num72 = 0; num72 < 1000; num72++)
            {
                if (Main.projectile[num72].owner == num68 && Main.projectile[num72].identity == num67 && Main.projectile[num72].active)
                {
                    num71 = num72;
                    break;
                }
            }
            if (num71 == 1000)
            {
                for (int num73 = 0; num73 < 1000; num73++)
                {
                    if (!Main.projectile[num73].active)
                    {
                        num71 = num73;
                        break;
                    }
                }
            }
            Projectile projectile = Main.projectile[num71];
            if (!projectile.active || projectile.type != num69)
            {
                projectile.SetDefaults(num69);
                if (Main.netMode == 2)
                {
                    tdsm.api.Callbacks.Netplay.slots[whoAmI].spamProjectile += 1f;
                }
            }
            projectile.identity = num67;
            projectile.position = position3;
            projectile.velocity = velocity3;
            projectile.type = num69;
            projectile.damage = damage;
            projectile.knockBack = knockBack;
            projectile.owner = num68;
            for (int num74 = 0; num74 < Projectile.maxAI; num74++)
            {
                projectile.ai[num74] = array2[num74];
            }
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(27, -1, whoAmI, String.Empty, num71, 0f, 0f, 0f, 0);
                return;
            }
            return;
        }
    }
}