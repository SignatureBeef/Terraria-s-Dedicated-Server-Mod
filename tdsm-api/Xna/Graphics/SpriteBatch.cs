using System.Text;


namespace Microsoft.Xna.Framework.Graphics
{
	public class SpriteBatch
	{
		public SpriteBatch (GraphicsDevice graphicsDevice)
		{
		}

		public GraphicsDevice GraphicsDevice { get; set; }

		public void DrawString (SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
		{
		}

		public void DrawString (SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
		}
		//public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) { }
		public void Draw (Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
		}

		public void Draw (Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
		}

		public void Draw (Texture2D texture, Vector2 position, Color color)
		{
		}

		public void Draw (Texture2D texture, Color color)
		{
		}

		public void Draw (Texture2D texture, Rectangle destinationRectangle, Color color)
		{
		}

		public void Draw (Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
		}

		public void Draw (Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
		}

		public void Draw (Texture2D a, Rectangle b, Rectangle? c, Color d, float e, Vector2 f, SpriteEffects g, float h)
		{
		}

		public void Begin (SpriteSortMode sortMode, BlendState blendState)
		{
		}

		public void Begin (SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
		{
		}

		public void Begin ()
		{
		}

		public void Begin (SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
		{
		}

		public void End ()
		{
		}


		public void DrawString (
			SpriteFont spriteFont,
			string text,
			Vector2 position,
			Color color
		)
		{
		}

		public void DrawString (
			SpriteFont spriteFont,
			string text,
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float layerDepth
		)
		{
		}

		public void DrawString (
			SpriteFont spriteFont,
			StringBuilder text,
			Vector2 position,
			Color color
		)
		{
		}

		public void DrawString (
			SpriteFont spriteFont,
			StringBuilder text,
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			float scale,
			SpriteEffects effects,
			float layerDepth
		)
		{
		}

		public void DrawString (
			SpriteFont spriteFont,
			StringBuilder text,
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float layerDepth
		)
		{
		}
	}
}
