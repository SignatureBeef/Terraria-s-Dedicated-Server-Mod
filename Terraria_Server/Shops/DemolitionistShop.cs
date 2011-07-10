
using Terraria_Server.Collections;
namespace Terraria_Server.Shops
{
    public class DemolitionistShop : Shop
    {
        protected override void Setup()
        {
            int i = 0;
            contents[i++] = Registries.Item.Create(168);
            contents[i++] = Registries.Item.Create(166);
            contents[i] = Registries.Item.Create(167);
        }
    }
}
