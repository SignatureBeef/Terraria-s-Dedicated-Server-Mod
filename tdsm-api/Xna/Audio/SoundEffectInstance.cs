
namespace Microsoft.Xna.Framework.Audio
{
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
