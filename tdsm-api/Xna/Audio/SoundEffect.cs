
namespace Microsoft.Xna.Framework.Audio
{
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
}
