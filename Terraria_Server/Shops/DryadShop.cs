
using Terraria_Server.Collections;
namespace Terraria_Server.Shops
{
    public class DryadShop : Shop
    {
        //Type 3
        protected override void Setup()
        {
            int i = 0;
            if (Main.bloodMoon)
            {
                contents[i++] = Registries.Item.Create(67);
                contents[i++] = Registries.Item.Create(59);
            }
            else
            {
                contents[i++] = Registries.Item.Create(66);
                contents[i++] = Registries.Item.Create(62);
                contents[i++] = Registries.Item.Create(63);
            }
            contents[i++] = Registries.Item.Create(27);
            contents[i] = Registries.Item.Create(114);
        }
    }
}
