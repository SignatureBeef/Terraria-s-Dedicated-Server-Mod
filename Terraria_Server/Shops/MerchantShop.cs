
namespace Terraria_Server.Shops
{
    public class MerchantShop : Shop
    {
        protected override void Setup()
        {
            int i = 0;
            contents[i++].SetDefaults("Mining Helmet");
            contents[i++].SetDefaults("Piggy Bank");
            contents[i++].SetDefaults("Iron Anvil");
            contents[i++].SetDefaults("Copper Pickaxe");
            contents[i++].SetDefaults("Copper Axe");
            contents[i++].SetDefaults("Torch");
            contents[i++].SetDefaults("Lesser Healing Potion");
            if (Main.players[Main.myPlayer].statManaMax == 200)
            {
                contents[i++].SetDefaults("Lesser Mana Potion");
            }
            contents[i++].SetDefaults("Wooden Arrow");
            contents[i++].SetDefaults("Shuriken");
            if (Main.bloodMoon)
            {
                contents[i++].SetDefaults("Throwing Knife");
            }
            if (!Main.dayTime)
            {
                contents[i].SetDefaults("Glowstick");
            }
        }
    }
}
