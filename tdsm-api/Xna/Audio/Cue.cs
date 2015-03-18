
namespace Microsoft.Xna.Framework.Audio
{
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
}
