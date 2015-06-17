
using System;
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

        static bool _hadPlayers;
        static DateTime _lastSave;
        public static bool EnsureSave { get; set; }


        public static void SaveAll()
        {
            if ((DateTime.Now - _lastSave).TotalSeconds >= 5)
            {
                //Don't perform any unnecessary writes
                var hasPlayers = ServerCore.ClientConnection.All.Count > 0;
                if (!hasPlayers && !_hadPlayers && !EnsureSave) return;

                EnsureSave = false;
                foreach (var ply in Terraria.Main.player)
                {
                    if (ply != null && ply.active)
                    {
                        SavePlayerData(ply);
                    }
                }

                _hadPlayers = hasPlayers;
                _lastSave = DateTime.Now;
            }
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
                var data = new ServerCharacter(player);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                System.IO.File.WriteAllText(file, json);
            }
            return null;
        }
    }
}
