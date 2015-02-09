
namespace tdsm.api.Callbacks
{
    public static class MessageBuffer
    {
//        public static string PlayerNameFormatForChat = "<{0}> {1}";
//        public static string PlayerNameFormatForConsole = PlayerNameFormatForChat;
//
        public static byte ProcessPacket(int playerId, byte packetId)
        {
            //var buffer = NetMessage.buffer[playerId].reader;

            //switch (packetId)
            //{
            //    case 26:
            //        buffer.ReadByte(); //Discard from user
            //        buffer.ReadRGB();

            //        var color = new Color(255, 255, 255);

            //        var text = buffer.ReadString();
            //        var lowered = text.ToLower();

            //        if (lowered == Lang.mp[6] || lowered == Lang.mp[21])
            //        {
            //            string text3 = String.Empty;
            //            for (int num62 = 0; num62 < 255; num62++)
            //            {
            //                if (Main.player[num62].active)
            //                {
            //                    if (text3 == String.Empty)
            //                        text3 = Main.player[num62].name;
            //                    else
            //                        text3 = text3 + ", " + Main.player[num62].name;
            //                }
            //            }
            //            NetMessage.SendData(25, playerId, -1, Lang.mp[7] + " " + text3 + ".", 255, 255f, 240f, 20f, 0);
            //            return 0;
            //        }
            //        if (lowered.StartsWith("/me "))
            //        {
            //            NetMessage.SendData(25, -1, -1, "*" + Main.player[playerId].name + " " + text.Substring(4), 255, 200f, 100f, 0f, 0);
            //            return 0;
            //        }
            //        if (lowered == Lang.mp[8])
            //        {
            //            NetMessage.SendData(25, -1, -1, string.Concat(new object[]
            //            {
            //                "*",
            //                Main.player[playerId].name,
            //                " ",
            //                Lang.mp[9],
            //                " ",
            //                Main.rand.Next(1, 101)
            //            }), 255, 255f, 240f, 20f, 0);
            //            return 0;
            //        }
            //        if (lowered.StartsWith("/p "))
            //        {
            //            int team = Main.player[playerId].team;
            //            color = Main.teamColor[team];
            //            if (team != 0)
            //            {
            //                for (int num63 = 0; num63 < 255; num63++)
            //                {
            //                    if (Main.player[num63].team == team)
            //                        NetMessage.SendData(25, num63, -1, text.Substring(3), (int)playerId, (float)color.R, (float)color.G, (float)color.B, 0);
            //                }
            //                return 0;
            //            }
            //            NetMessage.SendData(25, playerId, -1, Lang.mp[10], 255, 255f, 240f, 20f, 0);
            //            return 0;
            //        }
            //        else
            //        {
            //            if (Main.player[playerId].difficulty == 2)
            //                color = Main.hcColor;
            //            else if (Main.player[playerId].difficulty == 1)
            //                color = Main.mcColor;

            //            NetMessage.SendData(25, -1, -1, text, (int)playerId, (float)color.R, (float)color.G, (float)color.B, 0);
            //            Tools.WriteLine(String.Format(PlayerNameFormatForConsole, Main.player[playerId].name, text));
            //        }
            //        return 0;
            //}

            //return false;
            return packetId;
        }
    }
}
