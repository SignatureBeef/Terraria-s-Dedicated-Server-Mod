using System;


namespace Microsoft.Xna.Framework.Graphics
{
	public abstract class Texture : GraphicsResource
	{

	}

	public class Texture2D : Texture
	{
		public static Texture2D[] Array;

		public int Height { get; set; }

		public int Width { get; set; }

		public Texture2D (
			GraphicsDevice graphicsDevice,
			int width,
			int height,
			bool mipMap,
			SurfaceFormat format
		)
		{
		}

		public Texture2D (
			GraphicsDevice graphicsDevice,
			int width,
			int height
		)
		{
		}

		public void SetData<T> (T[] data, int startIndex, int elementCount) where T : struct
		{
			//this.SetData<T>(0, null, data, startIndex, elementCount);
		}

		public void GetData<T> (
			T[] data
		)
		{

		}

		public void GetData<T> (
			T[] data,
			int startIndex,
			int elementCount
		)
		{

		}

		public void GetData<T> (
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