
namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteFont
    {
        public static SpriteFont[] Array;
        public int LineSpacing { get; set; }

        public Vector2 MeasureString(string text)
        {
            //if (text == null)
            //{
            //    throw new ArgumentNullException("text");
            //}
            //SpriteFont.StringProxy stringProxy = new SpriteFont.StringProxy(text);
            //return this.InternalMeasure(ref stringProxy);

            return default(Vector2);
        }
    }
}
