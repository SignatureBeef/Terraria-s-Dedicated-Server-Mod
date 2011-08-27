
namespace Terraria_Server.Misc
{
    public struct Color
    {
        public int R;
        public int G;
        public int B;
        public int A;

        public Color(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = 0;
        }

        public Color(int r, int g, int b, int a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
    }
}
