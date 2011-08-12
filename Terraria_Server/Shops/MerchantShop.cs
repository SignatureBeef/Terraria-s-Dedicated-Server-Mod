using Terraria_Server.Collections;

namespace Terraria_Server.Shops
{
    public class MerchantShop : Shop
    {
        //Type 1
        protected override void Setup()
        {
            int i = 0;
            contents[i++] = Registries.Item.Create(88);
            contents[i++] = Registries.Item.Create(87);
            contents[i++] = Registries.Item.Create(35);
            contents[i++] = Registries.Item.Create("Copper Pickaxe");
            contents[i++] = Registries.Item.Create("Copper Axe");
            contents[i++] = Registries.Item.Create(8);
            contents[i++] = Registries.Item.Create(28);

            if (Main.players[Main.myPlayer].statManaMax == 200)
            {
                contents[i++] = Registries.Item.Create(110);
            }
            contents[i++] = Registries.Item.Create(40);
            contents[i++] = Registries.Item.Create(42);
            if (Main.bloodMoon)
            {
                contents[i++] = Registries.Item.Create(279);
            }
            if (!Main.dayTime)
            {
                contents[i++] = Registries.Item.Create(282);
            }
			if (NPC.downedBoss3)
			{
				contents[i++] = Registries.Item.Create(346);
			}
        }
    }
}
