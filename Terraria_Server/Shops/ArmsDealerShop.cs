
namespace Terraria_Server.Shops
{
    public class ArmsDealerShop : Shop
    {
        protected override void Setup()
        {
            int i = 0;
            contents[i++].SetDefaults("Musket Ball");
            if (Main.bloodMoon)
            {
                contents[i++].SetDefaults("Silver Bullet");
            }
            if (NPC.downedBoss2 && !Main.dayTime)
            {
                contents[i++].SetDefaults(47, false);
            }
            contents[i++].SetDefaults("Flintlock Pistol");
            contents[i++].SetDefaults("Minishark");
            if (Main.moonPhase == 4)
            {
                contents[i].SetDefaults(324, false);
            }
        }
    }
}
