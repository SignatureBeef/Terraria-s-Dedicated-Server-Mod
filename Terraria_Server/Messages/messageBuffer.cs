
using Terraria_Server.Events;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server.Commands;
using System;
using Terraria_Server.Shops;
using Terraria_Server.Misc;

namespace Terraria_Server.Messages
{
    public class MessageBuffer
    {
        public const int BUFFER_MAX = 65535;

        private const int MAX_HAIR_ID = 17;

        public bool broadcast;
        public bool checkBytes;

        public byte[] readBuffer;
        public byte[] writeBuffer;

        public int messageLength;
        public int spamCount;
        public int totalData;
        public int whoAmI;

        public void Reset()
        {
            readBuffer = new byte[BUFFER_MAX];
            writeBuffer = new byte[BUFFER_MAX];
            messageLength = 0;
            totalData = 0;
            spamCount = 0;
        }

        public void GetData(int start, int length)
        {
            if (whoAmI < 256)
            {
                Netplay.serverSock[whoAmI].timeOut = 0;
            }
            else
            {
                Netplay.clientSock.timeOut = 0;
            }

            int num = start + 1;
            byte bufferData = readBuffer[start];

            if (Main.netMode == 1 && Netplay.clientSock.statusMax > 0)
            {
                Netplay.clientSock.statusCount++;
            }

            if (Main.netMode == 2)
            {
                if (bufferData != 38)
                {
                    if (Netplay.serverSock[whoAmI].state == -1)
                    {
                        NetMessage.SendData(2, whoAmI, -1, "Incorrect password.");
                        return;
                    }

                    if (Netplay.serverSock[whoAmI].state < 10 && bufferData > 12 && bufferData != 16 && bufferData != 42 && bufferData != 50)
                    {
                        NetMessage.BootPlayer(whoAmI, "Invalid operation at this state.");
                    }
                }
            }

            //Certain events are only processed under specific net modes. Check for these modes first.
            bool dataSkipped = false;
            if (Main.netMode == 2)
            {
                switch (bufferData)
                {
                    case 34:
                        killTile(num);
                        break;
                    case 46:
                        ReadSign(num);
                        break;
                    default:
                        dataSkipped = true;
                        break;
                }
            }

            /*
             * If the message was not already caught it isn't specific to a certain net mode
             * and should be processed as such.
            */
            if (dataSkipped)
            {
                switch (bufferData)
                {
                    case 35:
                        heal(num);
                        break;
                    case 36:
                        enterZone(num);
                        break;
                    case 43:
                        useMana(num);
                        break;
                    case 44:
                        pvpKill(start, length, num);
                        break;
                    case 45:
                        joinParty(num);
                        break;
                    case 47:
                        WriteSign(start, length, num);
                        break;
                    case 48:
                        FlowLiquid(num);
                        break;
                    case 50:
                        Buffs(num);
                        break;
                    case 51:
                        SummonSkeletron(num);
                        break;
                }
            }
        }



        private void killTile(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            if (Main.tile[x, y].type == 21)
            {
                WorldGen.KillTile(x, y);
                if (!Main.tile[x, y].active)
                {
                    NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
                }
            }
        }


        private void heal(int num)
        {
            int playerIndex = (int)readBuffer[num++];

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int heal = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.player[playerIndex].HealEffect(heal);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(35, -1, whoAmI, "", playerIndex, (float)heal);
            }
        }


