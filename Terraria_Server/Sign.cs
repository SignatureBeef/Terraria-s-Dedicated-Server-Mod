using System;

namespace Terraria_Server
{
    public class Sign
    {
        public int x;
        public int y;
        public string text;

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public static void KillSign(int x, int y)
        {
            for (int i = 0; i < Main.MAX_SIGNS; i++)
            {
                if (Main.sign[i] != null && Main.sign[i].x == x && Main.sign[i].y == y)
                {
                    Main.sign[i] = null;
                }
            }
        }

        public static int ReadSign(int i, int j)
        {
            int k = (int)(Main.tile.At(i, j).FrameX / 18);
            int num = (int)(Main.tile.At(i, j).FrameY / 18);
            while (k > 1)
            {
                k -= 2;
            }
            int num2 = i - k;
            int num3 = j - num;
            if (Main.tile.At(num2, num3).Type != 55 && Main.tile.At(num2, num3).Type != 85)
            {
                Sign.KillSign(num2, num3);
                return -1;
            }
            int num4 = -1;
			for (int l = 0; l < Main.MAX_SIGNS; l++)
            {
                if (Main.sign[l] != null && Main.sign[l].x == num2 && Main.sign[l].y == num3)
                {
                    num4 = l;
                    break;
                }
            }
            if (num4 < 0)
            {
				for (int m = 0; m < Main.MAX_SIGNS; m++)
                {
                    if (Main.sign[m] == null)
                    {
                        num4 = m;
                        Main.sign[m] = new Sign();
                        Main.sign[m].x = num2;
                        Main.sign[m].y = num3;
                        Main.sign[m].text = "";
                        break;
                    }
                }
            }
            return num4;
        }
        public static void TextSign(int i, string text)
        {
            if (!Main.tile.At(Main.sign[i].x, Main.sign[i].y).Active || (Main.tile.At(Main.sign[i].x, Main.sign[i].y).Type != 55 && Main.tile.At(Main.sign[i].x, Main.sign[i].y).Type != 85))
            {
                Main.sign[i] = null;
                return;
            }
            Main.sign[i].text = text;
        }
    }
}
