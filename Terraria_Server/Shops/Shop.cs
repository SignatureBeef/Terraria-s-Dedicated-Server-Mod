
namespace Terraria_Server.Shops
{
    public abstract class Shop : Chest
    {
        public Shop()
        {
            Setup();
        }

        protected abstract void Setup();
    }
}
