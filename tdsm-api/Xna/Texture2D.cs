
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    }
    public class SpriteFont
    {
        public static SpriteFont[] Array;

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
    public class RasterizerState
    {
        public static readonly RasterizerState CullCounterClockwise = null;
    }
    public enum SurfaceFormat
    {
        Color,
        Bgr565,
        Bgra5551,
        Bgra4444,
        Dxt1,
        Dxt3,
        Dxt5,
        NormalizedByte2,
        NormalizedByte4,
        Rgba1010102,
        Rg32,
        Rgba64,
        Alpha8,
        Single,
        Vector2,
        Vector4,
        HalfSingle,
        HalfVector2,
        HalfVector4,
        HdrBlendable
    }
    public enum DepthFormat
    {
        None,
        Depth16,
        Depth24,
        Depth24Stencil8
    }
    public enum RenderTargetUsage
    {
        DiscardContents,
        PreserveContents,
        PlatformContents
    }
    public class RenderTarget2D
    {
        public static RenderTarget2D[,] Array = new RenderTarget2D[0, 0];

        public bool IsContentLost { get { return false; } set { } }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height)
        {
        }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {

        }
        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat) { }
    } // : Texture2D, IDynamicGraphicsResource
    public class GraphicsAdapter
    {
        public static GraphicsAdapter DefaultAdapter
        {
            get { return default(GraphicsAdapter); }
        }

        public DisplayMode CurrentDisplayMode
        {
            get { return default(DisplayMode); }
        }

        public DisplayModeCollection SupportedDisplayModes
        {
            get { return default(DisplayModeCollection); }
        }
    }
    public class DisplayMode
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class DisplayModeCollection
    {
        public IEnumerator<DisplayMode> GetEnumerator() { return null; }
    }
    public enum SpriteSortMode
    {
        Deferred,
        Immediate,
        Texture,
        BackToFront,
        FrontToBack
    }
    [Flags]
    public enum SpriteEffects
    {
        None = 0,
        FlipHorizontally = 1,
        FlipVertically = 2
    }
    public class SpriteBatch
    {
        public SpriteBatch(GraphicsDevice graphicsDevice) { }
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) { }
        //public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) { }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) { }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) { }
        public void Draw(Texture2D texture, Vector2 position, Color color) { }
        public void Draw(Texture2D texture, Color color) { }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color) { }
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) { }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) { }
        public void Draw(Texture2D a, Rectangle b, Rectangle? c, Color d, float e, Vector2 f, SpriteEffects g, float h) { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState) { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix) { }
        public void Begin() { }
        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState) { }
        public void End() { }
    }
    public class Effect
    {
        public EffectTechnique CurrentTechnique { get; set; }
    }
    public class EffectTechnique
    {
        public EffectPassCollection Passes { get; set; }
    }
    public class EffectPass
    {
        public void Apply() { }

    }
    public class EffectPassCollection
    {
        public EffectPass this[int index]
        {
            get
            {
                return default(EffectPass);
            }
        }
        /// <summary>Gets a specific element in the collection by using a name.</summary>
        /// <param name="name">Name of the EffectPass to get.</param>
        public EffectPass this[string name]
        {
            get
            {
                return default(EffectPass);
            }
        }

        //public EffectPass Item(int index)
        //{
        //    return default(EffectPass);
        //}
    }
    //public struct EffectPassCollection { }
    public enum BlendState { }
    public class SamplerState { }
    public class DepthStencilState
    {
        public DepthStencilState() { }
        public bool DepthBufferEnable { get; set; }
    }
    public class GraphicsDevice
    {
        public void Clear(Color colour) { }
        public void SetRenderTarget(RenderTarget2D renderTarget) { }
        public DepthStencilState DepthStencilState { get; set; }
        public PresentationParameters PresentationParameters { get; set; }
        public Viewport Viewport { get; set; }
    }
    public class GraphicsResource
    {
        public void Dispose() { }
    }
    public class PresentationParameters
    {
        public SurfaceFormat BackBufferFormat { get; set; }
        public int BackBufferWidth { get; set; }
        public int BackBufferHeight { get; set; }
    }
    public struct Viewport
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

namespace Microsoft.Xna.Framework.Audio
{
    public class AudioEngine
    {
        public AudioEngine(string settingsFile) { }

    }
    public class SoundBank
    {
        public static SoundBank[] Array;

        public SoundBank(AudioEngine audioEngine, string filename) { }

        public Cue GetCue(string name) { return default(Cue); }

    }
    public class WaveBank
    {
        public static WaveBank[] Array;

        public WaveBank(AudioEngine audioEngine, string nonStreamingWaveBankFilename) { }
        public WaveBank(AudioEngine audioEngine, string streamingWaveBankFilename, int offset, short packetsize) { }
    }
    public class SoundEffect
    {
        public static SoundEffect[] Array;

        public SoundEffectInstance CreateInstance()
        {
            //SoundEffectInstance result;
            //lock (this.syncObject)
            //{
            //    if (this.IsDisposed)
            //    {
            //        throw new ObjectDisposedException(base.GetType().Name, FrameworkResources.ObjectDisposedException);
            //    }
            //    SoundEffectInstance soundEffectInstance = new SoundEffectInstance(this, false);
            //    lock (this.children)
            //    {
            //        this.children.Add(new WeakReference(soundEffectInstance));
            //    }
            //    result = soundEffectInstance;
            //}
            //return result;
            return default(SoundEffectInstance);
        }
    }
    public class Cue
    {
        public static Cue[] Array;
        public bool IsPaused { get; set; }
        public bool IsPlaying { get; set; }

        public void Play() { }
        public void Pause() { }
        public void Resume() { }

