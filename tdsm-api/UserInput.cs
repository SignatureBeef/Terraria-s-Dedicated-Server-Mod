using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace tdsm.api
{
    public static class Patches
    {
        public static string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }
    }

    public static class UserInput
    {
//        static readonly List<String> _officialCommands = new List<String>() 
//        {
//            "help",
//            "playing",
//            "clear",
//            "exit",
//            "exit-nosave",
//            "save",
//            "kick",
//            "ban",
//            "password",
//            "version",
//            "time",
//            "port",
//            "maxplayers",
//            "say",
//            "motd",
//            "dawn",
//            "noon",
//            "dusk",
//            "midnight",
//            "settle"
//        };

        public static void startDedInputCallBack(object threadContext)
        {
//            while (!Netplay.disconnect)
//            {
//                Console.Write(": ");
//                string text = Console.ReadLine();
//                string text2 = text;
//                text = text.ToLower();
//                try
//                {
//                    if (text == "help")
//                    {
//                        Console.WriteLine("Available commands:");
//                        Console.WriteLine("");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"help ",
//					'\t',
//					'\t',
//					" Displays a list of commands."
//				}));
//                        Console.WriteLine("playing " + '\t' + " Shows the list of players");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"clear ",
//					'\t',
//					'\t',
//					" Clear the console window."
//				}));
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"exit ",
//					'\t',
//					'\t',
//					" Shutdown the server and save."
//				}));
//                        Console.WriteLine("exit-nosave " + '\t' + " Shutdown the server without saving.");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"save ",
//					'\t',
//					'\t',
//					" Save the game world."
//				}));
//                        Console.WriteLine("kick <player> " + '\t' + " Kicks a player from the server.");
//                        Console.WriteLine("ban <player> " + '\t' + " Bans a player from the server.");
//                        Console.WriteLine("password" + '\t' + " Show password.");
//                        Console.WriteLine("password <pass>" + '\t' + " Change password.");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"version",
//					'\t',
//					'\t',
//					" Print version number."
//				}));
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"time",
//					'\t',
//					'\t',
//					" Display game time."
//				}));
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"port",
//					'\t',
//					'\t',
//					" Print the listening port."
//				}));
//                        Console.WriteLine("maxplayers" + '\t' + " Print the max number of players.");
//                        Console.WriteLine("say <words>" + '\t' + " Send a message.");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"motd",
//					'\t',
//					'\t',
//					" Print MOTD."
//				}));
//                        Console.WriteLine("motd <words>" + '\t' + " Change MOTD.");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"dawn",
//					'\t',
//					'\t',
//					" Change time to dawn."
//				}));
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"noon",
//					'\t',
//					'\t',
//					" Change time to noon."
//				}));
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"dusk",
//					'\t',
//					'\t',
//					" Change time to dusk."
//				}));
//                        Console.WriteLine("midnight" + '\t' + " Change time to midnight.");
//                        Console.WriteLine(string.Concat(new object[]
//				{
//					"settle",
//					'\t',
//					'\t',
//					" Settle all water."
//				}));
//                    }
//                    else
//                    {
//                        if (text == "settle")
//                        {
//                            if (!Liquid.panicMode)
//                            {
//                                Liquid.StartPanic();
//                            }
//                            else
//                            {
//                                Console.WriteLine("Water is already settling");
//                            }
//                        }
//                        else
//                        {
//                            if (text == "dawn")
//                            {
//                                Main.dayTime = true;
//                                Main.time = 0.0;
//                                NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
//                            }
//                            else
//                            {
//                                if (text == "dusk")
//                                {
//                                    Main.dayTime = false;
//                                    Main.time = 0.0;
//                                    NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
//                                }
//                                else
//                                {
//                                    if (text == "noon")
//                                    {
//                                        Main.dayTime = true;
//                                        Main.time = 27000.0;
//                                        NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
//                                    }
//                                    else
//                                    {
//                                        if (text == "midnight")
//                                        {
//                                            Main.dayTime = false;
//                                            Main.time = 16200.0;
//                                            NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0);
//                                        }
//                                        else
//                                        {
//                                            if (text == "exit-nosave")
//                                            {
//                                                Netplay.disconnect = true;
//                                            }
//                                            else
//                                            {
//                                                if (text == "exit")
//                                                {
//                                                    WorldFile.saveWorld(false);
//                                                    Netplay.disconnect = true;
//                                                }
//                                                else
//                                                {
//                                                    if (text == "fps")
//                                                    {
//                                                        if (!Main.dedServFPS)
//                                                        {
//                                                            Main.dedServFPS = true;
//                                                            Main.fpsTimer.Reset();
//                                                        }
//                                                        else
//                                                        {
//                                                            Main.dedServCount1 = 0;
//                                                            Main.dedServCount2 = 0;
//                                                            Main.dedServFPS = false;
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        if (text == "save")
//                                                        {
//                                                            WorldFile.saveWorld(false);
//                                                        }
//                                                        else
//                                                        {
//                                                            if (text == "time")
//                                                            {
//                                                                string text3 = "AM";
//                                                                double num = Main.time;
//                                                                if (!Main.dayTime)
//                                                                {
//                                                                    num += 54000.0;
//                                                                }
//                                                                num = num / 86400.0 * 24.0;
//                                                                double num2 = 7.5;
//                                                                num = num - num2 - 12.0;
//                                                                if (num < 0.0)
//                                                                {
//                                                                    num += 24.0;
//                                                                }
//                                                                if (num >= 12.0)
//                                                                {
//                                                                    text3 = "PM";
//                                                                }
//                                                                int num3 = (int)num;
//                                                                double num4 = num - (double)num3;
//                                                                num4 = (double)((int)(num4 * 60.0));
//                                                                string text4 = string.Concat(num4);
//                                                                if (num4 < 10.0)
//                                                                {
//                                                                    text4 = "0" + text4;
//                                                                }
//                                                                if (num3 > 12)
//                                                                {
//                                                                    num3 -= 12;
//                                                                }
//                                                                if (num3 == 0)
//                                                                {
//                                                                    num3 = 12;
//                                                                }
//                                                                Console.WriteLine(string.Concat(new object[]
//														{
//															"Time: ",
//															num3,
//															":",
//															text4,
//															" ",
//															text3
//														}));
//                                                            }
//                                                            else
//                                                            {
//                                                                if (text == "maxplayers")
//                                                                {
//                                                                    Console.WriteLine("Player limit: " + Main.maxNetPlayers);
//                                                                }
//                                                                else
//                                                                {
//                                                                    if (text == "port")
//                                                                    {
//                                                                        Console.WriteLine("Port: " + Netplay.serverPort);
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        if (text == "version")
//                                                                        {
//                                                                            Console.WriteLine("Terraria Server " + Main.versionNumber);
//                                                                        }
//                                                                        else
//                                                                        {
//                                                                            if (text == "clear")
//                                                                            {
//                                                                                try
//                                                                                {
//                                                                                    Console.Clear();
//                                                                                    continue;
//                                                                                }
//                                                                                catch
//                                                                                {
//                                                                                    continue;
//                                                                                }
//                                                                            }
//                                                                            if (text == "playing")
//                                                                            {
//                                                                                int num5 = 0;
//                                                                                for (int i = 0; i < 255; i++)
//                                                                                {
//                                                                                    if (Main.player[i].active)
//                                                                                    {
//                                                                                        num5++;
//                                                                                        Console.WriteLine(string.Concat(new object[]
//																				{
//																					Main.player[i].name,
//																					" (",
//																					Netplay.serverSock[i].tcpClient.Client.RemoteEndPoint,
//																					")"
//																				}));
//                                                                                    }
//                                                                                }
//                                                                                if (num5 == 0)
//                                                                                {
//                                                                                    Console.WriteLine("No players connected.");
//                                                                                }
//                                                                                else
//                                                                                {
//                                                                                    if (num5 == 1)
//                                                                                    {
//                                                                                        Console.WriteLine("1 player connected.");
//                                                                                    }
//                                                                                    else
//                                                                                    {
//                                                                                        Console.WriteLine(num5 + " players connected.");
//                                                                                    }
//                                                                                }
//                                                                            }
//                                                                            else
//                                                                            {
//                                                                                if (!(text == ""))
//                                                                                {
//                                                                                    if (text == "motd")
//                                                                                    {
//                                                                                        if (Main.motd == "")
//                                                                                        {
//                                                                                            Console.WriteLine("Welcome to " + Main.worldName + "!");
//                                                                                        }
//                                                                                        else
//                                                                                        {
//                                                                                            Console.WriteLine("MOTD: " + Main.motd);
//                                                                                        }
//                                                                                    }
//                                                                                    else
//                                                                                    {
//                                                                                        if (text.Length >= 5 && text.Substring(0, 5) == "motd ")
//                                                                                        {
//                                                                                            string text5 = text2.Substring(5);
//                                                                                            Main.motd = text5;
//                                                                                        }
//                                                                                        else
//                                                                                        {
//                                                                                            if (text.Length == 8 && text.Substring(0, 8) == "password")
//                                                                                            {
//                                                                                                if (Netplay.password == "")
//                                                                                                {
//                                                                                                    Console.WriteLine("No password set.");
//                                                                                                }
//                                                                                                else
//                                                                                                {
//                                                                                                    Console.WriteLine("Password: " + Netplay.password);
//                                                                                                }
//                                                                                            }
//                                                                                            else
//                                                                                            {
//                                                                                                if (text.Length >= 9 && text.Substring(0, 9) == "password ")
//                                                                                                {
//                                                                                                    string password = text2.Substring(9);
//                                                                                                    if (password == "")
//                                                                                                    {
//                                                                                                        Netplay.password = "";
//                                                                                                        Console.WriteLine("Password disabled.");
//                                                                                                    }
//                                                                                                    else
//                                                                                                    {
//                                                                                                        Netplay.password = password;
//                                                                                                        Console.WriteLine("Password: " + Netplay.password);
//                                                                                                    }
//                                                                                                }
//                                                                                                else
//                                                                                                {
//                                                                                                    if (text == "say")
//                                                                                                    {
//                                                                                                        Console.WriteLine("Usage: say <words>");
//                                                                                                    }
//                                                                                                    else
//                                                                                                    {
//                                                                                                        if (text.Length >= 4 && text.Substring(0, 4) == "say ")
//                                                                                                        {
//                                                                                                            string str = text2.Substring(4);
//                                                                                                            if (str == "")
//                                                                                                            {
//                                                                                                                Console.WriteLine("Usage: say <words>");
//                                                                                                            }
//                                                                                                            else
//                                                                                                            {
//                                                                                                                Console.WriteLine("<Server> " + str);
//                                                                                                                NetMessage.SendData(25, -1, -1, "<Server> " + str, 255, 255f, 240f, 20f, 0);
//                                                                                                            }
//                                                                                                        }
//                                                                                                        else
//                                                                                                        {
//                                                                                                            if (text.Length == 4 && text.Substring(0, 4) == "kick")
//                                                                                                            {
//                                                                                                                Console.WriteLine("Usage: kick <player>");
//                                                                                                            }
//                                                                                                            else
//                                                                                                            {
//                                                                                                                if (text.Length >= 5 && text.Substring(0, 5) == "kick ")
//                                                                                                                {
//                                                                                                                    string text6 = text.Substring(5);
//                                                                                                                    text6 = text6.ToLower();
//                                                                                                                    if (text6 == "")
//                                                                                                                    {
//                                                                                                                        Console.WriteLine("Usage: kick <player>");
//                                                                                                                    }
//                                                                                                                    else
//                                                                                                                    {
//                                                                                                                        for (int j = 0; j < 255; j++)
//                                                                                                                        {
//                                                                                                                            if (Main.player[j].active && Main.player[j].name.ToLower() == text6)
//                                                                                                                            {
//                                                                                                                                NetMessage.SendData(2, j, -1, "Kicked from server.", 0, 0f, 0f, 0f, 0);
//                                                                                                                            }
//                                                                                                                        }
//                                                                                                                    }
//                                                                                                                }
//                                                                                                                else
//                                                                                                                {
//                                                                                                                    if (text.Length == 3 && text.Substring(0, 3) == "ban")
//                                                                                                                    {
//                                                                                                                        Console.WriteLine("Usage: ban <player>");
//                                                                                                                    }
//                                                                                                                    else
//                                                                                                                    {
//                                                                                                                        if (text.Length >= 4 && text.Substring(0, 4) == "ban ")
//                                                                                                                        {
//                                                                                                                            string text7 = text.Substring(4);
//                                                                                                                            text7 = text7.ToLower();
//                                                                                                                            if (text7 == "")
//                                                                                                                            {
//                                                                                                                                Console.WriteLine("Usage: ban <player>");
//                                                                                                                            }
//                                                                                                                            else
//                                                                                                                            {
//                                                                                                                                for (int k = 0; k < 255; k++)
//                                                                                                                                {
//                                                                                                                                    if (Main.player[k].active && Main.player[k].name.ToLower() == text7)
//                                                                                                                                    {
//                                                                                                                                        Netplay.AddBan(k);
//                                                                                                                                        NetMessage.SendData(2, k, -1, "Banned from server.", 0, 0f, 0f, 0f, 0);
//                                                                                                                                    }
//                                                                                                                                }
//                                                                                                                            }
//                                                                                                                        }
//                                                                                                                        else
//                                                                                                                        {
//                                                                                                                            Console.WriteLine("Invalid command.");
//                                                                                                                        }
//                                                                                                                    }
//                                                                                                                }
//                                                                                                            }
//                                                                                                        }
//                                                                                                    }
//                                                                                                }
//                                                                                            }
//                                                                                        }
//                                                                                    }
//                                                                                }
//                                                                            }
//                                                                        }
//                                                                    }
//                                                                }
//                                                            }
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                catch
//                {
//                    Console.WriteLine("Invalid command.");
//                }
//            }
        }


        //public static readonly Tile DefaultTile = default(Tile);
        //public static Tile GetTile()
        //{
        //    return DefaultTile;
        //}

        //public static int X = 0;
        //public static Tile[,] Tiles;

        public static string ProcessInput(string value)
        {
			return value;
//            var t = String.Empty;
//            //int x = 0, y = 0;
//            //if (Tiles[x, y] == DefaultTile)
//            //{
//            //    Tiles[x, y] = DefaultTile;
//            //}
//
//            //Main.tile[0, 0] = default(Tile);
//            //if (Main.tile[0, 0] == DefaultTile)
//            //{
//            //    return "";
//            //}
//
//            //if (Main.netMode == 1 || X > Main.maxTilesX || X > Main.maxTilesY)
//            //{
//            //    for (int k = 0; k < X; k++)
//            //    {
//            //        float num2 = (float)k / (float)X;
//            //        for (int l = 0; l < X; l++)
//            //        {
//            //            Main.tile[k, l] = default(Tile);
//            //        }
//            //    }
//            //}
//
//            if (!String.IsNullOrEmpty(value))
//            {
//                var command = value.Split(' ').First().ToLower();
//                if (_officialCommands.Contains(command) && command != "help") return value;
//
//                if (command == "spawnnpc")
//                {
//
//                }
//                else if (command == "test")
//                {
//                    //Console.WriteLine("Active player count: " + Terraria.Main.player.Where(x => x != null && x.active).Count());
//                }
//                else if (command == "help")
//                {
//                    Console.WriteLine("spawnnpc " + '\t' + " Spawn an npc by name or id");
//                    return command; //Allow printout of defaults
//                }
//                else
//                {
//                    Console.WriteLine("Server command does not exist.");
//                }
//            }
//            return t;
        }
    }
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct TileData
    //{ }
}
