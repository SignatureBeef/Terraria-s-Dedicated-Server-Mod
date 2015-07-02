using System;


namespace Microsoft.Xna.Framework.Graphics
{
    public class Texture2D
    {
        public static Texture2D[] Array;

        public int Height { get; set; }

        public int Width { get; set; }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            //this.SetData<T>(0, null, data, startIndex, elementCount);
        }

        public void GetData<T>(
            T[] data
        )
        {

        }

        public void GetData<T>(
            T[] data,
            int startIndex,
            int elementCount
        ) 
        {

        }

        public void GetData<T>(
            int level,
            Nullable<Rectangle> rect,
            T[] data,
            int startIndex,
            int elementCount
        ) 
        {

        }
    }
}