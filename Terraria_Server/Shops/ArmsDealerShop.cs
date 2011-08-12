
using Terraria_Server.Collections;
namespace Terraria_Server.Shops
{
    public class ArmsDealerShop : Shop
    {
        //Type 2
        protected override void Setup()
        {
            int i = 0;
            contents[i++] = Registries.Item.Create(97);
            if (Main.bloodMoon)
            {
                contents[i++] = Registries.Item.Create(278);
            }
            if (NPC.downedBoss2 && !Main.dayTime)
            {
                contents[i++] = Registries.Item.Create(47);
            }
            contents[i++] = Registries.Item.Create(95);
            contents[i++] = Registries.Item.Create(98);
            if (Main.moonPhase == 4)
            {
                contents[i] = Registries.Item.Create(324);
            }
        }
    }
}
