
namespace Terraria_Server.Shops
{
    public class DryadShop : Shop
    {
        protected override void Setup()
        {
            int i = 0;
            if (Main.bloodMoon)
            {
                contents[i++].SetDefaults(67, false);
                contents[i++].SetDefaults(59, false);
            }
            else
            {
                contents[i++].SetDefaults("Purification Powder");
                contents[i++].SetDefaults("Grass Seeds");
                contents[i++].SetDefaults("Sunflower");
            }
            contents[i++].SetDefaults("Acorn");
            contents[i].SetDefaults(114, false);
        }
    }
}
