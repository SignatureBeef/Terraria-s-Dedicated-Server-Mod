
namespace Terraria_Server
{
    public class Color
    {
        public int R;
        public int G;
        public int B;
        public int A;

        public Color() { }

        public Color(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
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
