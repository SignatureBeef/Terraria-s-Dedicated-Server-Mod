using Terraria_Server.Collections;

namespace Terraria_Server.Shops
{
    public class ClothierShop : Shop
    {
        //Type 5
        protected override void Setup()
        {
            int i = 0;
            contents[i++] = Registries.Item.Create(254);
            if (Main.dayTime)
            {
                contents[i++] = Registries.Item.Create(242);
                
            }
            if (Main.moonPhase == 0)
            {
                contents[i++] = Registries.Item.Create(245);
                contents[i++] = Registries.Item.Create(246);
            }
            else
            {
                if (Main.moonPhase == 1)
                {
                    contents[i++] = Registries.Item.Create(325);
                    contents[i++] = Registries.Item.Create(326);
                }
            }
            contents[i++] = Registries.Item.Create(269);
            contents[i++] = Registries.Item.Create(270);
            contents[i++] = Registries.Item.Create(271);
            if (Main.bloodMoon)
            {
                contents[i++] = Registries.Item.Create(322);
            }
        }
    }
}