        public void Stop(AudioStopOptions options) { }
        public void SetVariable(string name, float value) { }
    }
    public enum AudioStopOptions
    {
        AsAuthored,
        Immediate
    }
    public enum SoundState
    {
        Playing,
        Paused,
        Stopped
    }
    public class SoundEffectInstance
    {
        public static SoundEffectInstance[] Array;

        public float Volume { get; set; }
        public float Pan { get; set; }
        public float Pitch { get; set; }
        public SoundState State { get; set; }

        public void Play() { }

        public void Stop()
        {
            this.Stop(true);
        }

        public void Stop(bool immediate)
        {
            //lock (this.voiceHandleLock)
            //{
            //    if (this.IsDisposed)
            //    {
            //        throw new ObjectDisposedException(base.GetType().Name, FrameworkResources.ObjectDisposedException);
            //    }
            //    Helpers.ThrowExceptionFromErrorCode(SoundEffectUnsafeNativeMethods.Stop(this.voiceHandle, immediate));
            //}
        }
    }
}

namespace Microsoft.Xna.Framework
{
    public enum PlayerIndex
    {
        One,
        Two,
        Three,
        Four
    }
}
namespace Microsoft.Xna.Framework.Input
{
    public struct Keyboard
    {
        public static Keys[] Array;

        public static KeyboardState GetState() { return default(KeyboardState); }
    }
    public struct KeyboardState
    {
        public bool IsKeyDown(Keys key)
        {
            return false;
        }

        public Keys[] GetPressedKeys() { return null; }
    }
    public struct Mouse
    {
        public static MouseState GetState()
        {
            return default(MouseState);
        }
    }
    public struct MouseState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ButtonState LeftButton { get; set; }
        public ButtonState MiddleButton { get; set; }
        public ButtonState RightButton { get; set; }
        public int ScrollWheelValue { get; set; }
    }
    public struct GamePad
    {
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return default(GamePadState);
        }
    }
    public struct GamePadState
    {
        public GamePadDPad DPad { get; set; }
        public GamePadTriggers Triggers { get; set; }
        public GamePadThumbSticks ThumbSticks { get; set; }
    }
    public struct GamePadDPad
    {
        public ButtonState Up { get; set; }
        public ButtonState Down { get; set; }
        public ButtonState Left { get; set; }
        public ButtonState Right { get; set; }
    }
    public struct GamePadTriggers
    {
        public float Right { get; set; }
        public float Left { get; set; }
    }
    public struct GamePadThumbSticks
    {
        public Vector2 Right { get; set; }
        public Vector2 Left { get; set; }
    }
    public enum Keys
    {
        A = 65,
        Add = 107,
        Apps = 93,
        Attn = 246,
        B = 66,
        Back = 8,
        BrowserBack = 166,
        BrowserFavorites = 171,
        BrowserForward = 167,
        BrowserHome = 172,
        BrowserRefresh = 168,
        BrowserSearch = 170,
        BrowserStop = 169,
        C = 67,
        CapsLock = 20,
        Crsel = 247,
        D = 68,
        D0 = 48,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        Decimal = 110,
        Delete = 46,
        Divide = 111,
        Down = 40,
        E = 69,
        End = 35,
        Enter = 13,
        EraseEof = 249,
        Escape = 27,
        Execute = 43,
        Exsel = 248,
        F = 70,
        F1 = 112,
        F10 = 121,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F2 = 113,
        F20 = 131,
        F21,
        F22,
        F23,
        F24,
        F3 = 114,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        G = 71,
        H,
        Help = 47,
        Home = 36,
        I = 73,
        ImeConvert = 28,
        ImeNoConvert,
        Insert = 45,
        J = 74,
        K,
        Kana = 21,
        Kanji = 25,
        L = 76,
        LaunchApplication1 = 182,
        LaunchApplication2,
        LaunchMail = 180,
        LeftControl = 162,
        Left = 37,
        LeftAlt = 164,
        LeftShift = 160,
        LeftWindows = 91,
        M = 77,
        MediaNextTrack = 176,
        MediaPlayPause = 179,
        MediaPreviousTrack = 177,
        MediaStop,
        Multiply = 106,
        N = 78,
        None = 0,
        NumLock = 144,
        NumPad0 = 96,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,
        O = 79,
        OemAuto = 243,
        OemCopy = 242,
        OemEnlW = 244,
        OemSemicolon = 186,
        OemBackslash = 226,
        OemQuestion = 191,
        OemTilde,
        OemOpenBrackets = 219,
        OemPipe,
        OemCloseBrackets,
        OemQuotes,
        Oem8,
        OemClear = 254,
        OemComma = 188,
        OemMinus,
        OemPeriod,
        OemPlus = 187,
        P = 80,
        Pa1 = 253,
        PageDown = 34,
        PageUp = 33,
        Pause = 19,
        Play = 250,
        Print = 42,
        PrintScreen = 44,
        ProcessKey = 229,
        Q = 81,
        R,
        RightControl = 163,
        Right = 39,
        RightAlt = 165,
        RightShift = 161,
        RightWindows = 92,
        S = 83,
        Scroll = 145,
        Select = 41,
        SelectMedia = 181,
        Separator = 108,
        Sleep = 95,
        Space = 32,
        Subtract = 109,
        T = 84,
        Tab = 9,
        U = 85,
        Up = 38,
        V = 86,
        VolumeDown = 174,
        VolumeMute = 173,
        VolumeUp = 175,
        W = 87,
        X,
        Y,
        Z,
        Zoom = 251,
        ChatPadGreen = 202,
        ChatPadOrange
    }
    public enum ButtonState
    {
        Released,
        Pressed
    }
}
