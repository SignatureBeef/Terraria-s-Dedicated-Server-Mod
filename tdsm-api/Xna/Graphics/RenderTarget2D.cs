
using System.Runtime.InteropServices;
using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public class RenderTarget2D : Texture2D
	{
		public static RenderTarget2D[,] Array = new RenderTarget2D[0, 0];

		public bool IsContentLost { get { return false; } set { } }

		public RenderTarget2D (GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice, width, height)
		{
		}

        public RenderTarget2D (GraphicsDevice graphicsDevice, int width, int height, [MarshalAs (UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
            : base(graphicsDevice, width, height)
		{

		}

        public RenderTarget2D (GraphicsDevice graphicsDevice, int width, int height, [MarshalAs (UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
            : base(graphicsDevice, width, height)
		{
		}

//		void IDisposable.Dispose ()
//		{
//		}
	}
	// : Texture2D, IDynamicGraphicsResource
}
