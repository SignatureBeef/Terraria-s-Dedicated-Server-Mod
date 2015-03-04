
namespace Microsoft.Xna.Framework.Audio
{
    public class SoundBank
    {
        public static SoundBank[] Array;

        public SoundBank(AudioEngine audioEngine, string filename) { }

        public Cue GetCue(string name) { return default(Cue); }
    }
}
