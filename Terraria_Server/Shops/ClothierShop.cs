
namespace Terraria_Server.Shops
{
    public class ClothierShop : Shop
    {
        protected override void Setup()
        {
            int i = 0;
            contents[i++].SetDefaults(254, false);
            if (Main.dayTime)
            {
                contents[i++].SetDefaults(242, false);
                
            }
            if (Main.moonPhase == 0)
            {
                contents[i++].SetDefaults(245, false);
                contents[i++].SetDefaults(246, false);
            }
            else
            {
                if (Main.moonPhase == 1)
                {
                    contents[i++].SetDefaults(325, false);
                    contents[i++].SetDefaults(326, false);
                }
            }
            contents[i++].SetDefaults(269, false);
            contents[i++].SetDefaults(270, false);
            contents[i++].SetDefaults(271, false);
            if (Main.bloodMoon)
            {
                contents[i++].SetDefaults(322, false);
            }
        }
    }
}