        private void enterZone(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.player[playerIndex];
            player.zoneEvil = (readBuffer[num++] != 0);
            player.zoneMeteor = (readBuffer[num++] != 0);
            player.zoneDungeon = (readBuffer[num++] != 0);
            player.zoneJungle = (readBuffer[num++] != 0);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(36, -1, whoAmI, "", playerIndex);
            }
        }


        private void useMana(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int manaAmount = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.player[playerIndex].ManaEffect(manaAmount);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(43, -1, whoAmI, "", playerIndex, (float)manaAmount);
            }
        }


        private void pvpKill(int start, int length, int num)
        {
            int playerIndex = readBuffer[num++];
            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int direction = (int)(readBuffer[num++] - 1);

            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte pvpFlag = readBuffer[num++];

            string deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            bool pvp = (pvpFlag != 0);

            Main.player[playerIndex].KillMe((double)damage, direction, pvp, deathText);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(44, -1, whoAmI, deathText, playerIndex, (float)direction, (float)damage, (float)pvpFlag, 0);
            }
        }


        private void joinParty(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int teamIndex = (int)readBuffer[num++];
            Player player = Main.player[playerIndex];
            int currentTeam = player.team;

            if (Main.netMode == 2)
            {
                NetMessage.SendData(45, -1, whoAmI, "", playerIndex);
                Party party = Party.NONE;
                string joinMessage = "";
                switch (teamIndex)
                {
                    case 0:
                        joinMessage = " is no longer on a party.";
                        break;
                    case 1:
                        joinMessage = " has joined the red party.";
                        party = Party.RED;
                        break;
                    case 2:
                        joinMessage = " has joined the green party.";
                        party = Party.GREEN;
                        break;
                    case 3:
                        joinMessage = " has joined the blue party.";
                        party = Party.BLUE;
                        break;
                    case 4:
                        joinMessage = " has joined the yellow party.";
                        party = Party.YELLOW;
                        break;
                }

                PartyChangeEvent changeEvent = new PartyChangeEvent();
                changeEvent.PartyType = party;
                changeEvent.Sender = Main.player[whoAmI];
                Program.server.getPluginManager().processHook(Hooks.PLAYER_PARTYCHANGE, changeEvent);
                if (changeEvent.Cancelled)
                {
                    return;
                }

                player.team = teamIndex;
                for (int i = 0; i < 255; i++)
                {
                    if (i == whoAmI
                        || (currentTeam > 0 && player.team == currentTeam)
                        || (teamIndex > 0 && player.team == teamIndex))
                    {
                        NetMessage.SendData(25, i, -1, player.name + joinMessage, 255, (float)Main.teamColor[teamIndex].R, (float)Main.teamColor[teamIndex].G, (float)Main.teamColor[teamIndex].B);
                    }
                }
            }
        }


        private void ReadSign(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            int signIndex = Sign.ReadSign(x, y);
            if (signIndex >= 0)
            {
                NetMessage.SendData(47, whoAmI, -1, "", signIndex);
            }
        }


        private void WriteSign(int start, int length, int num)
        {
            int signIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            string string11 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            Main.sign[signIndex] = new Sign();
            Sign sign = Main.sign[signIndex];
            sign.x = x;
            sign.y = y;
            Sign.TextSign(signIndex, string11);
            Player player = Main.player[Main.myPlayer];

            if (Main.netMode == 1 
                && sign != null
                && signIndex != player.sign)
            {
                Main.playerInventory = false;
                player.talkNPC = -1;
                Main.editSign = false;
                player.sign = signIndex;
                Main.npcChatText = sign.text;
            }
        }


        private void FlowLiquid(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            byte liquid = readBuffer[num++];
            byte lavaFlag = readBuffer[num]++;

            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int playerIndex = whoAmI;
                Player player = Main.player[playerIndex];
                int centerX = (int)(player.position.X + (float)(player.width / 2));
                int centerY = (int)(player.position.Y + (float)(player.height / 2));
                int disperseDistance = 10;
                int left = centerX - disperseDistance;
                int right = centerX + disperseDistance;
                int top = centerY - disperseDistance;
                int bottom = centerY + disperseDistance;
                if (centerX < left || centerX > right || centerY < top || centerY > bottom)
                {
                    NetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                    return;
                }
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            Tile tile = Main.tile[x, y];
            lock (tile)
            {
                tile.liquid = liquid;
                tile.lava = (lavaFlag == 1);

                if (Main.netMode == 2)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                }
            }
        }

        private void Buffs(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }
            else if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.player[playerIndex];
            for (int i = 0; i < 10; i++)
            {
                player.buffType[i] = (int)readBuffer[num++];
                if (player.buffType[i] > 0)
                {
                    player.buffTime[i] = 60;
                }
                else
                {
                    player.buffTime[i] = 0;
                }
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(50, -1, whoAmI, "", playerIndex);
            }
        }


        private void SummonSkeletron(int num)
        {
            byte buffer = readBuffer[num];
            if (buffer == 1)
            {
                NPC.SpawnSkeletron();
            }
        }
    }
}