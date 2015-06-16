
using System.IO;
using tdsm.api;
using Terraria;
namespace tdsm.core.ServerCharacters
{
    public static class CharacterManager
    {
        public class NewPlayerInfo
        {
            public int Mana { get; set; }
            public int Health { get; set; }

            public PlayerItem[] Inventory { get; set; }
        }

        public static NewPlayerInfo StartingOutInfo = new NewPlayerInfo()
        {
            Health = 200,
            Mana = 20,

            Inventory = new PlayerItem[]
            {
                new PlayerItem(-15, 1, 0),
                new PlayerItem(-13, 1, 0),
                new PlayerItem(-16, 1, 0)
            }
        };

        public static void LoadConfig()
        {

        }
        
        public static ServerCharacter LoadPlayerData(Player player)
        {
            if (player.AuthenticatedAs != null)
            {
                var file = Path.Combine(Globals.CharacterDataPath, player.AuthenticatedAs, ".ssc");
                if (System.IO.File.Exists(file))
                {
                    var json = System.IO.File.ReadAllText(file);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ServerCharacter>(json);
                }
            }
            return null;
        }
        
        public static ServerCharacter SavePlayerData(Player player)
        {
            if (player.AuthenticatedAs != null)
            {
                var file = Path.Combine(Globals.CharacterDataPath, player.AuthenticatedAs, ".ssc");
                var data = new ServerCharacter()
                {
                    Mana = player.statMana
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                System.IO.File.WriteAllText(file, json);
            }
            return null;
        }
    }
}
