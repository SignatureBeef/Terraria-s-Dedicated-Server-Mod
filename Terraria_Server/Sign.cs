using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Sign
    {
        public const int maxSigns = 0x3e8;
        public string text;
        public int x;
        public int y;

        public static void KillSign(int x, int y, World world)
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                if (((world.getSigns()[i] != null) && (world.getSigns()[i].x == x)) && (world.getSigns()[i].y == y))
                {
                    world.getSigns()[i] = null;
                }
            }
        }

        public static int ReadSign(int i, int j, World world)
        {
            int num = world.getTile()[i, j].frameX / 0x12;
            int num2 = world.getTile()[i, j].frameY / 0x12;
            while (num > 1)
            {
                num -= 2;
            }
            int x = i - num;
            int y = j - num2;
            if (world.getTile()[x, y].type != 0x37)
            {
                KillSign(x, y, world);
                return -1;
            }
            int num5 = -1;
            for (int k = 0; k < 0x3e8; k++)
            {
                if (((world.getSigns()[k] != null) && (world.getSigns()[k].x == x)) && (world.getSigns()[k].y == y))
                {
                    num5 = k;
                    break;
                }
            }
            if (num5 < 0)
            {
                for (int m = 0; m < 0x3e8; m++)
                {
                    if (world.getSigns()[m] == null)
                    {
                        num5 = m;
                        world.getSigns()[m] = new Sign();
                        world.getSigns()[m].x = x;
                        world.getSigns()[m].y = y;
                        world.getSigns()[m].text = "";
                        return num5;
                    }
                }
            }
            return num5;
        }

        public static void TextSign(int i, string text, World world)
        {
            if (((world.getTile()[world.getSigns()[i].x, world.getSigns()[i].y] == null) || !world.getTile()[world.getSigns()[i].x, world.getSigns()[i].y].active) || (world.getTile()[world.getSigns()[i].x, world.getSigns()[i].y].type != 0x37))
            {
                world.getSigns()[i] = null;
            }
            else
            {
                world.getSigns()[i].text = text;
            }
        }
    }
}
