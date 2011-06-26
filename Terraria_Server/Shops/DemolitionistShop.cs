
namespace Terraria_Server.Shops
{
    public class DemolitionistShop : Shop
    {
        protected override void Setup()
        {
            int i = 0;
            contents[i++].SetDefaults("Grenade");
            contents[i++].SetDefaults("Bomb");
            contents[i].SetDefaults("Dynamite");
        }
    }
}
