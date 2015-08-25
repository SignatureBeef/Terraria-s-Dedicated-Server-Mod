
namespace Microsoft.Xna.Framework.Audio
{
    public class WaveBank
    {
        public static WaveBank[] Array;

        public WaveBank(AudioEngine audioEngine, string nonStreamingWaveBankFilename) { }
        public WaveBank(AudioEngine audioEngine, string streamingWaveBankFilename, int offset, short packetsize) { }
    }
}
